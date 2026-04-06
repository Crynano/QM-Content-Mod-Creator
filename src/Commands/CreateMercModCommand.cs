using MGSC;
using QM_ImporterAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Commands
{
    [ConsoleCommand(new string[] { "create-merc-mod", "api-create-merc-mod" })]
    public class CreateMercModCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Creates folders and example files to start creating a mercenary class mod using the Importer API. Syntax: create-merc-mod <folder-path>";
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

                ModCreator.CreateMercMod(providedPath);

                return $"<color=green>Created Assets folder and example files at \"{providedPath}\".</color>";
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
            return new List<string>();
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