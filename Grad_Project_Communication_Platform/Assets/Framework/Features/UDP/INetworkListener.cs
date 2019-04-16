namespace Framework.Features.UDP
{
	public interface INetworkListener
	{
		void OnMessageReceived(UDPMessage message);
	}
}
