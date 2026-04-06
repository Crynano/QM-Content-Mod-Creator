using System;
using System.IO;

namespace QM_ImporterAPI.Services.Helpers
{
    public static class Helper
    {
        private static T StringToEnum<T>(string type) where T : Enum
        {
            foreach (T value in Enum.GetValues(typeof(T)))
            {
                if (value.ToString().ToLower().Contains(type.ToLower()))
                {
                    return value;
                }
            }
            return (T)Enum.ToObject(typeof(T), 0);
        }

        public static string ResolvePath(string basePath, string path)
        {
            return Path.Combine(basePath, path);
        }

        public static string FilterToken(string[] tokens, int index)
        {
            if (tokens.Length > index)
            {
                return tokens[index];
            }
            return null;
        }

        public static string ValidatePath(string providedPath)
        {
            if (string.IsNullOrEmpty(providedPath))
            {
                return "<color=red>ERROR: </color>No folder path provided.";
            }
            else if (!Path.IsPathRooted(providedPath))
            {
                return "<color=red>ERROR: </color>Provided path must be an absolute path.";
            }
            else if (!Directory.Exists(providedPath))
            {
                return "<color=red>ERROR: </color>Provided path does not exist.";
            }
            return null;
        }
    }
}
