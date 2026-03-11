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
    [ConsoleCommand(new string[] { "export-weapons" })]
    public class ExportWeaponsCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Export all in-game weapons to JSON files.";
        }

        public string Execute(string[] tokens)
        {
            try
            {
                var path = Path.Combine(Plugin.ConfigDirectories.ModPersistenceFolder, "Exports", "Weapons");
                Directory.CreateDirectory(path);
                var items = Data.Items;

                var exportedCount = Data.Items.Ids
                    .Select(id => items.GetSimpleRecord<WeaponRecord>(id))
                    .Where(weapon => weapon != null)
                    .Select(weapon => { ExportItems(weapon, path); return weapon; })
                    .Count();

                return $"<color=green>Exported {exportedCount} weapons to JSON files.</color>";
            }
            catch (Exception ex)
            {
                string msg = $"<color=red>ERROR: </color>" + ex.Message;
                Debug.LogError(ex.InnerException);
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