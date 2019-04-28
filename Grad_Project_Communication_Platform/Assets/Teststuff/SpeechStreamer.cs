using UnityEngine;
using Project.Videocalling;

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

	public void OnAudioAcquired(float[] samples)
	{
		audioClipOut.SetData(samples, 0);
	}

	public void OnSamplesAcquired(float[] samples)
	{
		audioClipOut.SetData(samples, 0);
	}
}
