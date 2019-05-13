using Framework.ScriptableObjects.Variables;
using Project.History;
using Project.Social;
using UnityEngine;

public sealed class NetworkClient : NetworkManager
{
	[Header("Data")]
	[SerializeField] private SharedString AccountName;
	[SerializeField] private SharedString AccountPhone;
	[SerializeField] private SharedString ServerIP;

	[Header("Controllers")]
	public ScreenController ScreenController;
	public RoleplayController RoleplayController;
	public ModuleController ModuleController;
	public CompleteEvaluationController CompleteEvaluationController;
	public HistoryScreen HistoryScreen;
	public SocialScreen SocialScreen;

	public string ClientId { get { return AccountPhone.Value; } } 
	public string ClientName { get { return AccountName.Value; } }


	protected override void Initialize()
	{
		base.Initialize();

		udpMaster.LogReceivedMessages = true;
		NetworkMessage connectMessage = new NetworkMessage(NetworkMessageType.ConnectToServer, AccountPhone.Value, "", AccountName.Value);
		SendMessage(connectMessage, ServerIP.Value);

		// Initializes all ApplicationControllers.
		ModuleController.Initialize(this);
		RoleplayController.Initialize(this);
		CompleteEvaluationController.Initialize(this);
		HistoryScreen.Initialize(this);
		SocialScreen.Initialize(this);
	}

	protected override void Stop()
	{
		NetworkMessage disconnectMessage = new NetworkMessage(NetworkMessageType.DisconnectFromServer, ClientId);
		SendMessage(disconnectMessage);

		RoleplayController.ForceEndCall();

		base.Stop();
	}


	public void ConnectToServer()
	{

	}

	[ExecuteOnMainThread]
	public void TransmitRoleplayDescription(NetworkMessage message)
	{
		ModuleController.LockOutModule();
		RoleplayController.OnRoleplayLoaded(message.Message);
	}

	[ExecuteOnMainThread]
	public void TransmitCompleteEvaluation(NetworkMessage message)
	{
		if (CompleteEvaluationController.WaitingForCompleteEvaluation)
		{
			CompleteEvaluationController.PrepareScreen(message.Message);
			ScreenController.SwitchScreenToCompleteEvaluation();
		}
	}

	[ExecuteOnMainThread]
	public void RequestHistoryLogs(NetworkMessage message)
	{
		HistoryScreen.OnHistoryLogsAcquired(message.Message);
	}

	[ExecuteOnMainThread]
	public void RequestSocialLogs(NetworkMessage message)
	{
		SocialScreen.OnSocialAcquired(message.Message);
	}
}
