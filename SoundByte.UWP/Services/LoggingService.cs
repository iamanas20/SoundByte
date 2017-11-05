/* |----------------------------------------------------------------|
 * | Copyright (c) 2017, Grid Entertainment                         |
 * | All Rights Reserved                                            |
 * |                                                                |
 * | This source code is to only be used for educational            |
 * | purposes. Distribution of SoundByte source code in             |
 * | any form outside this repository is forbidden. If you          |
 * | would like to contribute to the SoundByte source code, you     |
 * | are welcome.                                                   |
 * |----------------------------------------------------------------|
 */


using System;

namespace SoundByte.UWP.Services
{
    public static class LoggingService
    {
        public static void Log(LogType type, string message)
        {
            System.Diagnostics.Debug.WriteLine($"[{type}] - {DateTime.Now:T} : {message}");
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
