using System.Net;
using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using Wasteland.Common.Network;

namespace Wasteland.Server
{
    public class ServerSubsystem : SharedNetworkSubsystem
    {
        public override string DeviceName => "NetworkServer";

        public ServerSubsystem(int port) : base()
        {
            IncomingMessages = new ConcurrentQueue<NetworkMessage>();
            OutgoingMessages = new ConcurrentQueue<OutgoingPayload>();
            running = new Conarium.Generic.ThreadSafeValue<bool>(false);
            UdpSocket = new UdpClient(port, AddressFamily.InterNetwork);
        }

        public void SendPacket(Packet packet, IPEndPoint target) => Send(packet, target);
    }
}
