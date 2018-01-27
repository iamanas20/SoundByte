using System;
using System.Collections.Generic;

namespace SoundByte.UWP.Services
{
    public static class LoggingService
    {
        public static List<string> Logs { get; set; } = new List<string>();

        public static void Log(LogType type, string message)
        {
            var logItem = $"[{type}] - {DateTime.Now:hh:mm:ss:fff} : {message}";

            Logs.Add(logItem);
            System.Diagnostics.Debug.WriteLine(logItem);
        }

        public enum LogType
        {
            Debug,
            Info,
            Warning,
            Error,
            Fatal
        }
    }
}
