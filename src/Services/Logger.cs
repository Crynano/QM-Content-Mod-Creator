using System;
using UnityEngine;

namespace QM_ImporterAPI.Services
{
    internal static class Logger
    {
        private enum LogType
        {
            Info,
            Warning,
            Error,
            Debug
        }

        private static string LogSignature = "Content Mod Creator";

        private static string Context = "";

        public static void LogDebug(string message)
        {
            // Only will log if debug mode.
#if DEBUG
            WriteToLog(message, LogType.Debug);
#endif
        }

        public static void LogInfo(string message)
        {
            WriteToLog(message, LogType.Info);
        }

        public static void LogWarning(string message)
        {
            WriteToLog(message, LogType.Warning);
        }

        public static void LogError(string message)
        {
            WriteToLog(message, LogType.Error, true);
        }

        private static void WriteToLog(string message, LogType logType, bool writeToUnity = true)
        {
            string beautifiedMessage = GetBeautifiedMessage(message, logType);

            if (writeToUnity) Debug.Log(beautifiedMessage);
        }

        private static string GetBeautifiedMessage(string message, LogType logType)
        {
            return $"[{DateTime.Now.ToString()}][{LogSignature}][{logType.ToString().ToUpper()}]" +
                (string.IsNullOrEmpty(Context) ? "" : $"[{Context}]") +
                $": {message}";
        }
    }
}
