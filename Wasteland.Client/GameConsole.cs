using System.Windows.Input;
using System;
using System.Collections.Generic;
using Conarium;
using Conarium.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;
using Wasteland.Common;

namespace Wasteland.Client 
{

    

    public class GameConsole : GameComponent, ICommandReciever, IServerOutput, ICommandSender
    {
		public static GameConsole Instance {get; private set;}
		public static GameConsole Get() => Instance;
        public Conarium.UI.WindowNode ConsoleWindow {get;set;}

		RectNode MessageHistoryBox;
        public bool Open {get; set;}

        

        List<Command> Commands;

        public void BindCommand(Command command) => Commands.Add(command);

		public List<Message> MessageHistory {get; private set;}

        public List<string> CommandHistory {get; set;}

        TextInputNode Input {get;set;}

		public Color SenderColor => Color.Gray;

		public float Scroll {get;set;}

		public string Name => ">";

		public GameConsole(Game game) : base(game)
        {
			Instance = this;

            ConsoleWindow = new WindowNode("GameConsole", 600, 200);
			ConsoleWindow.CloseButton.OnPressed += (s,e) => this.Open = !this.Open;
			Commands = new List<Command>();

			Commands.Add(new Command("quit", (ev)=>Environment.Exit(0)));
			Commands.Add(new Command("help", HelpCommand));

			CommandHistory = new List<string>();
			MessageHistory = new List<Message>();
			MessageHistory.Add(new Message("Initializing..."));

            MessageHistoryBox = new RectNode("history")
            {
                Parent = ConsoleWindow,
                Position = new UIVector(0,20,0,0),
                Size = new UIVector(0,-40, 1, 1),
               // CanvasSize = 3,
                RectColor = Color.Transparent,
                BorderSize = 0,
                ClipsDescendants = true,
            };

            var inputbar = new RectNode("inputbox")
            {
                Parent = ConsoleWindow,
                Position = UIVector.FromScale(0, 1),
                AnchorPoint = new Vector2(0, 1),
                Size = new UIVector(0, 20, 1, 0),
                RectColor = new Color(0.1f, 0.1f, 0.1f),
                BorderSize = 0,
            };

            Input = new TextInputNode("input")
            {
                TextColor = Color.White,
                Font = GraphicsService.Get().Fonts.Arial12,
                XAlignment = TextXAlignment.Left,
                Parent = inputbar,
                ClearOnReturn = true,
            };

			Input.OnInputMessageSent += OnInputBoxEntered;
            Input.BlacklistCharacter('`');
        }


		#region Builtin Command Methods

		void HelpCommand(CommandEventArgs args)
		{
			Log("Available Commands:",  Color.Green);
			foreach(var command in Commands)
				Log($"{command.Keyword} {command.Description}", Color.Green);
		}

		#endregion

        public override void Initialize()
        {
            base.Initialize();
        }


        public void ClearConsole()
        {
            MessageHistory.Clear();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ConsoleWindow.Update(gameTime);
        }


		void RunCommand(ICommandSender sender, string input)
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
			MessageHistory.Add(new Message{
				Text = $"No command '{keyword}' found! Type 'help' for a list of commands.",
				Color = new Color(1f, 0.5f, 0.5f),
			});
		}

		void OnInputBoxEntered(string text)
		{
			MessageHistory.Add(new Message{
				Text = $">{text}",
				Color = Color.Gray
			});
			RunCommand(this, text);
		}

		// recieved from server console or other places
        public void ProcessCommandString(ICommandSender sender, string input)
        {
			MessageHistory.Add(new Message($"[{sender.Name}] {input}"));
            RunCommand(this, input);
        }

		public void Draw()
		{
			var GFX = GraphicsService.Get();
            if (!Open)
                return;

            GFX.Begin();
            ConsoleWindow.Draw();
			int iterator = 0;
			var history = MessageHistory.ToList();
			history.Reverse();

			var showableLines = MessageHistoryBox.AbsoluteSize.Y / 12;

			var bottom = MessageHistoryBox.AbsolutePosition + new Vector2(0, MessageHistoryBox.AbsoluteSize.Y - 14);

			foreach(var mesg in history)
			{
				GFX.Text(GraphicsService.Get().Fonts.Arial10, mesg.Text, bottom - new Vector2(0, 12*iterator), mesg.Color);
				iterator++;

				if (iterator > showableLines)
					break;
			}
            GFX.End();
		}

		public void Log(string text)
		{
			MessageHistory.Add(new Message(text));
		}
		public void Log(string text, Color color)
		{
			MessageHistory.Add(new Message{
				Text = text,
				Color = color
			});
		}


		// maps console color IDs to XNA color class.
		static Dictionary<ConsoleColor, Color> ConsoleColorMap = new Dictionary<ConsoleColor, Color>
		{
			[ConsoleColor.Black] 		= Color.Black,
			[ConsoleColor.DarkBlue] 	= Color.DarkBlue,
			[ConsoleColor.DarkGreen] 	= Color.DarkGreen,
			[ConsoleColor.DarkCyan] 	= Color.DarkCyan,
			[ConsoleColor.DarkRed] 		= Color.DarkRed,
			[ConsoleColor.DarkMagenta] 	= Color.DarkMagenta,
			[ConsoleColor.DarkYellow] 	= Color.Orange,
			[ConsoleColor.Gray] 		= Color.Gray,
			[ConsoleColor.DarkGray] 	= Color.DarkGray,
			[ConsoleColor.Blue] 		= Color.Blue,
			[ConsoleColor.Green]		= Color.Green,
			[ConsoleColor.Cyan]			= Color.Cyan,
			[ConsoleColor.Red] 			= Color.Red,
			[ConsoleColor.Magenta]		= Color.Magenta,
			[ConsoleColor.Yellow] 		= Color.Yellow,
			[ConsoleColor.White]		= Color.White
		};

		public void Log(string text, ConsoleColor fg, ConsoleColor bg, bool timestamp)
		{	
			Color sendingThisColor = Color.White;
			
			bool success = ConsoleColorMap.TryGetValue(fg, out sendingThisColor);

			MessageHistory.Add(new Message{
				Text = text,
				Color = sendingThisColor
			});
		}
	}
}