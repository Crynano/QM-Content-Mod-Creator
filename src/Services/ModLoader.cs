using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
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

        public void LoadModFromContext(IModContext modContext)
        {
            LoadModFromDirectory(modContext.ModContentPath);
        }

        public void LoadModFromDirectory(string givenPath)
        {
            var stopWatch = Stopwatch.StartNew();
            // Search for the ASSET folder in the given path
            // Search for all the json assets in the subfolders.
            // Maybe add a specific name for the Folders?
            // Load the json assets and add them to the game
            // Report all added and unadded items to the game.
            Logger.LogInfo("Starting mod loading process...");
            if (!Directory.Exists(givenPath))
            {
                Logger.LogError($"The given path '{givenPath}' does not exist. Please provide a valid path to the mod folder.");
                return;
            }

            // Because it exists, we can search for the ASSET folder
            var assetFolderPath = Path.Combine(givenPath, ASSETS_FOLDER_NAME);
            if (!Directory.Exists(assetFolderPath))
            {
                Logger.LogError($"Missing 'Assets' folder in {givenPath}");
                return;
            }

            var jsonFiles = Directory.GetFiles(assetFolderPath, "*.json", SearchOption.AllDirectories);
            Logger.LogInfo($"Found {jsonFiles.Length} json files in the ASSET folder. Starting to load them...");

            LoadImportableJsons(jsonFiles);
            ProcessImportableJsons(assetFolderPath);
            Logger.LogInfo("Finished mod loading process!");
            
            stopWatch.Stop();
            Logger.LogInfo($"Mod Loading Process: {stopWatch.ElapsedMilliseconds}ms");
        }

        private void LoadImportableJsons(string[] jsonFilesPath)
        {
            // We know the file exists because we found it in the previous step, so we can try to load it
            // Parse json into ImportableJson. Store it into a list of ImportableJsons. Then we can process the list and add the items to the game.
            foreach (var jsonFile in jsonFilesPath)
            {
                var json = File.ReadAllText(jsonFile);
                var importableJson = JsonConvert.DeserializeObject<ImportableJson>(json, JsonExporterSettings.DeserializerSettings);
                if (importableJson != null && !string.IsNullOrEmpty(importableJson.RecordType))
                {
                    ImportableJsons.Add(importableJson);
                }
            }
        }

        private void ProcessImportableJsons(string assetFolderPath)
        {
            Logger.LogDebug("Processing importable jsons...");
            var deserializedImportableJsons = ImportableJsons
                .Select(json => json.Deserialize())
                .Where(json => json != null)
                .ToList();

            var descriptors = deserializedImportableJsons
                .Where(obj => obj.GetType().IsSubclassOf(typeof(CustomItemContentDescriptor)))
                .ToList();

            Logger.LogDebug($"Found {descriptors.Count} descriptors in the imported jsons.");
            var weaponDescriptors = descriptors
                .OfType<CustomWeaponDescriptor>()
                .ToList();

            var armorDescriptors = descriptors
                .OfType<CustomArmorDescriptor>()
                .ToList();

            Logger.LogDebug($"Found {weaponDescriptors.Count} weapon descriptors in the imported jsons.");

            // Game Records
            var records = deserializedImportableJsons
               .Where(obj => obj.GetType().IsSubclassOf(typeof(ConfigTableRecord)))
               .ToList();

            Logger.LogDebug($"Filtering records by type from {records.Count} records");

            var datadisks = records
                .OfType<DatadiskRecord>()
                .ToList();

            var transformationRecords = records
                .OfType<ItemTransformationRecord>()
                .ToList();

            var craftingRecords = records
                .OfType<ItemProduceReceipt>()
                .ToList();

            var factionRecords = deserializedImportableJsons
                .OfType<FactionTemplate>()
                .ToList();

            var localizationFiles = deserializedImportableJsons
                .OfType<LocalizationTemplate>()
                .ToList();

            var weaponRecords = records
                .OfType<WeaponRecord>()
                .ToList();

            var armorRecords = records
                .Where(obj => obj.GetType().IsSubclassOf(typeof(ResistRecord)))
                .ToList();

            var cumulativeOperation = new ImportOperationResult();

            var weaponsLoadResult = LoadWeapons(assetFolderPath, weaponRecords, weaponDescriptors, datadisks);
            cumulativeOperation.AddErrors(weaponsLoadResult.ErrorMessages);

            var armorLoadResult = LoadArmors(assetFolderPath, armorRecords, armorDescriptors, datadisks);
            cumulativeOperation.AddErrors(armorLoadResult.ErrorMessages);

            var craftsLoadResult = AddCrafts(transformationRecords, craftingRecords);
            cumulativeOperation.AddErrors(craftsLoadResult.ErrorMessages);

            factionRecords.ForEach(faction =>
            {
                var opResult = ItemCreator.AddFactionRewards(faction);
                cumulativeOperation.AddErrors(opResult.ErrorMessages);
            });

            localizationFiles.ForEach(loc => QuasimorphHelper.AddLocalization(loc));

            Logger.LogWarning(cumulativeOperation.Print());
        }

        private static ImportOperationResult AddCrafts(List<ItemTransformationRecord> transformationRecords, List<ItemProduceReceipt> craftingRecords)
        {
            var operationResult = new ImportOperationResult();

            transformationRecords.ForEach(transformationRecord =>
            {
                var result = ItemCreator.AddItemTransformation(transformationRecord);
                operationResult.AddErrors(result.ErrorMessages);
            });

            craftingRecords.ForEach(craftingRecord =>
            {
                var result = ItemCreator.AddItemCraftRecipe(craftingRecord);
                operationResult.AddErrors(result.ErrorMessages);
            });

            return operationResult;
        }


        private static ImportOperationResult LoadWeapons(string assetFolderPath, List<WeaponRecord> weaponRecords, List<CustomWeaponDescriptor> weaponDescriptors, List<DatadiskRecord> datadisks)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"Found {weaponRecords.Count} records and {weaponDescriptors.Count} descriptors.");
            foreach (var descriptor in weaponDescriptors)
            {
                var weaponRecord = weaponRecords.First(x => x.Id.Equals(descriptor.ItemId));
                if (weaponRecord != null)
                {
                    Logger.LogDebug($"Trying to add weapon '{weaponRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.CreateWeapon(weaponRecord, descriptor, assetFolderPath);

                    if (!opResult.IsSuccess)
                    {
                        operationResult.AddErrors(opResult.ErrorMessages);
                        continue;
                    }

                    datadisks
                        .Where(dd => dd.UnlockIds.Contains(weaponRecord.Id))
                        .ToList()
                        .ForEach(dataDisk => ItemCreator.AddItemToDatadisk(dataDisk, weaponRecord));

                    Logger.LogDebug($"Weapon '{weaponRecord.Id}' added to the game successfully!");
                }
            }

            return operationResult;
        }

        private static ImportOperationResult LoadArmors(string assetFolderPath, List<object> equipmentRecords, List<CustomArmorDescriptor> equipmentDescriptors, List<DatadiskRecord> datadisks)
        {
            var operationResult = new ImportOperationResult();

            if (equipmentRecords.Count == 0)
            {
                Logger.LogDebug("No armor records found to load.");
                return operationResult;
            }
            Logger.LogDebug($"Found {equipmentRecords.Count} equipment records and {equipmentDescriptors.Count} descriptors.");

            // Filter all armor records.
            var armorRecords = equipmentRecords
                .OfType<ArmorRecord>()
                .ToList();

            var bootsRecords = equipmentRecords
                .OfType<BootsRecord>()
                .ToList();

            var helmetRecords = equipmentRecords
                .OfType<HelmetRecord>()
                .ToList();

            var leggingRecords = equipmentRecords
                .OfType<LeggingsRecord>()
                .ToList();

            // And some other shit

            return operationResult;
        }
    }
}
