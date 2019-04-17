using Framework.Features.Json;

namespace Framework.Features.UDP
{
	public abstract class UDPMessage
	{
		public string SenderIP = "";


		public UDPMessage() { }

		public string Serialize()
		{
			return JsonUtility.ToJson(this);
		}
	}
}
