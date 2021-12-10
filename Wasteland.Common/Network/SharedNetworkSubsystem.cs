using System.Collections.Generic;
using System;
using System.Collections.Concurrent;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Conarium.Extension;
using Conarium.Generic;
using Microsoft.Xna.Framework;
using System.Linq;

namespace Wasteland.Common.Network
{

    public class OutgoingPayload
    {
        public Packet Payload { get; set; }
        public IPEndPoint TargetAddress { get; set; } 
    }
    
    public class SharedNetworkSubsystem
    {
         public const int NAP_TIME_MILLISECONDS = 1;
        public const int SIO_UDP_CONNRESET = -1744830452;
        public const int PROTOCOL_VERSION = 1;


        protected ThreadSafeValue<bool> running { get; set; }
        protected ConcurrentQueue<NetworkMessage> IncomingMessages { get; set; }
        protected ConcurrentQueue<OutgoingPayload> OutgoingMessages { get; set; }

        public bool IsRunning => running.Value;

        public virtual string DeviceName { get; }

        public virtual DateTime LatestReceiveTimestamp { get; protected set; }
        public virtual DateTime LatestSendTimestamp { get; protected set; }
        public virtual int PacketsReceived { get; protected set; }
        public virtual int PacketsSent { get; protected set; }
        public virtual int TotalBytesSent { get; protected set; }
        public virtual int TotalBytesReceived { get; protected set; }
        public virtual int BytesSentPerSecond { get; protected set; }
        public virtual int BytesReceivedPerSecond { get; protected set; }

        //public virtual IMessageOutlet Output { get; set; }

        public virtual int Port { get; protected set; }

        protected int InternalReceiveCount { get; set; }
        protected int InternalSendCount { get; set; }

        private float counter = 0;

        protected UdpClient UdpSocket { get; set; }

        protected void IOControlFixICMPBullshit()
        {
            UdpSocket.Client.IOControl(
                (IOControlCode)SIO_UDP_CONNRESET,
                new byte[] { 0, 0, 0, 0 },
                null
            );
        }

        private void ResetByteCounters()
        {
            BytesSentPerSecond = InternalSendCount;
            BytesReceivedPerSecond = InternalReceiveCount;
            InternalSendCount = 0;
            InternalReceiveCount = 0;
            counter = 0;
        }

        public SharedNetworkSubsystem()
        {
            IncomingMessages = new ConcurrentQueue<NetworkMessage>();
            OutgoingMessages = new ConcurrentQueue<OutgoingPayload>();
           // IOControlFixICMPBullshit();

        }

        // do this ONCE because reflection is quite expensive.


        public virtual void Update(GameTime gt)
        {
            counter += gt.GetDelta();
            if (counter > (1.0f))
                ResetByteCounters();
        }

        public bool HaveIncomingMessage() => !IncomingMessages.IsEmpty;

        public NetworkMessage GetLatestMessage()
        {
            NetworkMessage msg;
            bool success = IncomingMessages.TryDequeue(out msg);

            if (success)
                return msg;
            throw new Exception("No Message Queued! Used HaveIncomingMessage() to check!");
        }

        protected virtual void ReadIncomingPackets()
        {
            bool canRead = UdpSocket.Available > 0;
            if (canRead)
            {

                IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
                byte[] data = UdpSocket.Receive(ref ep);

                NetworkMessage nm = new NetworkMessage();
                nm.Sender = ep;
                nm.Packet = new Packet(data);
                nm.ReceiveTime = DateTime.Now;

				Console.WriteLine($"{DeviceName} Recieved {nm.Packet.Type} from {nm.Sender} at {nm.ReceiveTime}: {nm.Packet.GetBytes().ToHexString()} ");

                IncomingMessages.Enqueue(nm);
                PacketsReceived++;
                TotalBytesReceived += nm.Packet.Payload.Length;
                InternalReceiveCount += nm.Packet.Payload.Length;
                LatestReceiveTimestamp = DateTime.Now;
            }
        }

        private void FlushOutgoingPackets()
        {
            int outQCount = OutgoingMessages.Count;

            // write out queued messages
            for (int i = 0; i < outQCount; i++)
            {
                OutgoingPayload packet;
                bool have = OutgoingMessages.TryDequeue(out packet);

                if (have)
                {
					Console.WriteLine($"{DeviceName} Sending {packet.Payload.Type} to {packet.TargetAddress}: {packet.Payload.GetBytes().ToHexString()}  ");
                    if (packet.TargetAddress == null)
                        packet.Payload.Send(UdpSocket);    
                    else
                        packet.Payload.Send(UdpSocket, packet.TargetAddress);

                    PacketsSent++;
                    TotalBytesSent += packet.Payload.Payload.Length;
                    InternalSendCount += packet.Payload.Payload.Length;
                }
            }
        }

        private void NetworkThreadLoop()
        {
            Console.WriteLine($"{DeviceName} thread started on {UdpSocket.Client.LocalEndPoint} {running.Value}");
            while (running.Value)
            {
                bool canRead = UdpSocket.Available > 0;
                int outgoingMessageQueueCount = OutgoingMessages.Count;
                
                ReadIncomingPackets();
                FlushOutgoingPackets();

                // if nothing happened, take a nap
                if (!canRead && (outgoingMessageQueueCount == 0))
                    Thread.Sleep(NAP_TIME_MILLISECONDS);
            }

            Console.WriteLine($"{DeviceName} thread finished, cleaning resources...");
            UdpSocket.Close();
            //UdpSocket.Dispose();

        }

        public void Start()
        {
            running.Value = true;
            Task.Factory.StartNew(NetworkThreadLoop);
        }

        public void Close()
        {
            running.Value = false;
        }

        protected void Send(Packet packet, IPEndPoint target)
        {

            OutgoingMessages.Enqueue(new OutgoingPayload { 
                Payload = packet,  
                TargetAddress = target,
            });
            LatestSendTimestamp = DateTime.Now;
        }

        protected void Send(Packet packet)
        {
            OutgoingMessages.Enqueue(new OutgoingPayload { Payload = packet });
            LatestSendTimestamp = DateTime.Now;
        }
    }
}