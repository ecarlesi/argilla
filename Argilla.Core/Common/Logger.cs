//using System;
//using System.Threading;

//namespace Argilla.Core.Common
//{
//    /// <summary>
//    /// Simple logger
//    /// </summary>
//    public static class Logger
//    {
//        private static NLog.Logger logger = NLog.Web.NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();

//        public static void Debug(string message)
//        {
//            logger.Debug(message);
//        }

//        public static void Info(string message)
//        {
//            logger.Info(message);
//        }

//        public static void Warn(string message)
//        {
//            logger.Warn(message);
//        }

//        public static void Error(string message)
//        {
//            logger.Error(message);
//        }

//        public static void Exception(Exception exception)
//        {
//            logger.Error(exception.Message, exception, null);
//        }
//    }
//}
