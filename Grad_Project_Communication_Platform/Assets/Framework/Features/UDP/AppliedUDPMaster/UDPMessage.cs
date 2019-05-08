using Framework.Features.Json;

namespace Framework.Features.UDP.Applied
{
	public abstract class UDPMessage
	{
		[JsonIgnore] public string SenderIP = "";

		public UDPMessage() { }
	}
}
