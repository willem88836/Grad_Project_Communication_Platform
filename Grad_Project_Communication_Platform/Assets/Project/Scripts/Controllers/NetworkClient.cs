using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class NetworkClient : NetworkManager
{
	private string clientId; // TODO: Set client id to match phone number or something.


	protected override void Awake()
	{
		base.Awake();

		NetworkMessage connectMsg = new NetworkMessage(NetworkMessageType.ConnectToServer, clientId);
		// TODO: Continue here with deserializing and such.
		SendMessage(connectMsg);
	}




	public void ConnectToServer(NetworkMessage message)
	{

	}

	public void TransmitRoleplayDescription(NetworkMessage message)
	{

	}

	public void TransmitFinalEvaluation(NetworkMessage message)
	{

	}

	public void TransmitFootage(NetworkMessage message)
	{

	}

	public void ForceEndCall(NetworkMessage message)
	{

	}

	public void ForceDisconnect(NetworkMessage message)
	{

	}
}
