using System;

namespace GameHighlightClipper.Helpers
{
    public static class NLogLogger
    {
        public static NLog.Logger Instance { get; private set; }

        static NLogLogger()
        {
            NLog.LogManager.ReconfigExistingLoggers();
            Instance = NLog.LogManager.GetCurrentClassLogger();
        }

        public static void LogInfo(string message)
        {
            Instance.Info(message);
        }

        public static void LogError(Exception exception, string message = "")
        {
            Instance.Error(exception, message);
        }
    }
}
