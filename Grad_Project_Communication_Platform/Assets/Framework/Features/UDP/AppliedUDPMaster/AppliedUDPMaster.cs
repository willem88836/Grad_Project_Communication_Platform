using Framework.Features.Json;
using Framework.Utils;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Framework.Features.UDP.Applied
{
	// TODO: rename this thing.. I don't know what to call this right now. 
	public sealed class AppliedUDPMaster<T> : UDPMaster, INetworkListener where T : UDPMessage
	{
		private List<IAppliedNetworkListener> networkListeners;

		/// <inheritdoc />
		public override void Initialize(int sendingPort = 11000, int receivingPort = 11001)
		{
			base.Initialize(sendingPort, receivingPort);

			networkListeners = new List<IAppliedNetworkListener>();
			this.AddListener(this);
		}

		/// <summary>
		///		Sends a message across the network.
		/// </summary>
		public void SendMessage(T message)
		{
			string msg = JsonUtility.ToJson(message);
			byte[] messageByteArray = Encoding.ASCII.GetBytes(msg);
			SendMessage(messageByteArray);

			LoggingUtilities.LogFormat("Sent message (\"{0}\") to ip ({1}) using port ({2})", msg, sendingEndPoint.Address, sendingEndPoint.Port);
		}
		/// <summary>
		///		Updates the current messaging target, and sends 
		///		a message across the network.
		/// </summary>
		public void SendMessage(T message, string ipAddress)
		{
			sendingEndPoint.Address = IPAddress.Parse(ipAddress);
			SendMessage(message);
		}

		/// <inheritdoc />
		public void OnMessageReceived(byte[] message)
		{
			string serializedMessage = Encoding.ASCII.GetString(message);
			UDPMessage udpMessage = (UDPMessage)JsonUtility.FromJson(serializedMessage, typeof(T));

			foreach (IAppliedNetworkListener listener in networkListeners)
			{
				listener.OnMessageReceived(udpMessage);
			}
		}


		/// <summary>
		///		Adds INetworkListener to the list of objects that 
		///		is invoked once a message is received.
		/// </summary>
		public void AddListener(IAppliedNetworkListener listener)
		{
			if (!networkListeners.Contains(listener))
			{
				networkListeners.Add(listener);
			}
			else
			{
				LoggingUtilities.Log("Can't add duplicate listener!");
			}
		}
		/// <summary>
		///		Removes INetworkListener from the list of objects
		///		that is invoked once a message is received.
		/// </summary>
		public void RemoveListener(IAppliedNetworkListener listener)
		{
			int index = networkListeners.IndexOf(listener);
			if (index != -1)
			{
				networkListeners.RemoveAt(index);
			}
			else
			{
				LoggingUtilities.Log("Can't remove unlisted listener!");
			}
		}
	}
}
