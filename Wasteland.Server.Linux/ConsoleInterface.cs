using System.ComponentModel;
using System.Threading.Tasks;
using System.Threading;
using System;
using System.Collections.Generic;
using Conarium;
using System.Linq;
using Wasteland.Common;

namespace Wasteland.Server.Linux
{

    public struct ColoredString
    {
        public string Text;
        public ConsoleColor Background;
        public ConsoleColor Foreground;
        public ColoredString(string text, ConsoleColor fg, ConsoleColor bg)
        {
            Text = text;
            Background = bg;
            Foreground = fg;
        }
    }

    public struct BufferString
    {
        public BufferString(List<ColoredString> strings)
        {
            int fullTextLength = 0;
            foreach (var str in strings)
            {


                fullTextLength += str.Text.Length;
            }
            BufferCharacters = new BufferIndex[fullTextLength];


            int iterator = 0;
            foreach (var str in strings)
            {
                foreach (char c in str.Text)
                {
                    BufferCharacters[iterator] = new BufferIndex
                    {
                        BackgroundColor = str.Background,
                        ForegroundColor = str.Foreground,
                        Character = c
                    };
                    iterator++;
                }

            }
            
        }

        public BufferString(string text, 
            ConsoleColor fgColor = ConsoleColor.Black, 
            ConsoleColor bgColor = ConsoleColor.White)
        {

            // build buffer array
            BufferCharacters = new BufferIndex[text.Length];

            int iterator = 0;
            foreach (char c in text)
            {
                BufferCharacters[iterator] = new BufferIndex
                {
                    BackgroundColor = bgColor,
                    ForegroundColor = fgColor,
                    Character = c
                };
                iterator++;
            }
        }

        public BufferIndex[] BufferCharacters {get; set;}

        public string Text
        {
            get {
                string text = "";
                foreach(var index in BufferCharacters)
                    text += index.Character;
                return text;
            }
        }

        public void Write(int x, int y)
        {
            
            for (int delta = 0; delta < BufferCharacters.Length; delta++)
            {
                var index = BufferCharacters[delta];
                Console.ForegroundColor = index.ForegroundColor;
                Console.BackgroundColor = index.BackgroundColor;
                Console.SetCursorPosition(x+delta, y);
                Console.Write(index.Character);
            }   
        }
    }

    public struct BufferIndex
    {
        public BufferIndex(char character)
        {
            BackgroundColor = ConsoleColor.White;
            ForegroundColor = ConsoleColor.Black;
            Character = character;
        }
        public ConsoleColor BackgroundColor {get;set;}
        public ConsoleColor ForegroundColor {get;set;}
        public char Character {get;set;}

        public static BufferIndex Blank = new BufferIndex{
            BackgroundColor = ConsoleColor.White,
            ForegroundColor = ConsoleColor.Black,
            Character = ' ',
        };
    }
    

    public class WrapperInputArgs : EventArgs
    {
        public string InputMessage {get;set;}

        public WrapperInputArgs(string msg)
        {
            InputMessage = msg;
        }
    }

    public class TerminalWrap : IServerOutput
    {
        public List<Command> CommandSet; // reference to set of commands to show autocomplete help

        public event EventHandler<string> OnCommandSent;

        public bool Running {get;set;}
        public string ConsoleTitle {get;set;}

        #region Theming
        public ConsoleColor DefaultBackgroundColor {get; set;} = ConsoleColor.White;
        public ConsoleColor DefaultForegroundColor {get; set;} = ConsoleColor.Black;
        public ConsoleColor DefaultDecorationColor {get; set;} = ConsoleColor.DarkBlue;
        #endregion
        public int Width => Console.WindowWidth;
        public int Height => Console.WindowHeight;

        List<BufferString> MessageHistory;

        string CommandInputBuffer;
        public TerminalWrap()
        {
            Running = true;
            MessageHistory = new List<BufferString>();
            CommandInputBuffer = "";
            ConsoleTitle = "Terminal Wrapper";
        }

        public void Put(char c, int x, int y)
        {
            Console.SetCursorPosition(x, y);
            Console.Write(c);
        }

        public void Log(string text) 
        {
            Log(text, ConsoleColor.Black, ConsoleColor.White);
        }
        public void Log(string text, ConsoleColor fg, ConsoleColor bg, bool timestamp = true)
        {
            string stamp = "";
            if (timestamp)
                stamp = $"[{DateTime.Now.ToShortDateString()} {DateTime.Now.ToShortTimeString()}] ";


            var strings = new BufferString(new List<ColoredString>{
                new(stamp, ConsoleColor.DarkGray, ConsoleColor.White),
                new(text, fg, bg)
            });
            MessageHistory.Add(strings);

            RedrawMessageHistory();
        }
        
        public void ListenForInputTask()
        {
            ConsoleKeyInfo recv;

            do
            {
                recv = Console.ReadKey(true);

                // custom ctrl+C functionality
                if ((recv.Modifiers & ConsoleModifiers.Control) != 0 && recv.Key == ConsoleKey.C) 
                {
                    Console.ResetColor();
                    System.Environment.Exit(1);

                }else if (recv.Key == ConsoleKey.Tab)
                {
                    DrawCommandSuggestions();
                }else if (recv.Key == ConsoleKey.Backspace && CommandInputBuffer != "")
                {
                    RedrawInputBuffer();

                    CommandInputBuffer = CommandInputBuffer.Remove(CommandInputBuffer.Length-1, 1);
                }else if (recv.Key == ConsoleKey.Enter)
                {
                    // TODO: invoke command
                    OnCommandSent?.Invoke(this, new(CommandInputBuffer));

                    Log(">"+CommandInputBuffer, ConsoleColor.DarkGray, ConsoleColor.White, false);

                    CommandInputBuffer = "";
                    RedrawMessageHistory();
                    //FullRedraw();
                }else
                {
                    CommandInputBuffer += recv.KeyChar;
                }
                RedrawInputBuffer();


            } while (Running);
        }

        protected void DrawCommandSuggestions()
        {
            var autocompleteSnippet = CommandInputBuffer;

            foreach(var command in CommandSet)
            {
                if (command.Keyword.StartsWith(autocompleteSnippet))
                {
                    CommandInputBuffer = command.Keyword;
                    return;
                }

                foreach(var alias in command.Aliases)
                {
                    if (alias.StartsWith(autocompleteSnippet))
                    {
                        CommandInputBuffer = command.Keyword;
                        return;
                    }
                }
                    
            }
        }

        protected void RedrawBackground()
        {
            Console.BackgroundColor = DefaultBackgroundColor;
            Console.ForegroundColor = DefaultForegroundColor;
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    
                    Console.SetCursorPosition(x, y);
                    Console.Write(' ');
                }
            }
        }

        protected void RedrawMessageHistory()
        {
            var lastMessages = MessageHistory.ToList();
            //lastMessages.Reverse();

            for (int id = 0; id < lastMessages.Count; id++)
            {
                var msg = lastMessages[id];
                msg.Write(0, 1+id);
            }
        }

        protected void RedrawInputBufferPartial()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Black;

            

            Console.SetCursorPosition(0, Height-1);
            Console.Write("> ");
            Console.SetCursorPosition(2, Height-1);
            Console.Write(CommandInputBuffer);
        }

        protected void RedrawInputBuffer()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Black;

            // clear fucked text
            for (int barX = 0; barX < Width-1; barX++)
                Put(' ', barX, Height-1);


            Console.SetCursorPosition(0, Height-1);
            Console.Write("> ");
            Console.SetCursorPosition(2, Height-1);
            Console.Write(CommandInputBuffer);
        }

        protected void RedrawTopbar()
        {
            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.ForegroundColor = ConsoleColor.Black;

            // draw top bar
            for (int barX = 0; barX < Width-1; barX++)
                Put(' ', barX, 0);

            Console.SetCursorPosition(1, 0);
            Console.Write($"{ConsoleTitle} => {DateTime.Now.ToShortDateString()} @ {DateTime.Now.ToShortTimeString()}");
        }


        protected void TopbarTask()
        {
            int topbarRefreshRateMS = (1000)/5;
            while(Running)
            {
                RedrawTopbar();
                Thread.Sleep(topbarRefreshRateMS);
            }
        }

        public void FullRedraw()
        {
            RedrawBackground();
            RedrawTopbar();
            RedrawMessageHistory();
            RedrawInputBuffer();
            
        }

        public void Start()
        {
            FullRedraw();
            Task.Run(ListenForInputTask);
            //Task.Run(TopbarTask);


            while(Running)
            {
                Thread.Sleep(100);
                
                // sup
            }
            Console.WriteLine("See you next time!");
        }
    }
}