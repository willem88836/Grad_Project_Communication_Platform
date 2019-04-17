using Framework.Features.UDP;
using System;

public class NetworkMessage : UDPMessage
{
	public NetworkMessageType Type = NetworkMessageType.Null;
	public string SenderId = "";
	public string ReceiverId = "";
	public string Message = "";
	public string TimeStamp = "";

	public NetworkMessage() { }
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


public enum NetworkMessageType		// Content stored in message field
{
	Null,							// No content in message field
	// Client -> Server
	DisconnectFromServer,           // No content in message field
	Enqueue,                        // Module in message field
	Dequeue,                        // Module in message field
	StoreFootage,					// 
    TransmitEvaluationTest,			// Serialized Evaluation/test in message field
    RemoveConnection,				// No content in message field
	// Server -> Client
	TransmitRoleplayDescription,	// RoleplayDescription in message field
    TransmitFinalEvaluation,		// Final Evaluation in message field
	// Client -> Client
    TransmitFootage,				// Serialized Footage in message field 
    ForceEndCall,					// No content in message field
    ForceDisconnect,				// No content in message field
	// Client <-> Server
	ConnectToServer,				// Name and Id in message field (from client) OR nothing (from server)
}
