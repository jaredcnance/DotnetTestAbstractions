using System;

namespace DotnetTestAbstractions.Utilities
{
    public class Logger
    {
        public static void Debug(object message)
        {
            Console.WriteLine(message);
        }

        public static void Info(object message)
        {
            Console.WriteLine(message);
        }

        internal static void Error(object message)
        {
            Console.WriteLine(message);
        }
    }
}