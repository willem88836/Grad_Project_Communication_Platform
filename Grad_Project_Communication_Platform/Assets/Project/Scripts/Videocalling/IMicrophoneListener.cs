namespace Project.Videocalling
{
	public interface IMicrophoneListener
	{
		void OnSamplesAcquired(float[] samples);
	}
}
