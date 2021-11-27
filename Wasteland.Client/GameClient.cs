using System;
using Wasteland.Client.Network;
using Wasteland.Common.Network;

namespace Wasteland.Client
{

    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
        Connected,
        Kicked,

    }
    public class GameClient
    {

		public string ConnectionStateMessage {get;set;}
        public const int ClientNetworkProtocolVersion = 1;
        public bool IsConnected {get; set;}
        public ConnectionStatus Status {get;set;}

        public ClientSubsystem NetworkManager {get;private set;}

        public GameClient() 
        {

        }


        public void Connect(string address)
        {
			NetworkManager = new ClientSubsystem(address);
            NetworkManager.SendPacket(new InterrogateServerPacket(ClientNetworkProtocolVersion));
            NetworkManager.Listen(PacketType.InterrogationResponse, OnServerRespondsToInterrogation);
        }


		private void OnServerRespondsToInterrogation(NetworkMessage data)
		{
			var packet = new InterrogationResponsePacket(data.Packet.GetBytes());

			if (packet.RequiresPassword)
			{
				// FIXME: DO NOT ACTUALLY SEND PLAINTEXT PASSWORD, THIS IS JUST A TEST;
				NetworkManager.SendPacket(new RequestJoinServerPacket("username", "assword"));
			}

		}

        public virtual void Disconnect()
        {

        }
    }
}
