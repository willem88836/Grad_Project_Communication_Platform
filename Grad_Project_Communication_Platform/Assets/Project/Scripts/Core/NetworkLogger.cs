using Framework.Features.UDP.Applied;
using System;
using System.IO;
using UnityEngine;

[Serializable]
public sealed class NetworkLogger<T> : IAppliedNetworkListener where T : UDPMessage
{
	public string LogFileName;
	private string storagePath;
	

	public void Initialize(AppliedUDPMaster<T> udpMaster)
	{
		CreateLog();
		udpMaster.AddListener(this);
	}

	public void OnMessageReceived(UDPMessage message)
	{
		LogMessage((T)message);
	}


	private void CreateLog()
	{
		storagePath = Path.Combine(Application.persistentDataPath, LogFileName);

		if (!File.Exists(storagePath))
		{
			File.Create(storagePath);
		}

		Debug.Log("Logging File Path: " + storagePath);
	}

	private void LogMessage(T message)
	{
		string json = Framework.Features.Json.JsonUtility.ToJson(message) + '\n';
		File.AppendAllText(storagePath, json);
	}


	public void OpenLog()
	{
		System.Diagnostics.Process.Start(storagePath);
	}

	public void ClearLog()
	{
		File.Copy(storagePath, storagePath + Time.time);
		File.WriteAllText(storagePath, "");
	}
}
