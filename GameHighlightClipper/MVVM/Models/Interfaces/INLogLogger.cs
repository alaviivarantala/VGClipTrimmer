using System;

namespace GameHighlightClipper.MVVM.Models.Interfaces
{
    public interface INLogLogger
    {
        void LogDebug(string message);
        void LogInfo(string message);
        void LogError(string message);
        void LogWarn(string message);
        void LogTrace(string message);

        void LogDebug(Exception exception, string message);
        void LogInfo(Exception exception, string message);
        void LogError(Exception exception, string message);
        void LogWarn(Exception exception, string message);
        void LogTrace(Exception exception, string message);

        void LogDebug(Exception exception);
        void LogInfo(Exception exception);
        void LogError(Exception exception);
        void LogWarn(Exception exception);
        void LogTrace(Exception exception);
    }
}
