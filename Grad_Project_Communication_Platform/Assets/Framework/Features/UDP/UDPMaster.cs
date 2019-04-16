using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Text;

namespace Framework.UDP
{
	/// <summary>
	///		Responsible for sending and receiving messages. 
	/// </summary>
	public class UDPMaster
	{
		public static UDPMaster Instance;


		public int Port = 11000;


		protected Socket sendingSocket;
		protected IPAddress targetAddress;
		protected IPEndPoint targetEndpoint;

		protected IPEndPoint receivingEndPoint;
		protected UdpClient listener;

		protected List<INetworkListener> networkListeners;


		/// <summary>
		///		Sends a message across the network.
		/// </summary>
		public static void SendMessage(string message)
		{
			if (Instance == null)
				throw new NullReferenceException("UDPMaster is not initialized");

			Instance.SendNetworkMessage(message);
		}
		/// <summary>
		///		Adds INetworkListener to the list of objects that 
		///		is invoked once a message is received.
		/// </summary>
		public static void AddListener(INetworkListener listener)
		{
			List<INetworkListener> networkListeners = UDPMaster.Instance.networkListeners;
			if (!networkListeners.Contains(listener))
			{
				networkListeners.Add(listener);
			}
			else
			{
				Console.WriteLine("Can't add duplicate listener!");
			}
		}
		/// <summary>
		///		Removes INetworkListener from the list of objects
		///		that is invoked once a message is received.
		/// </summary>
		public static void RemoveListener(INetworkListener listener)
		{
			List<INetworkListener> networkListeners = UDPMaster.Instance.networkListeners;
			int index = networkListeners.IndexOf(listener);
			if (index != -1)
			{
				networkListeners.RemoveAt(index);
			}
			else
			{
				Console.WriteLine("Can't remove unlisted listener!");
			}
		}


		public virtual void Initialize()
		{
			if (Instance != null)
			{
				throw new Exception("UDPMaster instance already existing. Can't have two instances running simultaneously.");
			}

			sendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			targetAddress = GetTargetIP();
			targetEndpoint = new IPEndPoint(targetAddress, Port);

			listener = new UdpClient(Port);
			receivingEndPoint = new IPEndPoint(IPAddress.Any, Port);

			networkListeners = new List<INetworkListener>();

			StartReceivingMessages();

			Instance = this;
		}

		/// <summary>
		///		Returns the target IP address.
		/// </summary>
		protected virtual IPAddress GetTargetIP()
		{
			var host = Dns.GetHostEntry(Dns.GetHostName());
			IEnumerable<IPAddress> ips = host.AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
			string finalIP = ips.ToArray()[0].ToString();

			string[] ipSegments = finalIP.Split('.');
			finalIP = finalIP.Substring(0, finalIP.Length - ipSegments[ipSegments.Length - 1].Length);
			finalIP += "255";

			return IPAddress.Parse(finalIP);
		}

		/// <summary>
		///		Sends a message across the network.
		/// </summary>
		protected virtual void SendNetworkMessage(string message)
		{
			byte[] messageByteArray = Encoding.ASCII.GetBytes(message);
			sendingSocket.SendTo(messageByteArray, targetEndpoint);
			OnNetworkMessageSent(message);

			Console.WriteLine(string.Format("Message (\"{0}\") sent to ip ({1}) using port ({2})", message, targetEndpoint.Address, targetEndpoint.Port));
		}

		/// <summary>
		///		Creates a new thread dedicated to message receiving.
		/// </summary>
		protected virtual void StartReceivingMessages()
		{
			ThreadStart activity = new ThreadStart(() => { while (true) { ReceiveNetworkMessage(); } });
			Thread messageThread = new Thread(activity);
			messageThread.Start();
		}
		/// <summary>
		///		Receives network messages.
		/// </summary>
		protected virtual void ReceiveNetworkMessage()
		{
			byte[] messageByteArray = listener.Receive(ref receivingEndPoint);
			string message = Encoding.ASCII.GetString(messageByteArray);

			foreach(INetworkListener listener in networkListeners)
			{
				listener.OnMessageReceived(message);
			}

			OnNetworkMessageReceived(message);
			Console.WriteLine(string.Format("Received message (\"{0}\") from ip ({1}) using port ({2})", message, targetEndpoint.Address, Port));
		}

		/// <summary>
		///		Is called after a message is received. 
		/// </summary>
		protected virtual void OnNetworkMessageReceived(string messsage) { }
		/// <summary>
		///		Is called after a message is sent.
		/// </summary>
		protected virtual void OnNetworkMessageSent(string message) { }
	}
}
