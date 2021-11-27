using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using Conarium;
using Microsoft.Xna.Framework;
using Wasteland.Common.Network;

namespace Wasteland.Server
{
	public interface IServerOutput
	{
		void Log(string text);
		void Log(string text, ConsoleColor fg, ConsoleColor bg, bool timestamp);
	}
	public interface IServerInput
	{
		void SendCommand(string command);
	}


	public class BaseServer : ICommandSender, ICommandReciever
	{ 
		public int Port {get; set;} = 42069;
		public int MaxPlayers {get;set;} = 20;
		public int TickRateMS {get;set;} = 20; // 1000/20 = 40 ticks/second

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
		private static Dictionary<PacketType, NetworkListener> NetworkEvents = new Dictionary<PacketType, NetworkListener>()
		{
			[PacketType.Acknowledge] = (msg) => {},
			[PacketType.Ignore] = (msg) => {},
		};
		
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
			Logger.Log("Available Commands:",  ConsoleColor.DarkGreen, ConsoleColor.White, false);
			foreach(var command in Commands)
				Logger.Log($"{command.Keyword} {command.Description}", ConsoleColor.DarkGreen, ConsoleColor.White, false);
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
		}

		public virtual void Start()
		{
			NetworkManager = new ServerSubsystem(Port);
			Logger.Log("Wasteland Server Initialized...");
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
				foreach(var ev in NetworkEvents)
				{

				}
			}
		}


		public virtual void Shutdown() { }
	}
}
