using System.Net.Mime;
using System;
using System.Collections.Generic;
using Conarium;

namespace Wasteland.Server.Linux
{
	class Program
	{

		static void Main(string[] args)
		{
			var server = new StandaloneServer();
			var terminal = new TerminalWrap{
				ConsoleTitle = "Wasteland Server"
			};

			server.Logger = terminal;
			terminal.OnCommandSent += (_, command)
				=> server.ProcessCommandString(server, command);
			terminal.CommandSet = server.Commands; // todo: refactor, this seems stupid
			server.Start();
			terminal.Start();
			
		}
	}
}

