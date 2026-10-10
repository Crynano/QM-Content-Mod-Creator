using MGSC;
using QM_ImporterAPI.Services.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Commands
{
    [ConsoleCommand(new string[] { "export-firemodes" })]
    public class ExportFiremodesCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Export all in-game firemodes to JSON files. Syntax: export-firemodes <folder-path>";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                if (tokens.Length == 0 || string.IsNullOrEmpty(tokens[0]))
                {
                    return "<color=red>ERROR: </color>No folder path provided. Syntax: export-firemodes <folder-path>";
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

                var exportedCount = Data.Firemodes.Ids
                    .Select(id => Data.Firemodes.GetRecord(id))
                    .Where(firemode => firemode != null)
                    .Select(firemode => { ExportHelper.ExportItem(firemode, providedPath); return firemode; })
                    .Count();

                return $"<color=green>Exported {exportedCount} firemodes to JSON files.</color>";
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.StackTrace);
                return "<color=red>ERROR: </color>" + ex.Message;
            }
        }

        public static List<string> FetchAutocompleteOptions(string command, string[] tokens)
        {
            return null;
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
