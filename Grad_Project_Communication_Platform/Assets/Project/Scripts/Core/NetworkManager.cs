using Framework.Features.UDP.Applied;
using Framework.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

/// <summary>
///		Base class for all networking within the Unity Engine.
/// </summary>
public abstract class NetworkManager : MonoBehaviour, IAppliedNetworkListener
{
	[SerializeField] private EnsureFileDirectories ensureFileDirectories;

	[Header("Networking")]
	public int PortIn = 11000;
	public int PortOut = 11001;

	protected AppliedUDPMaster<NetworkMessage> udpMaster;
	protected Queue<Action> actionQueue = new Queue<Action>();

	private NetworkLogger<NetworkMessage> networkLogger;


	/// <summary>
	///		Stops the NetworkManager in the editor.
	/// </summary>
	protected void OnDestroy()
	{
		Stop();
	}

	protected virtual void Awake()
	{
		Initialize();
	}

	/// <summary>
	///		Stops or initializes the NetworkManager
	///		based on the paused state.
	/// </summary>
	protected virtual void OnApplicationPause(bool paused)
	{
		if (paused)
			Stop();
		else
			Initialize();
	}

	/// <summary>
	///		Invokes all methods that must be 
	///		executed on the main thread.
	/// </summary>
	protected virtual void Update()
	{
		while(actionQueue.Count > 0)
		{
			Action a = actionQueue.Dequeue();
			a.Invoke();
		}
	}

	/// <summary>
	///		Kills the network connection.
	/// </summary>
	protected virtual void Stop()
	{
		try
		{
			Debug.Log("Stopping NetworkManager");
			udpMaster.RemoveListener(this);
			udpMaster.RemoveListener(networkLogger);
			udpMaster.Kill();
		}
		catch (Exception ex)
		{
			Debug.LogError(ex.Message);
		}
	}
	/// <summary>
	///		Starts the network connection.
	/// </summary>
	protected virtual void Initialize()
	{
		try
		{
			Debug.Log("Initializing NetworkManager");
			SaveLoad.SavePath = Application.persistentDataPath;
			SaveLoad.Extention = ".sprc";
			SaveLoad.EncryptData = false;
			ensureFileDirectories.Invoke();

			if (udpMaster == null || !udpMaster.IsInitialized)
			{
				udpMaster = new AppliedUDPMaster<NetworkMessage>();
				udpMaster.Initialize("1.1.1.1", PortOut, PortIn);
				udpMaster.AddListener(this);
			}

			networkLogger = new NetworkLogger<NetworkMessage>() { LogFileName = "NetworkLogs.sprc" };
			networkLogger.Initialize(udpMaster);
		}
		catch(Exception ex)
		{
			Debug.LogError(ex.Message);
		}
	}

	/// <summary>
	///		Is called on NetworkMessage arrival, 
	///		and calls the method accompanied to it
	///		on either the secondary or main thread. 
	/// </summary>
	public void OnMessageReceived(UDPMessage message)
	{
		// Calls the function corresponding with the message's type.
		NetworkMessage networkMessage = (NetworkMessage)message;
		string methodName = networkMessage.Type.ToString();
		MethodInfo method = GetType().GetMethod(methodName);

		Action methodCall = delegate { method.Invoke(this, new object[] { networkMessage }); };

		if (method.GetCustomAttributes(typeof(ExecuteOnMainThread), true).Any())
			actionQueue.Enqueue(methodCall);
		else
			methodCall.Invoke();
	}

	/// <summary>
	///		Sends NetworkMessage through the UDPMaster.
	/// </summary>
	public void SendMessage(NetworkMessage message)
	{
		udpMaster.SendMessage(message);
	}
	/// <summary>
	///		Sends NetworkMessage  through the UDPMaster to the 
	///		specified IP Address.
	/// </summary>
	public void SendMessage(NetworkMessage message, string targetIP)
	{
		udpMaster.UpdateTargetIP(targetIP);
		SendMessage(message);
	}
}
