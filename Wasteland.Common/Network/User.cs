using System.Net;
using System;

namespace Wasteland.Common.Network
{
    public class User
    {
        public IPEndPoint EndPoint {get;set;}
        public string Username {get;set;}
        public uint UserNetworkID {get;set;}
        public float KeepAlive {get;set;}


        public bool Kicked {get;set;}
        public string DisconnectReason {get; private set;}


        
    }
}
