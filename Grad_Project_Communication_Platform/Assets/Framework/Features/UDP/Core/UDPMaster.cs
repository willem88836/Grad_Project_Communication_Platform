using Framework.Utils;
using System.Collections.Generic;
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
		public int MessageBufferSize { get { return SendingSocket.SendBufferSize - 100; } } 

		#if UNITY_EDITOR
			public bool LocalHost = false;
			public bool LogReceivedMessages = true;
		#endif

		protected int SendingPort;
		protected Socket SendingSocket;
		protected IPAddress SendingAddress;
		protected IPEndPoint SendingEndPoint;

		protected int ReceivingPort;
		protected UdpClient Receiver;
		protected IPEndPoint ReceivingEndPoint;
		protected Thread ReceiverThread;

		protected List<INetworkListener> NetworkListeners = new List<INetworkListener>();


		/// <summary>
		///		Sets the UDPMaster up for sending messages,
		///		and start listening to messages. 
		/// </summary>
		public virtual void Initialize(string sendingAddress, int sendingPort = 11000, int receivingPort = 11001)
		{
			this.ReceivingPort = receivingPort;
			this.SendingPort = sendingPort;
			this.SendingAddress = IPAddress.Parse(sendingAddress);

			SendingSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
			SendingEndPoint = new IPEndPoint(this.SendingAddress, this.SendingPort);

			Receiver = new UdpClient(this.ReceivingPort);
			ReceivingEndPoint = new IPEndPoint(IPAddress.Any, this.ReceivingPort);

			ReceiveMessages();

			LoggingUtilities.LogFormat("UDPMaster initialized with ip ({0}) and sending port ({1}) receiving port ({2})", SendingEndPoint.Address, sendingPort, this.ReceivingPort);
		}
		/// <summary>
		///		Closes used port and message receiving thread. 
		/// </summary>
		public virtual void Kill()
		{
			LoggingUtilities.LogFormat("Killing UDPMaster");

			SendingSocket.Close();

			Receiver.Close();
			ReceiverThread.Abort();
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
			SendingSocket.SendTo(message, SendingEndPoint);
		}
		/// <summary>
		///		Updates the current messaging target, and sends 
		///		a message across the network.
		/// </summary>
		public void SendMessage(byte[] message, string targetIP)
		{
			UpdateTargetIP(targetIP);
			SendMessage(message);
		}


		/// <summary>
		///		Creates a new thread dedicated to message receiving.
		/// </summary>
		protected void ReceiveMessages()
		{
			ReceiverThread = new Thread(new ThreadStart(delegate 
			{
				while (true)
				{
					try
					{
						// TODO: This throws an error when killing the network connection.
						byte[] messageByteArray = Receiver.Receive(ref ReceivingEndPoint);
						DistributeMessage(messageByteArray);
					}
					catch (System.Exception ex)
					{
						LoggingUtilities.LogFormat(
							"Message: ({0})\nInner Exception: ({1})\nData: ({2})\nHelplink: ({3})\nHResult: ({4})\nSource: ({5})\nTargetSite: ({6})\nStack Trace: ({7})", 
							ex.Message, 
							ex.InnerException, 
							ex.Data,
							ex.HelpLink,
							ex.HResult,
							ex.Source,
							ex.TargetSite,
							ex.StackTrace); 
					}
				}
			}));

			ReceiverThread.Start();
		}
		/// <summary>
		///		Calls the networkListeners.
		/// </summary>
		protected virtual void DistributeMessage(byte[] message)
		{
			foreach (INetworkListener networkListener in NetworkListeners)
			{
				networkListener.OnMessageReceived(message);
			}

			if (LogReceivedMessages && UnityEngine.Application.isEditor)
			{
				LoggingUtilities.LogFormat(
					"Received message (\"{0}\") from ip ({1}) using port ({2})",
					message.ToString(),
					ReceivingEndPoint.Address.ToString(),
					ReceivingPort);
			}
		}


		/// <summary>
		///		Adds INetworkListener to the list of objects that 
		///		is invoked once a message is received.
		/// </summary>
		public void AddListener(INetworkListener listener)
		{
			NetworkListeners.SafeAdd(listener);
		}
		/// <summary>
		///		Removes INetworkListener from the list of objects
		///		that is invoked once a message is received.
		/// </summary>
		public void RemoveListener(INetworkListener listener)
		{
			NetworkListeners.SafeRemove(listener);
		}

		/// <summary>
		///		Updates the targetIP to the 
		///		provided target.
		/// </summary>
		public void UpdateTargetIP(string ipAddress)
		{
			SendingEndPoint.Address = IPAddress.Parse(ipAddress);
			LoggingUtilities.LogFormat("Updating target IP to: {0}", ipAddress);
		}
	}
}
