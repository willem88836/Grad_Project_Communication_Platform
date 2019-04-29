using Framework.Features.Json;
using Framework.Utils;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Framework.Features.UDP.Applied
{
	/// <summary>
	///		Does the same thing as UDPMaster.
	///		However, uses UDPMessages instead of byte[] 
	///		and has its own NetworkListener interface.
	///		User's Note: This is easier to use than UDPMaster, but far slower.
	/// </summary>
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

			LoggingUtilities.LogFormat("Message Received: ({0})", serializedMessage);

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
			networkListeners.SafeAdd(listener);
		}
		/// <summary>
		///		Removes INetworkListener from the list of objects
		///		that is invoked once a message is received.
		/// </summary>
		public void RemoveListener(IAppliedNetworkListener listener)
		{
			networkListeners.SafeRemove(listener);
		}
	}
}
