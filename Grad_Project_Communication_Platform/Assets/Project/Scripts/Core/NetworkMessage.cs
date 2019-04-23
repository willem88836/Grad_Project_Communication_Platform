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
	public NetworkMessage(string senderIP, NetworkMessageType type, string senderId, DateTime timestamp, string receiverId = "", string message = "")
	{
		this.SenderIP = senderIP;
		this.Type = type;
		this.SenderId = senderId;
		this.ReceiverId = receiverId;
		this.Message = message;
		this.TimeStamp = timestamp.ToString();
	}
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


/* NetworkMessageType Foo messages. 
 * Null: 
 * DisconnectFromServer:				{Type:{value__:1},SenderId:"123456789",ReceiverId:"",Message:"",TimeStamp:"4/18/2019 2:44:43 PM",SenderIP:"10.102.37.150"}
 * Enqueue:								{Type:{value__:2},SenderId:"123456789",ReceiverId:"",Message:"Paraphrasing",TimeStamp:"4/17/2019 11:56:58 AM",SenderIP:"145.37.144.101"}
 * Dequeue:								{Type:{value__:3},SenderId:"123456789",ReceiverId:"",Message:"Paraphrasing",TimeStamp:"4/17/2019 11:56:58 AM",SenderIP:"145.37.144.101"}
 * StoreFootage:						
 * TransmitEvaluationTest:				
 * RemoveConnection:					
 * TransmitRoleplayDescription:			{Type:{value__:7},SenderId:"",ReceiverId:"123456788",Message:"{Id:"0",Client:{Name:"Steve",IP:"145.37.144.11",Id:"123456789"},Professional:{Name:"Stevette",IP:"145.37.144.12",Id:"123456788"},Case:{ClientCharacteristics:[],Assignments:[],Module:{value__:1}}}",TimeStamp:"4/20/2019 2:53:18 PM",SenderIP:"192.168.2.11"}
 * TransmitFinalEvaluation:				
 * TransmitFootage:						
 * ForceEndCall:						
 * ForceDisconnect:						
 * ConnectToServer:						{Type:{value__:12},SenderId:"123456789",ReceiverId:"",Message:"Steve",TimeStamp:"4/17/2019 11:56:58 AM",SenderIP:"145.37.144.101"}
 *										{Type:{value__:12},SenderId:"",ReceiverId:"123456789",Message:"",TimeStamp:"4/17/2019 11:59:17 AM",SenderIP:"145.37.144.101"}
 */
