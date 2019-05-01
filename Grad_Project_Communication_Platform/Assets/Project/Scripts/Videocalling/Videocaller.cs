using Framework.Features.UDP;
using Framework.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Project.Videocalling
{
	/// <summary>
	///		Is used for videocalling with someone across the network in Unity.
	///		
	///		A message consists of two parts: 1) a header, and 2) data. 
	///		A header consists of the type of data: Audio or video. And in case of video,
	///		the second byte refers to the howmanieth chunk the package is in order
	///		to update the right pixel.
	/// </summary>
	public class Videocaller : MonoBehaviour, INetworkListener, IMicrophoneListener
	{
		private const byte VIDEO_ID = 0;
		private const byte AUDIO_ID = 1;
		private const byte VIDEORES_ID = 2;


		public Action OnCallEnded;
		//HACK: Changing the width/height of a Texture2D is not supported yet.
		public Action<Texture2D> OnOtherFootageApplied;

		// Networking locals.
		public int PortA = 11002;
		public int PortB = 11003;

		private UDPMaster udpMaster;


		// Sending Locals
		[Space]
		public MicrophoneRecorder Microphone;
		public WebCamTexture OwnFootage { get; private set; }

		private string targetIP;
		private int frameRate;
		private float resolutionScale;


		// Receiving Locals
		public Texture2D OtherFootage { get; private set; }

		public AudioSource AudioSource;
		private AudioClip audioClipOut;

		private bool dimensionsEstablished = false;

		private Color32[] currentColors;
		private bool colorsUpdated;

		private Queue<Action> mainThreadActions = new Queue<Action>();


		private void Update()
		{
			if (colorsUpdated)
			{
				Action applyColors = delegate
				{
					if (OtherFootage != null)
					{
						OtherFootage.SetPixels32(currentColors);
						OtherFootage.Apply();
						OnOtherFootageApplied.SafeInvoke(OtherFootage);
					}
				};

				mainThreadActions.Enqueue(applyColors);
				colorsUpdated = false;
			}

			while (mainThreadActions.Count > 0)
			{
				Action a = mainThreadActions.Dequeue();
				a.Invoke();
			}
		}


		/// <summary>
		///		Initializes the network connection.
		/// </summary>
		public void Initialize(int portA = 11002, int portB = 11003)
		{
			// Makes sure the resolutionscale is not out of bounds.
			resolutionScale = Mathf.Clamp01(resolutionScale);

			// Initializes the Networking.
			udpMaster = new UDPMaster();
			udpMaster.Initialize("1.1.1.1", portA, portB);
			udpMaster.AddListener(this);
			#if UNITY_EDITOR
				udpMaster.LogReceivedMessages = false;
			#endif

			// Initializes voice recording.
			Microphone.Initialize();
			Microphone.AddListener(this);

			// Initializes audio output for peers' microphone input.
			audioClipOut = AudioClip.Create("OtherMicrophoneAudio", Microphone.SampleLength, 1, Microphone.RecordingFrequency, false);
			AudioSource.clip = audioClipOut;

			// HACK: This will most definitely break at some point. 
			WebCamDevice frontCam = WebCamTexture.devices.Where((WebCamDevice d) => d.isFrontFacing).ToArray()[0];
			OwnFootage = new WebCamTexture(frontCam.name);
			OwnFootage.name = "Webcamfootage_Self";
			OwnFootage.Play();
		}


		/// <summary>
		///		Starts a videocall.
		/// </summary>
		public void StartCalling(string targetIP, int frameRate, float resolutionScale)
		{
			this.targetIP = targetIP;
			this.frameRate = frameRate;
			this.resolutionScale = resolutionScale;

			udpMaster.UpdateTargetIP(targetIP);

			Microphone.StartRecording();
			AudioSource.Play();

			StopAllCoroutines();
			StartCoroutine(SendFootage());
		}

		/// <summary>
		///		Ends the videocall. 
		/// </summary>
		public void StopCalling(bool isForcedByPeer)
		{
			if (isForcedByPeer)
				udpMaster.SendMessage(new byte[0]);

			udpMaster.Kill();
			OwnFootage.Stop();
			dimensionsEstablished = false;
			OnCallEnded.SafeInvoke();
			AudioSource.Stop();
			Microphone.StopRecording();
			dimensionsEstablished = false;
		}


		/// <summary>
		///		Collects the own video footage, reduces resolution, 
		///		and sends it across the network. 
		/// </summary>
		private IEnumerator<YieldInstruction> SendFootage()
		{
			while (OwnFootage.width == 0 || OwnFootage.height == 0)
			{
				yield return new WaitForEndOfFrame();
			}

			int ownFootageWidth = OwnFootage.width;
			int ownFootageHeight = OwnFootage.height;

			// Converts the width and height to byte array and sends it across the network.
			List<byte> resolutionByteList = new List<byte>();
			resolutionByteList.Add(VIDEORES_ID);
			int videoWidth = (int)(ownFootageWidth * resolutionScale);
			int videoHeight = (int)(ownFootageHeight * resolutionScale);
			resolutionByteList.AddRange(videoWidth.ToByteArray());
			resolutionByteList.AddRange(videoHeight.ToByteArray());
			byte[] resolutionByteArray = resolutionByteList.ToArray();

			while (!dimensionsEstablished)
			{
				udpMaster.SendMessage(resolutionByteList.ToArray());
				yield return new WaitForSeconds(1);
			}


			// TODO: Do this on a different thread (framerate and such)?
			while (true)
			{
				// Lowers the resolution of the video frame.
				Color32[] frame = OwnFootage.GetPixels32();
				List<Color32> lowResFrame = new List<Color32>();
				for (float i = 0; i < videoHeight; i++)
				{
					for (float j = 0; j < videoWidth; j++)
					{
						int x = (int)(j / resolutionScale);
						int y = (int)(i / resolutionScale);

						int k = y * ownFootageWidth + x;

						Color32 pixel = frame[k];
						lowResFrame.Add(pixel);
					}
				}

				// Sends the low resolution frame in chunks across the network.
				// Color32 contains 3 byte values (RGB). Therefore, everything is done in 3s.
				int colorBufferSize = (int)(udpMaster.MessageBufferSize / 3f);
				int chunkCount = Mathf.CeilToInt((float)lowResFrame.Count / colorBufferSize);
				for (int i = 0; i < chunkCount; i++)
				{
					// Establishes the message size. 
					int j = i * colorBufferSize;
					int length = i == chunkCount - 1 // Is the last chunk.
						? lowResFrame.Count - j
						: colorBufferSize;

					// Creates the message.
					List<byte> byteList = new List<byte>();
					byteList.Add(VIDEO_ID);
					byteList.Add((byte)i);
					for (int k = 0; k < length; k++)
					{
						Color32 color = lowResFrame[k];
						byteList.Add(color.r);
						byteList.Add(color.g);
						byteList.Add(color.b);
					}

					udpMaster.SendMessage(byteList.ToArray());
					yield return new WaitForEndOfFrame();
				}
				yield return new WaitForEndOfFrame();
			}
		}

		/// <inheritdoc />
		public void OnSamplesAcquired(float[] samples)
		{
			// TODO: There must be a better solution to do this.
			System.Text.StringBuilder sBilder = new System.Text.StringBuilder();
			foreach (float f in samples)
			{
				sBilder.Append(f.ToString());
				sBilder.Append(',');
			}
			sBilder.Remove(sBilder.Length - 1, 1);

			List<byte> byteList = new List<byte>(sBilder.ToString().ToByteArray());
			byteList.Insert(0, AUDIO_ID);

			byte[] data = byteList.ToArray();

			udpMaster.SendMessage(data);
		}


		/// <inheritdoc />
		public void OnMessageReceived(byte[] message)
		{
			if (message.Length == 0)
			{
				StopCalling(true);
			}
			else if (message[0] == VIDEO_ID)
			{
				ProcessColorData(message);
			}
			else if (message[0] == AUDIO_ID)
			{
				ProcessAudioData(message);
			}
			else if (message[0] == VIDEORES_ID)
			{
				ProcessDimensionData(message);
			}
		}

		private void ProcessDimensionData(byte[] data)
		{
			if (dimensionsEstablished)
				return;

			// Converts the byte array to two values.
			int intByteArrayLength = ObjectUtilities.INT_BYTEARRAYLENGTH;
			int width = data.SubArray(1, intByteArrayLength).ToObject<int>();
			int height = data.SubArray(intByteArrayLength + 1, intByteArrayLength).ToObject<int>();

			// Texture2D cannot be created on a non-main thread.
			Action textureCreation = delegate
			{
				// Creates the texture.
				OtherFootage = new Texture2D(width, height);
				OtherFootage.name = "Webcamfootage_Other";
				currentColors = new Color32[width * height];
				dimensionsEstablished = true;
			};
			mainThreadActions.Enqueue(textureCreation);

			Debug.Log("Dimensions Established");
		}

		private void ProcessColorData(byte[] data)
		{
			int startIndex = data[2] * (int)(udpMaster.MessageBufferSize / 3f);

			// Applies the colors to the texture.
			int otherFootageWidth = OtherFootage.width;
			int otherFootageHeight = OtherFootage.height;
			for (int i = 2; i < data.Length; i += 3)
			{
				// Converts the byte info to Color32.
				byte r = data[i];
				byte g = data[i + 1];
				byte b = data[i + 2];
				Color32 color = new Color32(r, g, b, byte.MaxValue);

				// Determines the x and y coordinates of the color.
				int j = (int)((startIndex + i) / 3f % otherFootageWidth);
				int k = (int)((startIndex + i) / 3f / otherFootageWidth);

				int l = k * otherFootageWidth + j;
				currentColors[l] = color;
			}

			colorsUpdated = true;
		}

		private void ProcessAudioData(byte[] data)
		{
			// TODO: There must be a better solution to do this.
			List<byte> dataList = new List<byte>(data);
			dataList.RemoveAt(0);

			string dataString = dataList.ToArray().ToObject<string>();
			string[] split = dataString.Split(',');

			float[] samples = new float[split.Length];

			for (int i = 0; i < split.Length; i++)
			{
				string s = split[i];
				samples[i] = float.Parse(s);
			}

			// Audiodata cannot be set on non-main thread.
			Action setAudio = delegate
			{
				audioClipOut.SetData(samples, 0);
			};
			mainThreadActions.Enqueue(setAudio);
		}
	}
}
