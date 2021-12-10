using Microsoft.Xna.Framework;

namespace Wasteland.Common
{

	public interface ILogSender
	{
		Color Color {get;}
		string Name {get;}
	}

	public static class LogService
	{
		
		
		public static event LogEvent OnLogged;
		public static void AddLogListener(LogEvent callback) => OnLogged += callback;
		public delegate void LogEvent(object sender, string message);
		public static void Log(object sender, string message)
		{
			OnLogged?.Invoke(sender, message);
		}
		public static void Log(ILogSender logger, string message)
		{

			OnLogged?.Invoke(logger, message);
		}
		public static void LogVerbatim(string message)
		{

		}
		/*
		LogService.OnLogged += GameConsole.OnLogged; or ServerConsole.OnLogged;
		GameConsole
		Server
		Client
		ServerConsole

		*/

	}
}