using System;

namespace Wasteland.Common
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
}
