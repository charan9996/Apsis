using NLog;
using System;
using System.Collections.Generic;
using System.Text;

namespace Apsis.Logging
{
    /// <summary>
    /// Log wrapper
    /// </summary>
    public class Logger
    {
        static NLog.Logger logger = LogManager.GetLogger("NLogger");

        /// <summary>
        /// Logs exception events
        /// </summary>
        /// <param name="exception"></param>
        public static void LogException(Exception exception)
        {
            logger.Log(LogLevel.Fatal, exception);
        }

        /// <summary>
        /// Logs Information events
        /// </summary>
        /// <param name="message"></param>
        public static void LogInfo(string message)
        {
            logger.Log(LogLevel.Info, message);
        }

        /// <summary>
        /// Logs Debug events
        /// </summary>
        /// <param name="message"></param>
        public static void LogDebug(string message)
        {
            logger.Log(LogLevel.Debug, message);
        }

        /// <summary>
        /// Logs Error events
        /// </summary>
        /// <param name="message"></param>
        public static void LogError(string message)
        {
            logger.Log(LogLevel.Error, message);
        }

        /// <summary>
        /// Logs Warning events
        /// </summary>
        /// <param name="message"></param>
        public static void LogWarn(string message)
        {
            logger.Log(LogLevel.Warn, message);
        }
    }
}
