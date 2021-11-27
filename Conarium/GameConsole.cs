using System.Windows.Input;
using System;
using System.Collections.Generic;
using Conarium.Services;
using Conarium.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Linq;

namespace Conarium 
{

    public struct Message
    {
        public string Text;
        public Color Color;

        public Message(string contents)
        {
            Text = contents;
            Color = Color.White;
        }
    }

    public class CommandEventArgs
    {
        public Command Command;
        public string[] Args;
        public ICommandSender Sender;
    }

    public delegate void CommandEvent(CommandEventArgs args);
    public class Command
    {
        public string Keyword;
        public string Description;
        public List<string> Arguments;
        public List<string> Aliases;
        public event CommandEvent OnCalled;

        public CommandEvent Callback;

        public void RunCommand(ICommandSender sender, string[] args)
        {
            var commandArgs = new CommandEventArgs{
                Command = this,
                Args = args,
                Sender = sender
            };
            Callback?.Invoke(commandArgs);
            OnCalled?.Invoke(commandArgs); // external event listener, not sure if will be used
        }


        public Command(string keyword)
        {
            Keyword = keyword;
        }

        public Command(string keyword, CommandEvent callback)
        {
            Keyword = keyword;
            Description = "";
            Arguments = new List<string>();


            Callback = callback;
        }


    }


    public interface ICommandSender
    {
        Color SenderColor {get;}
        string Name {get;}
    }

    public interface ICommandReciever
    {
        void ProcessCommandString(ICommandSender sender, string rawtext);
        void BindCommand(Command command);
    }

    public class GameConsole : GameComponent, ICommandReciever
    {
        public Conarium.UI.WindowNode ConsoleWindow {get;set;}
        public bool Open {get; set;}

        

        List<Command> Commands;

        public void BindCommand(Command command) => Commands.Add(command);

		public List<Message> MessageHistory {get; private set;}

        public List<string> CommandHistory {get; set;}

        TextInputNode Input {get;set;}

        public GameConsole(Game game) : base(game)
        {
            ConsoleWindow = new WindowNode("GameConsole", 600, 200);

            var messageHistoryBox = new ScrollRectNode("history")
            {
                Parent = ConsoleWindow,
                Position = new UIVector(0,20,0,0),
                Size = new UIVector(0,-20, 1, 1),
                CanvasSize = 3,
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
                Font = GraphicsService.Get().Fonts.Arial14,
                XAlignment = TextXAlignment.Left,
                Parent = inputbar,
                ClearOnReturn = true,
            };
            Input.BlacklistCharacter('`');
        }

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

		public void Draw()
		{
			var GFX = GraphicsService.Get();
            if (!Open)
                return;

                

            GFX.Begin();
            ConsoleWindow.Draw();
            GFX.End();
		}
    }
}