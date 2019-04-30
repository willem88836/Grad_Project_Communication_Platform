using Framework.Features.Json;
using Framework.Utils;
using System.Collections.Generic;
using System.Text;

namespace Framework.Features.UDP.Applied
{
	/// <summary>
	///		Does the same thing as UDPMaster.
	///		However, uses UDPMessages instead of byte[] 
	///		and has its own NetworkListener interface.
	///		User's Note: This is easier to use than UDPMaster, but far slower.
	/// </summary>
	public sealed class AppliedUDPMaster<T> : UDPMaster where T : UDPMessage
	{
		private List<IAppliedNetworkListener> appliedNetworkListeners = new List<IAppliedNetworkListener>();


		/// <summary>
		///		Sends a message across the network.
		/// </summary>
		public void SendMessage(T message)
		{
			string msg = JsonUtility.ToJson(message);
			byte[] messageByteArray = Encoding.ASCII.GetBytes(msg);
			SendMessage(messageByteArray);

			LoggingUtilities.LogFormat("Sent message (\"{0}\") to ip ({1}) using port ({2})", msg, SendingEndPoint.Address, SendingEndPoint.Port);
		}
		/// <summary>
		///		Updates the current messaging target, and sends 
		///		a message across the network.
		/// </summary>
		public void SendMessage(T message, string ipAddress)
		{
			UpdateTargetIP(ipAddress);
			SendMessage(message);
		}


		/// <inheritdoc />
		protected override void DistributeMessage(byte[] message)
		{
			base.DistributeMessage(message);

			string serializedMessage = Encoding.ASCII.GetString(message);
			UDPMessage udpMessage = (UDPMessage)JsonUtility.FromJson(serializedMessage, typeof(T));
			udpMessage.SenderIP = ReceivingEndPoint.Address.ToString();

			if (LogReceivedMessages && UnityEngine.Application.isEditor)
			{
				LoggingUtilities.LogFormat(
					"Message Received: ({0})", 
					serializedMessage);
			}

			foreach (IAppliedNetworkListener appliedListeners in appliedNetworkListeners)
			{
				appliedListeners.OnMessageReceived(udpMessage);
			}
		}


		/// <summary>
		///		Adds INetworkListener to the list of objects that 
		///		is invoked once a message is received.
		/// </summary>
		public void AddListener(IAppliedNetworkListener listener)
		{
			appliedNetworkListeners.SafeAdd(listener);
		}
		/// <summary>
		///		Removes INetworkListener from the list of objects
		///		that is invoked once a message is received.
		/// </summary>
		public void RemoveListener(IAppliedNetworkListener listener)
		{
			appliedNetworkListeners.SafeRemove(listener);
		}
	}
}
