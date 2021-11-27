using System;
using System.Net;
namespace Wasteland.Common.Network
{
    public class NetworkMessage
    {
        public IPEndPoint Sender {get;set;}
        public Packet Packet {get;set;}
        public DateTime ReceiveTime {get;set;}
    }
}