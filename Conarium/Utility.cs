using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Conarium
{
    public static class Utility
    {
        public static bool IsWindows() => RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
		public static bool IsLinux() => RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
		public static bool IsOSX() => RuntimeInformation.IsOSPlatform(OSPlatform.OSX);
		public static bool IsFreeBSD() => RuntimeInformation.IsOSPlatform(OSPlatform.FreeBSD);
        public static void OpenUrl(string url)
		{
			try
			{
				Process.Start(url);
			} catch
			{
				// hack because of this: https://github.com/dotnet/corefx/issues/10361
				if (IsWindows())
				{
					url = url.Replace("&", "^&");
					Process.Start(new ProcessStartInfo("cmd", $"/c start {url}") { CreateNoWindow = true });
				}
				else if (IsLinux())
					Process.Start("xdg-open", url);
				else if (IsOSX())
					Process.Start("open", url);
				else
					throw;
			}
		}

		public static T[] Populate<T>(this T[] array, Func<T> provider)
		{
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = provider();
			}
			return array;
		}

		public static T[,] PopulateGrid<T>(this T[,] array, Func<T> provider)
		{
			var rows = array.GetLength(0);
			var columns = array.GetLength(1);
			for (int x = 0; x < rows; x++)
			{
				for (int y = 0; y < columns; y++)
				{
					array[x,y] = provider();
				}
				
			}
			return array;
		}



		public static async Task<string> RunProgramAsync(string filename, string arguments) =>await Task.Run(()=> RunProgram(filename, arguments));

		public static string RunProgram(string filename, string arguments)
		{
			var process = new Process
			{
				StartInfo = new ProcessStartInfo
				{
					FileName = filename,
					Arguments = arguments,
					RedirectStandardOutput = true,
					UseShellExecute = false,
					CreateNoWindow = false,
				}
			};
			process.Start();
			string result = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			return result;
		}


		public static string InvokeBashCommand(string command)
		{
			var escapedArgs = command.Replace("\"", "\\\"");
			string result = RunProgram("/bin/bash", $"-c \"{escapedArgs}\"");
			return result;
		}

		public static string InvokeBatchCommand(string command)
		{
			var escapedArgs = command.Replace("\"", "\\\"");
			string result = RunProgram("cmd.exe", $"/c \"{escapedArgs}\"");
			return result;
		}
    }
}