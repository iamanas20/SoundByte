using System;

namespace SoundByte.UWP.Services
{
    public static class LoggingService
    {
        public static void Log(LogType type, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{type}] - {DateTime.Now:hh:mm:ss:fff} : {message}");
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
