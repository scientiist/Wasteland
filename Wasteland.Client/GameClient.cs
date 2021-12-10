using System.Collections.Generic;
using Microsoft.VisualBasic;
using System;
using Microsoft.Xna.Framework;
using Wasteland.Client.Network;
using Wasteland.Common.Network;
using Conarium.Extension;
using Conarium.Services;
using System.Linq;
using Wasteland.Common;
using System.Reflection;
using Wasteland.Common.Game;
using Wasteland.Common.Entities;
using Conarium.Datatypes;

namespace Wasteland.Client
{

    public enum ConnectionStatus
    {
        Disconnected,
        Connecting,
		ConnectionRefused,
        Connected,
        Kicked,

    }
    public class GameClient : Context
    {

		public float ConnectionTimeout;
		public string ConnectionStateMessage {get;set;}
        public bool IsConnected {get; set;}
        public ConnectionStatus Status {get;set;}

        public ClientSubsystem NetworkManager {get;private set;}

		public Gameworld World {get;set;}

		public delegate void NetworkListener(NetworkMessage message);

		Dictionary<PacketType, NetworkListener> NetworkEvents;

        public GameClient(IGameClient client) : base(client) 
        {
			NetworkEvents = new Dictionary<PacketType, NetworkListener>()
			{
				[PacketType.AcceptConnectRequest] = OnConnectApproved,
				[PacketType.RejectConnectRequest] = OnConnectRejected,
				[PacketType.SpawnEntity] = OnSpawnEntity,
				[PacketType.DespawnEntity] = OnDespawnEntity,
				[PacketType.UpdateEntityPhysicsState] = OnEntityPhysicsUpdated,
				[PacketType.EntityAction] = OnEntityAction,
			};


			World = new Gameworld();
			World.Entities.Add(new Vehicle(Guid.Empty) {
				Position = new Vector2(600, 500),
				Direction = Rotation.RightAngle,
				FrontAxleCenterOffset = 20,
				FrontAxleLength = 10,
				WheelDirection = 20f,
				MaxTurnRadius = 45f,
			});
        }


		#region Network Methods
		void OnConnectApproved(NetworkMessage msg)
		{
			var packet = new AcceptConnectRequestPacket(msg.Packet.GetBytes());

			Status = ConnectionStatus.Connected;
		}

		void OnConnectRejected(NetworkMessage msg)
		{
			var packet = new RejectConnectRequestPacket(msg.Packet.GetBytes());


			Status = ConnectionStatus.ConnectionRefused;


		}

		void OnSpawnEntity(NetworkMessage msg)
		{
			var packet = new SpawnEntityPacket(msg.Packet.GetBytes());

			Entity spawned;
			switch(packet.EntityType)
			{
				case EntityType.PeerPlayer:
					spawned = new PeerPlayer(packet.AssignedNetworkID);
					break;
				case EntityType.Player:
					spawned = new LocalPlayer(packet.AssignedNetworkID);
					break;
				default:
					break;
			}

			
		}

		void OnDespawnEntity(NetworkMessage msg) { }
		void OnEntityPhysicsUpdated(NetworkMessage msg) { }
		void OnEntityAction(NetworkMessage msg) { }


        public void Connect(string address)
        {
			GameConsole.Get().Log($"Client starting: connecting to {address}");
			NetworkManager = new ClientSubsystem(address);
			NetworkManager.Start();
            NetworkManager.SendPacket(new QueryServerPacket(Wasteland.Common.Constants.NetworkProtocolVersion));
			Status = ConnectionStatus.Connecting;

			ConnectionTimeout = 10;
            //NetworkManager.Listen(PacketType.InterrogationResponse, OnServerRespondsToInterrogation);
        }

		public virtual void Disconnect() { }

		private void OnServerRespondsToInterrogation(NetworkMessage data)
		{
			var packet = new QueryResponsePacket(data.Packet.GetBytes());

			if (packet.RequiresPassword)
			{
				// FIXME: DO NOT ACTUALLY SEND PLAINTEXT PASSWORD, THIS IS JUST A TEST;
				NetworkManager.SendPacket(new RequestConnectPacket("username", "assword", Wasteland.Common.Constants.NetworkProtocolVersion));
		
			}

		}
		#endregion

		public override void Update(GameTime gt)
		{
			NetworkManager.Update(gt);
			base.Update(gt);

			World.Update(gt);
			angle += gt.GetDelta();


			if (Status == ConnectionStatus.Connecting)
			{
				ConnectionTimeout -= gt.GetDelta();
				if (ConnectionTimeout <= 0)
				{
					Status = ConnectionStatus.ConnectionRefused;
					ConnectionStateMessage = "Failed to connect: Server did not respond. (Likely offline)";
				}
			}
		}

		float angle = 0;
		public override void Draw()
		{
			
			var gfx = GraphicsService.Get();
			gfx.Begin();

			gfx.Line(Color.Red, new Vector2(100, 100), new Vector2(200, 200), 2);
			gfx.Line(Color.Green, new Vector2(100, 100), 32, new Rotation(angle), 2);
			gfx.Text($"{new Rotation(angle).Degrees%360f}", new Vector2(100, 80));
			if (Status == ConnectionStatus.ConnectionRefused)
			{

			}

			World.Draw();
			gfx.End();
		}

        
    }
}
