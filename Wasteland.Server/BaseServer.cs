using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Conarium;
using Microsoft.Xna.Framework;
using Wasteland.Common.Network;
using Wasteland.Common;

namespace Wasteland.Server
{
	public class BaseServer : ICommandSender, ICommandReciever
	{
		#region Config File props
		public int Port {get; set;} = 42069;
		public int MaxPlayers {get;set;} = 20;
		public int TickRateMS {get;set;} = 20; // 1000/20 = 40 ticks/second

		public bool RequiresPassword {get;set;} = false;


		#endregion
		
		public IServerInput Input {get; set;}
		public IServerOutput Logger {get; set;}
		public bool Running {get;set;}
		public ServerSubsystem NetworkManager {get;private set;}
		public List<User> ConnectedUsers {get;private set;}
		// ICommandSender Properties
		public Color SenderColor => Color.White;
		public string Name => "Wasteland Server";
		//--------------


		// Networking events and such
		public delegate void NetworkListener(NetworkMessage message);
		private Dictionary<PacketType, NetworkListener> NetworkEvents;
		
		// Commands
		public List<Command> Commands {get; set;}
		public void BindCommand(Command command) =>  Commands.Add(command);

		public void ProcessCommandString(ICommandSender sender, string input)
		{
			var tokens = input.Split(' ');
			var keyword = tokens[0];
			var arguments = tokens.Skip(1).ToArray();
			foreach(var command in Commands)
			{
				if (command.Keyword.ToLower() == keyword.ToLower())
				{ // got match
					command.RunCommand(sender, arguments);
					return;
				}
			}
		}

		#region Command-Related Methods
		void SayHelp(CommandEventArgs e)
		{

			if (e.Args.Length > 0) // has command
			{
				var requestedCommand = e.Args[0];
				// TODO: print help information for command
				return;
			}
			Logger?.Log("Available Commands:",  ConsoleColor.DarkGreen, ConsoleColor.White, false);
			foreach(var command in Commands)
				Logger?.Log($"{command.Keyword} {command.Description}", ConsoleColor.DarkGreen, ConsoleColor.White, false);
		}

		void ExitServer(CommandEventArgs e)
		{
			Console.ResetColor();
			Console.Clear();
			Environment.Exit(0);

		}

		#endregion

		public BaseServer()
		{
			Commands = new List<Command>();
			Running = true;
			ConnectedUsers = new List<User>();
			
			#region Built-in Command Definitions
			BindCommand(new Command("quit")
			{
				Aliases = new List<string>{"q", "exit", "killserver"},
				Description = "stops the server",
				Arguments = new List<string>{},
				Callback = ExitServer,
			});

			BindCommand(new Command("help")
			{
				Aliases = new List<string>{"wtf", "commands", "list"},
				Description = "lists avaliable commands and arguments.",
				Arguments = new List<string>{"<command_name>"},
				Callback=SayHelp
			});
			#endregion


			NetworkEvents = new Dictionary<PacketType, NetworkListener>()
			{
				[PacketType.QueryServer] = OnServerQuery,
				[PacketType.RequestConnect] = OnConnectRequest,
				
			};
		}

		// server information requested by potential client
		void OnServerQuery(NetworkMessage message)
		{
			QueryServerPacket packet = new QueryServerPacket(message.Packet.GetBytes());

			ProtocolStatus versionComparison = ProtocolStatus.Match;
			bool isCompat = false;

			// protocol match
			if (packet.ClientProtocolCode == Wasteland.Common.Constants.NetworkProtocolVersion)
			{
				versionComparison = ProtocolStatus.Match;
				isCompat = true;
			
			} // server out of date
			else if (packet.ClientProtocolCode > Wasteland.Common.Constants.NetworkProtocolVersion)
			{
				versionComparison = ProtocolStatus.ServerOutOfDate;
			} // client out of date
			else if (packet.ClientProtocolCode < Wasteland.Common.Constants.NetworkProtocolVersion)
			{
				versionComparison = ProtocolStatus.ClientOutOfDate;
			}

			var response = new QueryResponsePacket()
			{
				Status = versionComparison,
				ProtocolCompatible = isCompat,
				RequiresPassword = RequiresPassword,
				MaxPlayers = MaxPlayers,
				CurrentPlayerCount = ConnectedUsers.Count,
			};
		}

		void OnConnectRequest(NetworkMessage message)
		{
			RequestConnectPacket packet = new RequestConnectPacket(message.Packet.GetBytes());

			// TODO: sanity check inputs and confirm server can take player
			// TODO: password check

			bool success = true;


			if (success)
			{
				User newClientPeer = new User();

				ConnectedUsers.Add(newClientPeer);


				AcceptConnectRequestPacket accept = new AcceptConnectRequestPacket();


				NetworkManager.SendPacket(accept, message.Sender);


			}
			else
			{
				RejectConnectRequestPacket reject = new RejectConnectRequestPacket("fuck you that's why");

				NetworkManager.SendPacket(reject, message.Sender);
			}
		}

		public virtual void Start()
		{
			NetworkManager = new ServerSubsystem(Port);
			NetworkManager.Start();
			Logger?.Log("Wasteland Server Initialized...");
			Task.Run(GameserverThreadLoop);
		}


		public virtual void Update(GameTime gt)
		{
			NetworkManager.Update(gt);
			ReadIncomingPackets();
		}

		public void GameserverThreadLoop()
		{
			// Manage timings and run updates;
			Stopwatch timing = new Stopwatch();
			TimeSpan runTotal = new TimeSpan();

			while (Running)
			{
				timing.Stop();
				runTotal += timing.Elapsed;
				GameTime gt = new GameTime(runTotal, timing.Elapsed);
				timing.Reset();
				timing.Start();
				Update(gt);
				Thread.Sleep(TickRateMS);
			}
		}
		private void ReadIncomingPackets()
		{
			while (NetworkManager.HaveIncomingMessage())
			{
				NetworkMessage message = NetworkManager.GetLatestMessage();
				Logger.Log("Gota fuckin MESSAGE");
				foreach(var ev in NetworkEvents)
				{
					if (ev.Key == message.Packet.Type)
					{
						ev.Value?.Invoke(message);
					}
				}
			}
		}


		public virtual void Shutdown() { }
	}
}
