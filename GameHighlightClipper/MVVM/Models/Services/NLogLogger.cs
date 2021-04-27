using GameHighlightClipper.MVVM.Models.Interfaces;
using NLog;
using System;

namespace GameHighlightClipper.MVVM.Models.Services
{
    public class NLogLogger : INLogLogger
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();
        public void LogDebug(string message)
        {
            logger.Debug(message);
        }
        public void LogInfo(string message)
        {
            logger.Info(message);
        }
        public void LogError(string message)
        {
            logger.Error(message);
        }
        public void LogWarn(string message)
        {
            logger.Warn(message);
        }
        public void LogTrace(string message)
        {
            logger.Trace(message);
        }

        public void LogDebug(Exception exception, string message)
        {
            logger.Debug(exception, message);
        }

        public void LogInfo(Exception exception, string message)
        {
            logger.Info(exception, message);
        }

        public void LogError(Exception exception, string message)
        {
            logger.Error(exception, message);
        }

        public void LogWarn(Exception exception, string message)
        {
            logger.Warn(exception, message);
        }
        public void LogTrace(Exception exception, string message)
        {
            logger.Trace(exception, message);
        }

        public void LogDebug(Exception exception)
        {
            logger.Debug(exception);
        }

        public void LogInfo(Exception exception)
        {
            logger.Info(exception);
        }

        public void LogError(Exception exception)
        {
            logger.Error(exception);
        }

        public void LogWarn(Exception exception)
        {
            logger.Warn(exception);
        }
        public void LogTrace(Exception exception)
        {
            logger.Trace(exception);
        }
    }
}
