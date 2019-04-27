using Framework.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MicrophoneRecorder : MonoBehaviour
{
	[Header("Scene References")]
	public AudioMixer AudioMixer;
	public AudioSource AudioSource;

	[Header("Mixer Settings")]
	[Range(-80, 20)] public int MicrophoneVolume = -80;
	public string AudioMixerMicrophoneGroupName = "Microphone";
	public string AudioMixerMicrophoneVolumeName = "MicrophoneVolume";

	[Header("Recording Settings")]
	public int SampleLength = 1024;
	public int RecordingLength = 1;
	public int RecordingFrequency = 48000;
	public int RecordingChannel = 0;

	public Action<float[]> OnAudioAcquired;

	private AudioClip outputClip;
	private float[] samples;

	// TODO: Add a stop & start recording method. 
	private void Start()
	{
		// Mutes the Audiomixer.
		AudioMixer.SetFloat(AudioMixerMicrophoneVolumeName, MicrophoneVolume);

		// Adds the audiosource to a AudioMixerGroup (if it exists).
		AudioMixerGroup[] audioMixerGroups = AudioMixer.FindMatchingGroups(AudioMixerMicrophoneGroupName);
		if (audioMixerGroups.Length == 0)
			throw new KeyNotFoundException("Referenced AudioMixer Group doesn't exist");
		AudioSource.outputAudioMixerGroup = audioMixerGroups[0];

		// Prepares the AudioSource to play Microphone sounds.
		outputClip = Microphone.Start(null, true, RecordingLength, RecordingFrequency);
		AudioSource.clip = outputClip;
		AudioSource.loop = true;

		samples = new float[SampleLength];

		// Waits until the Microphone has start to avoid stutter.
		while (Microphone.GetPosition(null) <= 0)
		{ }

		AudioSource.Play();
	}

	private void Update()
	{
		AudioSource.GetOutputData(samples, RecordingChannel);
		OnAudioAcquired.SafeInvoke(samples);
	}
}
