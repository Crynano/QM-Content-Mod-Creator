using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Services.Loaders;
using QM_ImporterAPI.Templates;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace QM_ImporterAPI.Services
{
    public class ModLoader
    {
        private const string ASSETS_FOLDER_NAME = "Assets";

        private List<ImportableJson> ImportableJsons = new List<ImportableJson>();

        /// <summary>
        /// Static collection of all item loaders. Initialized once and reused across all ModLoader instances.
        /// Loaders are executed in this order to respect item dependencies.
        /// </summary>
        private static readonly List<BaseItemLoader> Loaders = new List<BaseItemLoader>
        {
            new TraitLoader(),           // Load traits first (no dependencies)
            new FireModeLoader(),        // Fire modes before weapons
            new ExplosionLoader(),       // Explosions before weapons/ammo
            new AmmoLoader(),            // Ammo before weapons
            new WeaponLoader(),          // Weapons depend on traits, fire modes, ammo
            new ConsumableLoader(),      // Consumables
            new DatadiskLoader(),        // Datadisks
            new CraftingLoader(),        // Crafting recipes (may reference items above)
            new FactionRewardsLoader(),  // Faction rewards (may reference items)
            new LocalizationLoader()     // Localization last (labels for all items)
        };

        public void LoadModFromContext(IModContext modContext)
        {
            LoadModFromDirectory(modContext.ModContentPath);
        }

        public void LoadModFromDirectory(string givenPath)
        {
            // Here we could try to get the mod name from the directory name or a config file
            // The file is modmanifest.json, which is the steam standard for it.
            // The structure is 
            // Parse file modmanifest.json if it exists
            var modManifestPath = Path.Combine(givenPath, "modmanifest.json");
            if (File.Exists(modManifestPath))
            {
                var modManifest = JsonConvert.DeserializeObject<UserMod>(File.ReadAllText(modManifestPath));
                if (modManifest != null)
                    Logger.LogInfo($"Loading mod '{modManifest.UniqueModName}' from directory '{givenPath}'");
            }

            Logger.LogDebug($"{nameof(LoadModFromDirectory)}: Starting mod loading process");
            if (!Directory.Exists(givenPath))
            {
                Logger.LogError($"The given path '{givenPath}' does not exist. Please provide a valid path to the mod folder.");
                return;
            }

            var assetFolderPath = Path.Combine(givenPath, ASSETS_FOLDER_NAME);
            if (!Directory.Exists(assetFolderPath))
            {
                Logger.LogError($"Missing 'Assets' folder in {givenPath}");
                return;
            }

            var jsonFiles = Directory.GetFiles(assetFolderPath, "*.json", SearchOption.AllDirectories);
            Logger.LogDebug($"Found {jsonFiles.Length} json files in the ASSET folder. Starting to load them...");

            LoadImportableJsons(jsonFiles);
            ProcessImportableJsons(assetFolderPath);

            Logger.LogDebug($"Finished loading mod from directory '{givenPath}'");
        }

        internal static void UpdateMod(string givenPath)
        {
            // Idea is to read the whole mod and then reprint them again with same values
            // So any new properties added to the templates will be added to the files without changing existing values
            // And also the new ordering structure will be applied to the files
            Logger.LogInfo($"{nameof(UpdateMod)}: Starting mod reprint process");
            var stopwatch = Stopwatch.StartNew();
            if (!Directory.Exists(givenPath))
            {
                Logger.LogError($"The given path '{givenPath}' does not exist. Please provide a valid path to the mod folder.");
                return;
            }

            var assetFolderPath = Path.Combine(givenPath, ASSETS_FOLDER_NAME);
            if (!Directory.Exists(assetFolderPath))
            {
                Logger.LogError($"Missing 'Assets' folder in {givenPath}");
                return;
            }

            var jsonFiles = Directory.GetFiles(assetFolderPath, "*.json", SearchOption.AllDirectories);
            var importedFilesDictionary = new Dictionary<string, object>();
            foreach (var jsonFile in jsonFiles)
            {
                var json = File.ReadAllText(jsonFile);
                var importableJson = JsonConvert.DeserializeObject<ImportableJson>(json, JsonExporterSettings.DeserializerSettings);
                if (importableJson != null && !string.IsNullOrEmpty(importableJson.RecordType))
                {
                    importedFilesDictionary.Add(jsonFile, importableJson.Deserialize());
                }
            }

            Logger.LogInfo($"Loaded {importedFilesDictionary.Count}/{jsonFiles.Length} json files. ");
            foreach (var pathFileDictionary in importedFilesDictionary)
            {
                ExportHelper.ExportCustom(pathFileDictionary.Key, pathFileDictionary.Value);
            }

            stopwatch.Stop();
            Logger.LogInfo($"Finished reprinting json files in: {givenPath}. Duration {stopwatch.ElapsedMilliseconds}ms");
        }

        private void LoadImportableJsons(string[] jsonFilesPath)
        {
            Logger.LogDebug($"{nameof(LoadImportableJsons)}: Loading importable JSONs");
            var importJsonStopwatch = Stopwatch.StartNew();

            foreach (var jsonFile in jsonFilesPath)
            {
                var json = File.ReadAllText(jsonFile);
                var importableJson = JsonConvert.DeserializeObject<ImportableJson>(json, JsonExporterSettings.DeserializerSettings);
                if (importableJson != null && !string.IsNullOrEmpty(importableJson.RecordType))
                {
                    ImportableJsons.Add(importableJson);
                }
            }
            importJsonStopwatch.Stop();
            Logger.LogDebug($"Finished loading json files in: {importJsonStopwatch.ElapsedMilliseconds}ms. Starting to process them...");
        }

        private void ProcessImportableJsons(string assetFolderPath)
        {
            Logger.LogDebug($"{nameof(ProcessImportableJsons)}: Processing mod JSONs");
            var stopWatch = Stopwatch.StartNew();

            var deserializedImportableJsons = ImportableJsons
                .Select(json => json.Deserialize())
                .Where(json => json != null)
                .ToList();

            var cumulativeOperation = new ImportOperationResult();

            // Execute each loader using the static loader collection
            foreach (var loader in Loaders)
            {
                var result = loader.Load(deserializedImportableJsons, assetFolderPath);
                cumulativeOperation.Absorb(result);
            }

            stopWatch.Stop();
            cumulativeOperation.SetExecutionTime(stopWatch.ElapsedMilliseconds);
            Logger.LogInfo("Import Operation Result: \n" + cumulativeOperation.Print());
        }
    }
}