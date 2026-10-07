using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Commands
{
    [ConsoleCommand(new string[] { "export-mercenaryprofile" })]
    public class ExportMercenaryProfileCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Export the first in-game mercenary profile record to a JSON file as an example.";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                if (tokens.Length == 0)
                {
                    return "<color=red>ERROR: </color>No folder path provided. Syntax: export-mercenaryprofile <folder-path>";
                }

                var providedPath = tokens[0];

                if (string.IsNullOrEmpty(providedPath))
                {
                    return "<color=red>ERROR: </color>No folder path provided. Syntax: export-mercenaryprofile <folder-path>";
                }
                else if (!Path.IsPathRooted(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path must be an absolute path.";
                }
                else if (!Directory.Exists(providedPath))
                {
                    return "<color=red>ERROR: </color>Provided path does not exist.";
                }

                var profiles = Data.MercenaryProfiles;

                var exportedCount = Data.MercenaryProfiles.Ids
                    .Select(id => profiles.GetRecord(id))
                    .Where(mercenaryProfile => mercenaryProfile != null && !mercenaryProfile.Id.Contains("_custom"))
                    .Select(mercenaryProfile => { ExportHelper.ExportItem(mercenaryProfile, providedPath); return mercenaryProfile; })
                    .Count();

                if (exportedCount == 0)
                {
                    return "<color=red>ERROR: </color>No mercenary profile records found.";
                }

                return $"<color=green>Exported {exportedCount} mercenary profile(s) to JSON file.</color>";
            }
            catch (Exception ex)
            {
                string msg = $"<color=red>ERROR: </color>" + ex.Message;
                Debug.LogError(ex.InnerException);
                return msg;
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
