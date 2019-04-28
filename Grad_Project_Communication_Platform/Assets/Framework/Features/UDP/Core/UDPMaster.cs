using Framework.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Framework.Features.UDP
{
	/// <summary>
	///		Responsible for sending and receiving messages. 
	/// </summary>
	public class UDPMaster
	{
		// HACK: This feels like the definition of a hack. Figure out why it doesn't work with the actual value.
		public int MessageBufferSize { get { return sendingSocket.SendBufferSize - 100; } } 

		#if UNITY_EDITOR
			public bool LocalHost = false;
		#endif

		protected int sendingPort;
		protected Socket sendingSocket;
		protected IPAddress sendingAddress;
		protected IPEndPoint sendingEndPoint;

		protected int receivingPort;
		protected UdpClient receiver;
		protected IPEndPoint receivingEndPoint;
		protected Thread receiverThread;

		private List<INetworkListener> networkListeners;


		/// <summary>
		///		Sets the UDPMaster up for sending messages,
		///		and start listening to messages. 
		/// </summary>
		public virtual void Initialize(int sendingPort = 11000, int receivingPort = 11001)
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
		public void SendMessage(byte[] message)
		{
			#if UNITY_EDITOR
				if (LocalHost)
				{
					DistributeMessage(message);
					return;
				}
			#endif
			sendingSocket.SendTo(message, sendingEndPoint);
		}
		/// <summary>
		///		Updates the current messaging target, and sends 
		///		a message across the network.
		/// </summary>
		public void SendMessage(byte[] message, string targetIP)
		{
			sendingEndPoint.Address = IPAddress.Parse(targetIP);
			SendMessage(message);
		}

		/// <summary>
		///		Creates a new thread dedicated to message receiving.
		/// </summary>
		protected void StartReceivingMessages()
		{
			ThreadStart activity = new ThreadStart(() => { while (true) { ReceiveNetworkMessage(); } });
			receiverThread = new Thread(activity);
			receiverThread.Start();
		}
		/// <summary>
		///		Receives network messages.
		/// </summary>
		protected virtual void ReceiveNetworkMessage()
		{
			try
			{
				// TODO: This throws an error when killing the network connection.
				byte[] messageByteArray = receiver.Receive(ref receivingEndPoint);
				DistributeMessage(messageByteArray);
			}
			catch (System.Exception ex)
			{
				LoggingUtilities.Log(ex.Message + " " + ex.StackTrace);
			}
		}

		/// <summary>
		///		Calls the networkListeners.
		/// </summary>
		protected virtual void DistributeMessage(byte[] message)
		{
			foreach (INetworkListener networkListener in networkListeners)
			{
				networkListener.OnMessageReceived(message);
			}

			LoggingUtilities.LogFormat("Received message (\"{0}\") from ip ({1}) using port ({2})", message.ToString(), receivingEndPoint.Address, receivingPort);
		}


		/// <summary>
		///		Adds INetworkListener to the list of objects that 
		///		is invoked once a message is received.
		/// </summary>
		public void AddListener(INetworkListener listener)
		{
			networkListeners.SafeAdd(listener);
		}
		/// <summary>
		///		Removes INetworkListener from the list of objects
		///		that is invoked once a message is received.
		/// </summary>
		public void RemoveListener(INetworkListener listener)
		{
			networkListeners.SafeRemove(listener);
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
