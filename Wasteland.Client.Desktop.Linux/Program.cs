﻿using System;

namespace Wasteland.Client.Desktop.Linux
{
    public static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var game = new LinuxClientApp())
                game.Run();
        }
    }
}
