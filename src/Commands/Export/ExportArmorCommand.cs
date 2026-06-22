using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Commands
{
    [ConsoleCommand(new string[] { "export-armor" })]
    public class ExportArmorCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Export all in-game armors to JSON files.";
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

                var items = Data.Items;

                var armorCount = Data.Items.Ids
                    .Select(id => items.GetSimpleRecord<ArmorRecord>(id))
                    .Where(armor => armor != null)
                    .Select(armor => { ExportItems(armor, providedPath); return armor; });

                var bootsCount = Data.Items.Ids
                    .Select(id => items.GetSimpleRecord<BootsRecord>(id))
                    .Where(boot => boot != null)
                    .Select(boot => { ExportItems(boot, providedPath); return boot; });

                var leggingsCount = Data.Items.Ids
                    .Select(id => items.GetSimpleRecord<LeggingsRecord>(id))
                    .Where(leggings => leggings != null)
                    .Select(leggings => { ExportItems(leggings, providedPath); return leggings; });

                var helmetCount = Data.Items.Ids
                    .Select(id => items.GetSimpleRecord<HelmetRecord>(id))
                    .Where(helmet => helmet != null)
                    .Select(helmet => { ExportItems(helmet, providedPath); return helmet; });

                var sum = armorCount.Count() + bootsCount.Count() + leggingsCount.Count() + helmetCount.Count();

                return $"<color=green>Exported {sum} armor pieces to JSON files.</color>";
            }
            catch (Exception ex)
            {
                string msg = $"<color=red>ERROR: </color>" + ex.Message;
                Debug.LogError(ex.Message);
                Debug.LogError(ex.StackTrace);
                return msg;
            }
        }

        private static void ExportItems<T>(T item, string basePath) where T : ConfigTableRecord
        {
            var classType = item.GetType();
            var result = new ImportableJson()
            {
                RecordType = classType.FullName,
                Data = item
            };
            var pathCombined = Path.Combine(basePath, $"{item.Id}.json");
            File.WriteAllText(pathCombined, JsonConvert.SerializeObject(result, JsonExporterSettings.SerializerSettings));
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