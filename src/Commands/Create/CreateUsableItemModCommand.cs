using MGSC;
using QM_ImporterAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Commands.Create
{
    [ConsoleCommand(new string[] { "create-usable-mod", "api-create-usable-mod" })]
    public class CreateUsableItemModCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Creates folders and example files to start creating a usable item mod using the Importer API. Syntax: create-usable-mod <folder-path>";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                if (tokens.Length == 0 || string.IsNullOrEmpty(tokens[0]))
                {
                    return "<color=red>ERROR: </color>No folder path provided. Syntax: create-usable-mod <folder-path>";
                }

                var providedPath = tokens[0];

                if (!Path.IsPathRooted(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path must be an absolute path.";
                }
                else if (!Directory.Exists(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path does not exist.";
                }

                ModCreator.CreateUsableItemMod(providedPath);

                return $"<color=green>Created Assets folder and example files at \"{providedPath}\".</color>";
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.StackTrace);
                return "<color=red>ERROR: </color>" + ex.Message;
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
