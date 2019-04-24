namespace Framework.Features.UDP.Applied
{
	public interface IAppliedNetworkListener
	{
		void OnMessageReceived(UDPMessage message);
	}
}
