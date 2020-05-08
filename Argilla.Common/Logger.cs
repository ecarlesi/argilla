﻿using System;
namespace Argilla.Common
{
    //TODO use the system logger instead of the System.Console

    /// <summary>
    /// Simple logger
    /// </summary>
    public static class Logger
    {
        public static void Debug(string message)
        {
            Write(LogLevel.Debug, message);
        }

        public static void Info(string message)
        {
            Write(LogLevel.Info, message);
        }

        public static void Warn(string message)
        {
            Write(LogLevel.Warn, message);
        }

        public static void Error(string message)
        {
            Write(LogLevel.Error, message);
        }

        public static void Exception(Exception exception)
        {
            Write(LogLevel.Exception, exception.ToString());
        }

        private static void Write(LogLevel logLevel, string message)
        {
            Console.WriteLine(String.Format("[{0}] [{1}] {2}", DateTime.Now, logLevel.ToString(), message));
        }
    }

    public enum LogLevel
    {
        Debug, Info, Warn, Error, Exception
    }
}