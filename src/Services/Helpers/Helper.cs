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
    }
}
