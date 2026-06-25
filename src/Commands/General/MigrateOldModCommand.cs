using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Services.Mappers;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using QM_ImporterAPI.Templates.OldDescriptors;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Commands.General
{
    [ConsoleCommand(new string[] { "migrate-old-mod" })]
    public class MigrateOldModCommand
    {
        public static string Help(string command, bool verbose)
        {
            return "Migrates all weapons from the API Item And Weapon Importer Mod format to the Content Mod Creator format!";
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

                var migratedFolder = Path.Combine(providedPath, "MigratedWeapons");

                Directory.CreateDirectory(migratedFolder);

                var oldFiles = Directory.GetFiles(providedPath, "*.json");
                // Read and parse them as old. Discard those non-parsed
                List<OldCustomWeaponRecord> oldRecords = new List<OldCustomWeaponRecord>();
                foreach (var file in oldFiles)
                {
                    try
                    {
                        var content = File.ReadAllText(file);
                        var record = JsonConvert.DeserializeObject<OldCustomWeaponRecord>(content, JsonExporterSettings.DeserializerSettings);
                        if (record != null) { oldRecords.Add(record); }
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Failed to parse \"{file}\" as OldCustomWeaponRecord, skipping. Error: {ex.Message}");
                    }
                }

                oldRecords
                    .ForEach(oldRecord => ExportItems(oldRecord.ToNew(), migratedFolder));

                return $"<color=green>Migrated {oldFiles.Length} old files to {oldRecords.Count} new files.</color>";
            }
            catch (Exception ex)
            {
                string msg = $"<color=red>ERROR: </color>" + ex.Message;
                Debug.LogError(ex.StackTrace);
                return msg;
            }
        }

        private static void ExportItems<TRecord>(TRecord item, string basePath) where TRecord : ConfigTableRecord
        {
            if (item == null) { Debug.LogWarning($"Item null for \"{basePath}\", skipping export."); return; }
            var classType = item.GetType();
            var result = new ImportableJson()
            {
                RecordType = classType.FullName,
                Data = item
            };
            var pathCombined = Path.Combine(basePath, $"{item.Id}.json");
            File.WriteAllText(pathCombined, JsonConvert.SerializeObject(result, JsonExporterSettings.SerializerSettings));
        }

        private static void ExportCustom<T>(T item, string basePath) where T : CustomBaseDescriptor
        {
            if (item == null) { Debug.LogWarning($"Item null for \"{basePath}\", skipping export."); return; }
            var classType = item.GetType();
            var result = new ImportableJson()
            {
                RecordType = classType.FullName,
                Data = item
            };
            var pathCombined = Path.Combine(basePath, $"{item.ItemId}.json");
            File.WriteAllText(pathCombined, JsonConvert.SerializeObject(result, JsonExporterSettings.SerializerSettings));
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