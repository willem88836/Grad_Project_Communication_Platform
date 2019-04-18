using Framework.Utils;
using Framework.Features.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Framework.Features.UDP
{
	/// <summary>
	///		Responsible for sending and receiving messages. 
	/// </summary>
	public sealed class UDPMaster<T> where T : UDPMessage
	{
		private int sendingPort;
		private Socket sendingSocket;
		private IPAddress sendingAddress;
		private IPEndPoint sendingEndPoint;

		private int receivingPort;
		private UdpClient receiver;
		private IPEndPoint receivingEndPoint;
		private Thread receiverThread;

		private List<INetworkListener> networkListeners;

		/// <summary>
		///		Sets the UDPMaster up for sending messages,
		///		and start listening to messages. 
		/// </summary>
		public void Initialize(int sendingPort = 11000, int receivingPort = 11001)
		{
			this.receivingPort = receivingPort;
			this.sendingPort = sendingPort;

			sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			sendingAddress = GetLocalBroadcast();
			sendingEndPoint = new IPEndPoint(sendingAddress, this.sendingPort);

			receiver = new UdpClient(this.receivingPort);
			receivingEndPoint = new IPEndPoint(IPAddress.Any, this.receivingPort);

			networkListeners = new List<INetworkListener>();

			StartReceivingMessages();

			LoggingUtilities.LogFormat("UDPMaster initialized with ip ({0}) and sending port ({1}) receiving port ({2})", sendingEndPoint.Address, sendingPort, this.receivingPort);
		}
		/// <summary>
		///		Closes used port and message receiving thread. 
		/// </summary>
		public void Kill()
		{
			LoggingUtilities.LogFormat("Killing UDPMaster");

			receiverThread.Abort();
			receiver.Close();
			sendingSocket.Close();
		}


		/// <summary>
		///		Sends a message across the network.
		/// </summary>
		public void SendMessage(T message)
		{
			string msg = JsonUtility.ToJson(message);
			byte[] messageByteArray = Encoding.ASCII.GetBytes(msg);
			sendingSocket.SendTo(messageByteArray, sendingEndPoint);

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


		/// <summary>
		///		Creates a new thread dedicated to message receiving.
		/// </summary>
		private void StartReceivingMessages()
		{
			ThreadStart activity = new ThreadStart(() => { while (true) { ReceiveNetworkMessage(); } });
			receiverThread = new Thread(activity);
			receiverThread.Start();
		}
		/// <summary>
		///		Receives network messages.
		/// </summary>
		private void ReceiveNetworkMessage()
		{
			try
			{
				byte[] messageByteArray = receiver.Receive(ref receivingEndPoint);
				string message = Encoding.ASCII.GetString(messageByteArray);

				LoggingUtilities.LogFormat("Received message (\"{0}\") from ip ({1}) using port ({2})", message, receivingEndPoint.Address, receivingPort);

				UDPMessage udpMessage = (UDPMessage)JsonUtility.FromJson(message, typeof(T));

				foreach (INetworkListener listener in networkListeners)
				{
					listener.OnMessageReceived(udpMessage);
				}
			}
			catch (System.Exception ex)
			{
				LoggingUtilities.Log(ex.Message + " " + ex.StackTrace);
			}
		}


		/// <summary>
		///		Adds INetworkListener to the list of objects that 
		///		is invoked once a message is received.
		/// </summary>
		public void AddListener(INetworkListener listener)
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
		public void RemoveListener(INetworkListener listener)
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


		public void UpdateTargetIP(string ipAddress)
		{
			sendingEndPoint.Address = IPAddress.Parse(ipAddress);
			LoggingUtilities.LogFormat("Updating target IP to: {0}", ipAddress);
		}

		//TODO: This does not always return the right IP address..
		public IPAddress GetLocalBroadcast()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			IEnumerable<IPAddress> ips = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
			string finalIP = ips.ToArray()[0].ToString();

			string[] ipChunks = finalIP.Split('.');
			finalIP = finalIP.Substring(0, finalIP.Length - ipChunks[ipChunks.Length - 1].Length);
			finalIP += "255";

			return IPAddress.Parse(finalIP);
		}

		public IPAddress GetLocalIP()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			IEnumerable<IPAddress> ips = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
			string finalIP = ips.ToArray()[0].ToString();
			return IPAddress.Parse(finalIP);
		}
	}
}
