using Framework.Features.UDP;
using System;

public class NetworkMessage : UDPMessage
{
	public NetworkMessageType Type = NetworkMessageType.Null;
	public string SenderId = "";
	public string ReceiverId = "";
	public string Message = "";
	public string TimeStamp = "";

	public NetworkMessage(NetworkMessageType type, string senderId, DateTime timestamp, string receiverId = "", string message = "")
	{
		this.Type = type;
		this.SenderId = senderId;
		this.ReceiverId = receiverId;
		this.Message = message;
		this.TimeStamp = timestamp.ToString();
	}
	public NetworkMessage(NetworkMessageType type, string senderId, string receiverId = "", string message = "")
	{
		this.Type = type;
		this.SenderId = senderId;
		this.ReceiverId = receiverId;
		this.Message = message;
		this.TimeStamp = DateTime.Now.ToString();
	}
}


public enum NetworkMessageType
{
	Null,
	// Client -> Server
	Enqueue,
    StoreFootage,
    TransmitEvaluationTest,
    RemoveConnection,
	// Server -> Client
	TransmitRoleplayDescription,
    TransmitFinalEvaluation,
	// Client -> Client
    TransmitFootage,
    ForceEndCall,
    ForceDisconnect,
	// Client <-> Server
	ConnectToServer
}
