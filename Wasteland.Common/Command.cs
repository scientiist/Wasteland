using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Wasteland.Common
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
}