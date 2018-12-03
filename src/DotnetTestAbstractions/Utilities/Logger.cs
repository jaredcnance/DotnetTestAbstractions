using System;
using Microsoft.Extensions.Logging;

namespace DotnetTestAbstractions.Utilities
{
    public class Logger
    {
        private static readonly LogLevel _logLevel = Enum.TryParse<LogLevel>(Environment.GetEnvironmentVariable("DTA_LOG_LEVEL"), out LogLevel result)
            ? result
            : LogLevel.Critical;

        private const string prefix = "   [𝐝𝐭𝐚]: ";

        public static void Debug(object message)
        {
            if(_logLevel <= LogLevel.Debug)
                Console.WriteLine(prefix + message);
        }

        public static void Info(object message)
        {
            if(_logLevel <= LogLevel.Information)
                Console.WriteLine(prefix + message);
        }

        internal static void Error(object message)
        {
            if(_logLevel <= LogLevel.Error)
                Console.WriteLine(prefix + message);
        }
    }
}