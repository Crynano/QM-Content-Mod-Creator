using System;
using System.IO;
using System.Reflection;
using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
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

        private static string LogFileName => $"Log.log"; //$"Log_{DateTime.Now.ToString(@"dd_MM_yyyy")}.log";
        private static string LogSignature = "Content Mod Creator";

        private static string LogStart => $"[{DateTime.Now}][{LogSignature}][START] ----------------- Log Start -----------------\nGame Version Report: {Application.version}\nWeapon Importer Version Report: {Assembly.GetExecutingAssembly().GetName().Version}\n";
        private static string LogEnd => $"[{DateTime.Now}][{LogSignature}][#END#] |---------------- Log #End# ----------------|\n";

        private static string Context = "";
        private static string Log = "";

        private static string LogPath = Path.Combine(Plugin.ConfigDirectories.AllModsConfigFolder, LogFileName);

        public static void SetConfig(string modName)
        {
            LogSignature = modName;
            LogPath = Path.Combine(Plugin.ConfigDirectories.AllModsConfigFolder, modName, LogFileName);
        }

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
            WriteToGameConsole(message, LogType.Error);
        }

        public static void SetContext(string context)
        {
            Context = context;
        }

        public static void ClearContext()
        {
            Context = "";
        }

        private static void WriteToLog(string message, LogType logType, bool writeToUnity = true)
        {
            string beautifiedMessage = GetBeautifiedMessage(message, logType);

            if (writeToUnity) Debug.Log(beautifiedMessage);
            Log += $"{beautifiedMessage}\n";
        }

        private static string GetBeautifiedMessage(string message, LogType logType)
        {
            return $"[{DateTime.Now.ToString()}][{LogSignature}][{logType.ToString().ToUpper()}]" +
                (string.IsNullOrEmpty(Context) ? "" : $"[{Context}]") +
                $": {message}";
        }
        private static string GetConsoleMessage(string message, LogType logType)
        {
            if (logType == LogType.Error)
            {
                return $"<color=red>{message}</color>";
            }
            else if (logType == LogType.Warning)
            {
                return $"<color=yellow>{message}</color>";
            }
            else
            {
                return message;
            }
        }

        private static void WriteToGameConsole(string message, LogType logType)
        {
            var beautifiedMessage = GetConsoleMessage(message, logType);
            UI.Get<DevConsole>()?.PrintText(beautifiedMessage);
        }

        public static void Flush()
        {
            string finalLog = LogStart + Log + LogEnd;
            File.WriteAllText(LogPath, finalLog);
            ResetLog();
        }

        public static void FlushAdditive()
        {
            string existingLog = "";
            if (File.Exists(LogPath))
            {
                existingLog = File.ReadAllText(LogPath);
            }
            string finalLog = LogStart + Log + LogEnd;
            existingLog += finalLog;
            File.WriteAllText(LogPath, existingLog);
            ResetLog();
        }

        private static void ResetLog()
        {
            Log = "";
        }
    }
}
