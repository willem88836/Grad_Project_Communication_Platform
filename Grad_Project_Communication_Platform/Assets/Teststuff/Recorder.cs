using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recorder : MonoBehaviour
{
	public AudioSource AudioSource;

	public AudioSource OtherAudioSource;

	const int X = 2048;

	float[] samples = new float[X];

	List<Transform> outputNodes = new List<Transform>();
	List<Transform> spectrumNodes = new List<Transform>();

	AudioClip outputClip;


    // Start is called before the first frame update
    void Start()
    {
		AudioSource.clip = Microphone.Start(null, true, 1, 48000);
		AudioSource.Play();
		outputClip = AudioClip.Create("outputaudio", X, 1, 48000, false	);
		for (int i = 0; i < samp2.Length; i++)
		{
			spectrumNodes.Add(GameObject.CreatePrimitive(PrimitiveType.Cube).transform);
			spectrumNodes[i].position = Vector3.right * i;
		}

		for (int i = 0; i < samples.Length; i++)
		{
			outputNodes.Add(GameObject.CreatePrimitive(PrimitiveType.Sphere).transform);
			outputNodes[i].position = Vector3.right * i + Vector3.forward;
		}
    }

	private void Update()
	{
		AudioSource.GetOutputData(samples, 0);
		//AudioSource.GetSpectrumData(spectrum, 0, FFTWindow.Rectangular);
		
		//for(int i = 0; i < spectrum.Length;i ++)
		//{
		//	spectrumNodes[i].transform.position = (Vector3.right * i) + (Vector3.up * spectrum[i] * 400);
		//}

		for (int i = 0; i < samples.Length; i++)
		{
			outputNodes[i].transform.position = (Vector3.right * i) + (Vector3.up * samples[i] * 400) + Vector3.forward;
		}

		outputClip.SetData(samples, 0);
		
		OtherAudioSource.clip = outputClip;
		OtherAudioSource.Play();



		OtherAudioSource.GetOutputData(samp2, 0);

		for (int i = 0; i < samp2.Length; i++)
		{
			outputNodes[i].transform.position = (Vector3.right * i) + (Vector3.up * samp2[i] * 400);
		}
	}
	public float[] samp2 = new float[X];
}
