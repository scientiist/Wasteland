using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using Conarium.Generic;
using Wasteland.Common.Network;

namespace Wasteland.Client.Network
{
    public class ClientSubsystem: SharedNetworkSubsystem
    {
        public override string DeviceName => "NetworkClient";

        public string ServerIPAddress { get; private set; }
		public int ServerPort { get; private set; }


		// parses shit like "localhost"
		public static string ParseHostnameShortcuts(string hostname)
        {
			hostname = hostname.Trim();
			if (hostname.Contains("localhost"))
				hostname = hostname.Replace("localhost", "127.0.0.1");

			if (!hostname.Contains(":"))
				hostname += ":"+42069;

			return hostname;
		}

		public static IPEndPoint GetEndPointFromAddress(string hostname)
        {
			bool success = IPEndPoint.TryParse(ParseHostnameShortcuts(hostname), out IPEndPoint output);
			if (success)
				return output;
			throw new Exception($"Invalid IP Endpoint! {hostname}, {ParseHostnameShortcuts(hostname)}");
		}

		public IPEndPoint ServerAddress { get; private set; }

		public ClientSubsystem(string hostname) : base()
		{
			running = new ThreadSafeValue<bool>(false);
			var endpoint = GetEndPointFromAddress(hostname);// = ParseHostnameShortcuts(hostname);

			ServerAddress = endpoint;

			IncomingMessages = new ConcurrentQueue<NetworkMessage>();
            OutgoingMessages = new ConcurrentQueue<OutgoingPayload>();
            ServerPort = endpoint.Port;
			ServerIPAddress = endpoint.Address.ToString();

			UdpSocket = new UdpClient(ServerIPAddress, ServerPort);



		}
		public void SendPacket(Packet packet) => Send(packet);

    }
}
