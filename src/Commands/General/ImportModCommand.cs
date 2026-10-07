using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Commands.General
{
    [ConsoleCommand(new string[] { "import-mod", "api-import-mod" })]
    public class ImportModCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Import a mod folder manually. Syntax: import-mod <folderPath>";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                if (tokens.Length == 0)
                {
                    return "<color=red>ERROR: </color>No folder path provided. Syntax: create-mod <folder-path>";
                }

                var providedPath = tokens[0];

                if (string.IsNullOrEmpty(providedPath))
                {
                    return "<color=red>ERROR: </color>No folder path provided. Syntax: create-mod <folder-path>";
                }
                else if (!Path.IsPathRooted(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path must be an absolute path.";
                }
                else if (!Directory.Exists(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path does not exist.";
                }

                ModLoader.LoadModFromDirectory(providedPath);

                return $"<color=green>Imported mod successfully!</color>";
            }
            catch (Exception ex)
            {
                string msg = $"<color=red>ERROR: </color>" + ex.Message;
                Debug.LogError(ex.StackTrace);
                return msg;
            }
        }

        public static List<string> FetchAutocompleteOptions(string command, string[] tokens)
        {
            var suggestions = CommandsHelper.GetDirectorySuggestions(tokens.Length > 0 ? tokens[0] : string.Empty);
            if (suggestions == null)
            {
                return null;
            }

            return suggestions.Select(s => command + " " + s).ToList();
        }

        public static bool IsAvailable()
        {
            return true;
        }

        public static bool ShowInHelpAndAutocomplete()
        {
            return true;
        }
    }
}