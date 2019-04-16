using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public sealed class NetworkClient : NetworkManager
{
	private string clientId;


	protected override void Awake()
	{
		base.Awake();

		NetworkMessage connectMsg = new NetworkMessage(NetworkMessageType.ConnectToServer, clientId);


		UnityEngine.Debug.Log(Framework.Features.Json.JsonUtility.ToJson(connectMsg));
		// TODO: Continue here with deserializing and such .

		//SendMessage(connectMsg);
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
