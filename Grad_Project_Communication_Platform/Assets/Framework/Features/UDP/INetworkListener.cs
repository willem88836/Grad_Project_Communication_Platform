namespace Framework.UDP
{
	public interface INetworkListener
	{
		void OnMessageReceived(string message);
	}
}
