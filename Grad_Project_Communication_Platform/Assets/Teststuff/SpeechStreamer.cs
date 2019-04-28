using UnityEngine;
using Project.Videocalling;
using System.Collections.Generic;
using Framework.Utils;
using System.Text;

public class SpeechStreamer : MonoBehaviour, IMicrophoneListener
{
	public AudioSource AudioSource;
	public MicrophoneRecorder Microphone;

	public string AudioName = "MicrophoneOut";

	private AudioClip audioClipOut;


	// Start is called before the first frame update
	void Start()
    {
		audioClipOut = AudioClip.Create(AudioName, Microphone.SampleLength, 1, Microphone.RecordingFrequency, false);
		AudioSource.clip = audioClipOut;
		AudioSource.Play();

		Microphone.AddListener(this);
		Microphone.Initialize();
		Microphone.StartRecording();
	}

	public void OnSamplesAcquired(float[] samples)
	{
		StringBuilder sBilder = new StringBuilder();
		foreach(float f in samples)
		{
			sBilder.Append(f.ToString());
			sBilder.Append(',');
		}
		sBilder.Remove(sBilder.Length - 1, 1);

		List<byte> datl = new List<byte>(sBilder.ToString().ToByteArray());



		byte[] dat = datl.ToArray();




		string newdatstring = dat.ToObject<string>();

		string[] split = newdatstring.Split(',');

		float[] newSamples = new float[split.Length];
		for(int i = 0; i < split.Length; i++)
		{
			string s = split[i];
			newSamples[i] = float.Parse(s);
		}









		audioClipOut.SetData(newSamples, 0);
	}
}
