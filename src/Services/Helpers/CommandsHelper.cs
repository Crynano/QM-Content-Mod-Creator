using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QM_ImporterAPI.Services.Helpers
{
    internal static class CommandsHelper
    {
        public static List<string> GetDirectorySuggestions(string currentPath)
        {
            if (string.IsNullOrEmpty(currentPath))
            {
                return null;
            }

            var suggestions = new List<string>();

            var trimLastSegment = currentPath.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var lastPath = currentPath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar).LastOrDefault();

            var enumerateDirectories = Directory.EnumerateDirectories(trimLastSegment, "*", SearchOption.TopDirectoryOnly);
            if (string.IsNullOrEmpty(lastPath))
            {
                suggestions.AddRange(enumerateDirectories);

            }
            else
            {
                suggestions.AddRange(enumerateDirectories.Where(d => Path.GetFileName(d).StartsWith(lastPath)));
            }

            Logger.LogDebug($"Directory suggestions for '{currentPath}': {string.Join(", ", suggestions)}");

            return suggestions;
        }
    }
}
