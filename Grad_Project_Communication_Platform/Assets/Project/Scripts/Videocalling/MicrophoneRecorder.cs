using Framework.Utils;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Project.Videocalling
{
	/// <summary>
	///		Records the device's microphone.
	/// </summary>
	public class MicrophoneRecorder : MonoBehaviour
	{
		[Header("Scene References")]
		public AudioMixer AudioMixer;
		public AudioSource AudioSource;

		[Header("Mixer Settings")]
		[Range(-80, 20)] public int MicrophoneVolume = -80;
		public string AudioMixerMicrophoneVolumeName = "MicrophoneVolume";

		[Header("Recording Settings")]
		public int SampleLength = 1024;
		public int RecordingLength = 1;
		public int RecordingFrequency = 48000;
		public int RecordingChannel = 0;

		private AudioClip outputClip;
		private float[] samples;


		private bool isRecording = false;
		private List<IMicrophoneListener> listeners = new List<IMicrophoneListener>();


		public void Initialize()
		{
			// Mutes the Audiomixer.
			AudioMixer.SetFloat(AudioMixerMicrophoneVolumeName, MicrophoneVolume);

			// Prepares the AudioSource to play Microphone sounds.
			outputClip = null;//Microphone.Start(null, true, RecordingLength, RecordingFrequency);
			AudioSource.clip = outputClip;
			AudioSource.loop = true;

			samples = new float[SampleLength];

			// Waits until the Microphone has start to avoid stutter.
			//while (Microphone.GetPosition(null) <= 0) { }
		}

		/// <summary>
		///		Acquires the microphone input every frame
		///		and distributes it to the MicrophoneListeners.
		/// </summary>
		private IEnumerator<YieldInstruction> Record()
		{
			while (true)
			{
				AudioSource.GetOutputData(samples, RecordingChannel);
				foreach (IMicrophoneListener listener in listeners)
					listener.OnSamplesAcquired(samples);
				
				yield return new WaitForEndOfFrame();
			}
		}


		public void StartRecording()
		{
			if (isRecording)
				return;

			AudioSource.Play();
			isRecording = true;
			StartCoroutine(Record());
		}
		public void StopRecording()
		{
			if (!isRecording)
				return;

			AudioSource.Stop();
			isRecording = false;
			StopCoroutine(Record());
		}
		public void ToggleRecording()
		{
			if (isRecording)
				StopRecording();
			else
				StartRecording();
		}

		public void AddListener(IMicrophoneListener listener)
		{
			listeners.SafeAdd(listener);
		}
		public void RemoveListener(IMicrophoneListener listener)
		{
			listeners.SafeRemove(listener);
		}
	}
}
