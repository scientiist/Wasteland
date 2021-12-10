using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

namespace Wasteland.Common.Network
{
    public static class TypeSizesInBytes
    {
        public readonly static int Int32 = 4;
        public readonly static int Float = 4;
        public readonly static int Double = 8;
        public readonly static int Char = 1;
    }
    

    public enum PacketType : uint {
        QueryServer, 
		QueryResponse,
		RequestConnect, 
		AcceptConnectRequest, 
		RejectConnectRequest,
		Disconnect,
		SpawnEntity,
		DespawnEntity,
		UpdateEntityPhysicsState,
		EntityAction,
    }

    public class Packet
    {
		protected static int GetUTF8ByteLength(string s) => Encoding.UTF8.GetByteCount(s);

        public PacketType Type;
        public long Timestamp;
        public byte[] Payload = new byte[0];

        public Packet(PacketType type)
        {
            Type = type;
            Timestamp = DateTime.Now.Ticks;
        }

        public Packet(byte[] bytes)
        {
            // start peeling out the data from the byte array
            int i = 0;
            Type = (PacketType)BitConverter.ToUInt32(bytes, 0);

            i += sizeof(PacketType);

            Timestamp = BitConverter.ToInt64(bytes, i);

            i += sizeof(long);

            Payload = bytes.Skip(i).ToArray();
        }

        public byte[] GetBytes()
        {
            int ptSize = sizeof(PacketType);
            int tsSize = sizeof(long); // size of long timestamp

            int i = 0;
            byte[] bytes = new byte[ptSize + tsSize + Payload.Length];


			BitConverter.GetBytes((uint)this.Type).CopyTo(bytes, i);
			i += ptSize;

            BitConverter.GetBytes(Timestamp).CopyTo(bytes, i);
            i += tsSize;

            Payload.CopyTo(bytes, i);
            i += Payload.Length;


            return bytes;
        }

        public override string ToString() => Payload.ToHexString(0, Payload.Length);

        // Send packet to specific endpoint
        public void Send(UdpClient client, IPEndPoint reciever)
        {
            byte[] bytes = GetBytes();
            client.Send(bytes, bytes.Length,  reciever);
        }

        // Send packet to default endpoint (Errors if not pre-set)
        public void Send(UdpClient client)
        {
            byte[] bytes = GetBytes();
            client.Send(bytes, bytes.Length);
        }

		
    }

    public class QueryServerPacket : Packet
    {
        public int ClientProtocolCode { // used to match versions between server and client
            get => Payload.ReadInt(0);
            set => Payload.WriteInt(0, value);
        }
        public QueryServerPacket(byte[] data) : base(data) {}
        public QueryServerPacket(int protocolID) : base(PacketType.QueryServer)
        {
            Payload = new byte[16];
            ClientProtocolCode = protocolID;
        }
    }


	public enum ProtocolStatus
	{
		Match,
		ClientOutOfDate,
		ServerOutOfDate,
	}

    public class QueryResponsePacket : Packet
    {
		public bool ProtocolCompatible 
		{
			get => Payload[0].Get(0);
			set => Payload[0].Set(0, value);
		}
		public ProtocolStatus Status {get;set;}

		public int MaxPlayers {get;set;}
		public int CurrentPlayerCount {get;set;}

		public bool RequiresPassword {get;set;}
		public string ServerName {get;set;}

        public QueryResponsePacket() : base(PacketType.QueryResponse)
        {
			Payload = new byte[256];
        }

        public QueryResponsePacket(byte[] bytes) : base(bytes)
        {
        }
    }

    public class RequestConnectPacket : Packet
    {

		public int UsernameLengthBytes
        {
            get => Payload.ReadInt(0);
			set => Payload.WriteInt(0, value);
        }
        public string Username
        {
            get => Encoding.UTF8.GetString(Payload, 4, UsernameLengthBytes);
            set {
                UsernameLengthBytes = GetUTF8ByteLength(value);
                Encoding.UTF8.GetBytes(value).CopyTo(Payload, 4);
            }
        }

		public int PasswordLengthBytes
        {
            get => Payload.ReadInt(128);
			set => Payload.WriteInt(128, value);
        }
        public string Password
        {
            get => Encoding.UTF8.GetString(Payload, 132, PasswordLengthBytes);
            set {
                PasswordLengthBytes = GetUTF8ByteLength(value);
                Encoding.UTF8.GetBytes(value).CopyTo(Payload, 132);
            }
        }

		public int NetworkProtocolVersion 
		{
			get => Payload.ReadInt(256);
			set => Payload.WriteInt(256, value);
		}

        public RequestConnectPacket(string user, string pass, int protocol) : base(PacketType.RequestConnect)
        {
			Payload = new byte[1024];
			int lengthOfTokens = GetUTF8ByteLength(user) + GetUTF8ByteLength(pass);
			
			Username = user;
			Password = pass;
			NetworkProtocolVersion = protocol; // protocol is also compared in the QueryServer sequence
			// but it is always possible for the client to force a connection packet
			// without successful querying, so we should handle it
        }

        public RequestConnectPacket(byte[] bytes) : base(bytes) { }
    }

    public class AcceptConnectRequestPacket : Packet
    {
        public AcceptConnectRequestPacket() : base(PacketType.AcceptConnectRequest)
        {
        }

        public AcceptConnectRequestPacket(byte[] bytes) : base(bytes)
        {
        }
    }

    public class RejectConnectRequestPacket : Packet
    {
		public int RejectReasonLengthBytes
        {
            get => Payload.ReadInt(0);
			set => Payload.WriteInt(0, value);
        }
        public string RejectReason
        {
            get => Encoding.UTF8.GetString(Payload, 4, RejectReasonLengthBytes);
            set {
                RejectReasonLengthBytes = GetUTF8ByteLength(value);
                Encoding.UTF8.GetBytes(value).CopyTo(Payload, 4);
            }
        }

        public RejectConnectRequestPacket(string reason) : base(PacketType.RejectConnectRequest)
        {
			RejectReason = reason;
        }

        public RejectConnectRequestPacket(byte[] bytes) : base(bytes) { }
    }

    public class NotifyDisconnectPacket : Packet
    {
        public NotifyDisconnectPacket() : base(PacketType.Disconnect)
        {

        }

        public NotifyDisconnectPacket(byte[] bytes) : base(bytes)
        {

        }
    }

	

	public class SpawnEntityPacket : Packet
	{
		public EntityType EntityType
		{
			get {return EntityType.PeerPlayer;}
			set {}
		}

		public Guid AssignedNetworkID
		{
			get => new Guid(Payload.ReadString(2, 36, Encoding.ASCII));
			set => Payload.WriteString(2, value.ToString(), Encoding.ASCII, 36);
		}

		public Vector2 SpawnPosition
		{
			get => Payload.ReadVector2(40);
			set => Payload.WriteVector2(40, value);
		}

		public SpawnEntityPacket(EntityType type, Guid assignedNetworkID, Vector2 spawnPos) : base(PacketType.SpawnEntity)
		{
			Payload = new byte[64];
			EntityType = type;
			AssignedNetworkID = assignedNetworkID;
			SpawnPosition = spawnPos;
		}

		public SpawnEntityPacket(byte[] data) : base(data) {}
	}

	public class DespawnEntityPacket : Packet
	{
		public DespawnEntityPacket() : base(PacketType.DespawnEntity)
		{
		}
	}

	public class UpdateEntityPhysicsStatePacket : Packet
	{
		public UpdateEntityPhysicsStatePacket() : base(PacketType.UpdateEntityPhysicsState)
		{
			
		}
	}

	public class EntityActionPacket : Packet
	{
		public EntityActionPacket() : base(PacketType.EntityAction)
		{
		}
	}


	public class DownloadMapPacket : Packet
	{
		public DownloadMapPacket(PacketType type) : base(type)
		{
		}
	}



}