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
    

    public enum PacketType {
        InterrogateServer,
        InterrogationResponse,
		RequestJoin, AcceptJoinRequest, RejectJoinRequest,
        Acknowledge, Ignore }


    public static class MonoGameByteArrayExtensions // serialize monogame types
    {
		/// <summary>
		/// Data Length = 8 (2*float)
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <returns></returns>
		public static Vector2 ReadVector2(this byte[] data, int index)=> new Vector2(data.ReadFloat(index), data.ReadFloat(index + 4));
		/// <summary>
		/// Data Length = 8 (2*float)
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
        public static void WriteVector2(this byte[] data, int index, Vector2 value)
        {
			data.WriteFloat(index, value.X);
			data.WriteFloat(index + 4, value.Y);
        }
		public static Color ReadColorRGBA(this byte[] data, int index) => new Color(data[index], data[index + 1], data[index + 2], data[index+3]);
		/// <summary>
		/// Data Length = 4 (4*byte)
		/// </summary>
		/// <param name="data"></param>
		/// <param name="index"></param>
		/// <param name="value"></param>
		public static void WriteColorRGBA(this byte[] data, int index, Color value)
        {
			data[index] = value.R;
			data[index + 1] = value.G;
			data[index + 2] = value.B;
			data[index + 3] = value.A;


			byte[] somedata = new byte[20];
        }
    }

    public class PacketSchemaAttribute : Attribute
    {
        public PacketType Type;
        public PacketSchemaAttribute(PacketType type)
        {
            Type = type;
        }
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

            BitConverter.GetBytes(Timestamp).CopyTo(bytes, i);
            i += tsSize;

            Payload.CopyTo(bytes, i);
            i += Payload.Length;


            return bytes;
        }

        public override string ToString() { return ""; }

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


    [PacketSchema(PacketType.Acknowledge)]
    public class MessagePacket : Packet
    {
		
        public int MessageLengthBytes
        {
            get => Payload.ReadInt(0);
			set => Payload.WriteInt(0, value);
        }
        public string Message
        {
            get => Encoding.UTF8.GetString(Payload, 4, MessageLengthBytes);
            set {
                MessageLengthBytes = GetUTF8ByteLength(value);
                Encoding.UTF8.GetBytes(value).CopyTo(Payload, 4);
            }
        }

        


		public MessagePacket(byte[] data) : base(data) {}
        public MessagePacket(string message) : base(PacketType.Acknowledge)
        {
            Payload = new byte[16+GetUTF8ByteLength(message)];
            Message = message;
            //MessageLengthBytes = Message.Length;
        }
    }

    [PacketSchema(PacketType.InterrogateServer)]
    public class InterrogateServerPacket : Packet
    {

        public int ClientProtocolCode { // used to match versions between server and client
            get => Payload.ReadInt(0);
            set => Payload.WriteInt(0, value);
        }
        public InterrogateServerPacket(byte[] data) : base(data) {}
        public InterrogateServerPacket(int protocolID) : base(PacketType.InterrogateServer)
        {
            Payload = new byte[16];
            ClientProtocolCode = protocolID;
        }
    }


    public class InterrogationResponsePacket : Packet
    {
		public bool ProtocolCompatible {get;set;}
		public int MaxPlayers {get;set;}
		public int CurrentPlayerCount {get;set;}

		public bool RequiresPassword {get;set;}

        public InterrogationResponsePacket() : base(PacketType.InterrogationResponse)
        {
        }

        public InterrogationResponsePacket(byte[] bytes) : base(bytes)
        {
        }
    }

    public class RequestJoinServerPacket : Packet
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
            get => Payload.ReadInt(4+UsernameLengthBytes);
			set => Payload.WriteInt(4+UsernameLengthBytes, value);
        }
        public string Password
        {
            get => Encoding.UTF8.GetString(Payload, 4+UsernameLengthBytes, PasswordLengthBytes);
            set {
                UsernameLengthBytes = GetUTF8ByteLength(value);
                Encoding.UTF8.GetBytes(value).CopyTo(Payload, 4);
            }
        }

        public RequestJoinServerPacket(string user, string pass) : base(PacketType.RequestJoin)
        {
			int lengthOfTokens = GetUTF8ByteLength(user) + GetUTF8ByteLength(pass);
			Payload = new byte[8+lengthOfTokens];
			Username = user;
			Password = pass;
        }

        public RequestJoinServerPacket(byte[] bytes) : base(bytes) { }
    }

    public class AcceptJoinRequestPacket : Packet
    {
        public AcceptJoinRequestPacket() : base(PacketType.AcceptJoinRequest)
        {
        }

        public AcceptJoinRequestPacket(byte[] bytes) : base(bytes)
        {
        }
    }
    public class RejectJoinRequestPacket : Packet
    {
        public RejectJoinRequestPacket() : base(PacketType.RejectJoinRequest)
        {
        }

        public RejectJoinRequestPacket(byte[] bytes) : base(bytes)
        {
        }
    }

    public class NotifyDisconnectPacket : Packet
    {
        public NotifyDisconnectPacket() : base(PacketType.Ignore)
        {
        }

        public NotifyDisconnectPacket(byte[] bytes) : base(bytes)
        {
        }
    }
}