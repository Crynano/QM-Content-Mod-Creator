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
using UnityEngine;

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
                .Where(json => json != null);

            var descriptors = deserializedImportableJsons
                .Where(obj => obj.GetType().IsSubclassOf(typeof(CustomBaseDescriptor)));

            var weaponDescriptors = descriptors
                .OfType<CustomWeaponDescriptor>();

            var armorDescriptors = descriptors
                .OfType<CustomArmorDescriptor>();

            var ammoDescriptors = descriptors
                .OfType<CustomAmmoDescriptor>();

            var firemodeDescriptors = descriptors
                .OfType<CustomFireModeDescriptor>();

            var explosionDescriptors = descriptors
                .OfType<CustomExplosionDescriptor>();

            var datadiskDescriptors = descriptors
                .OfType<CustomDatadiskDescriptor>();

            var consumableDescriptors = descriptors
                .OfType<CustomConsumableDescriptor>();

            // Game Records
            var records = deserializedImportableJsons
               .Where(obj => obj.GetType().IsSubclassOf(typeof(ConfigTableRecord)));

            var datadisks = records
                .OfType<DatadiskRecord>();

            var transformationRecords = records
                .OfType<ItemTransformationRecord>();

            var craftingRecords = records
                .OfType<ItemProduceReceipt>();

            var factionRecords = deserializedImportableJsons
                .OfType<FactionTemplate>();

            var localizationFiles = deserializedImportableJsons
                .OfType<LocalizationTemplate>();

            var weaponRecords = records
                .OfType<WeaponRecord>();

            var armorRecords = records
                .Where(obj => obj.GetType().IsSubclassOf(typeof(ResistRecord)));

            var ammoRecords = records
                .OfType<AmmoRecord>();

            var consumableRecords = records
                .OfType<ConsumableRecord>();

            var firemodeRecords = records
                .OfType<FireModeRecord>();

            var explosionRecords = records
                .OfType<ExplosionRecord>();

            var traits = records
                .OfType<ItemTraitRecord>();

            var cumulativeOperation = new ImportOperationResult();

            var traitLoadResult = LoadTraits(traits);
            cumulativeOperation.Absorb(traitLoadResult);

            var firemodeLoadResult = LoadFiremodes(assetFolderPath, firemodeRecords, firemodeDescriptors);
            cumulativeOperation.Absorb(firemodeLoadResult);

            var explosionLoadResult = LoadExplosions(assetFolderPath, explosionRecords, explosionDescriptors);
            cumulativeOperation.Absorb(explosionLoadResult);

            var ammoResult = LoadAmmo(assetFolderPath, ammoRecords, ammoDescriptors);
            cumulativeOperation.Absorb(ammoResult);

            var weaponsLoadResult = LoadWeapons(assetFolderPath, weaponRecords, weaponDescriptors);
            cumulativeOperation.Absorb(weaponsLoadResult);

            var armorLoadResult = LoadArmors(assetFolderPath, armorRecords, armorDescriptors);
            cumulativeOperation.Absorb(armorLoadResult);

            var consumablesLoadResult = LoadConsumables(assetFolderPath, consumableRecords, consumableDescriptors);
            cumulativeOperation.Absorb(consumablesLoadResult);

            var dataDiskResult = LoadDatadisks(assetFolderPath, datadisks, datadiskDescriptors);
            cumulativeOperation.Absorb(dataDiskResult);

            var craftsLoadResult = AddCrafts(transformationRecords, craftingRecords);
            cumulativeOperation.Absorb(craftsLoadResult);

            var factionRewardsLoadResult = LoadFactionRewards(factionRecords);
            cumulativeOperation.Absorb(factionRewardsLoadResult);

            foreach (var locFile in localizationFiles)
            {
                QuasimorphHelper.AddLocalization(locFile);
            }

            stopWatch.Stop();
            cumulativeOperation.SetExecutionTime(stopWatch.ElapsedMilliseconds);
            Logger.LogInfo("Import Operation Result: \n" + cumulativeOperation.Print());
        }

        private static ImportOperationResult LoadExplosions(string assetFolderPath, IEnumerable<ExplosionRecord> explosionRecords, IEnumerable<CustomExplosionDescriptor> explosionDescriptors)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadExplosions)}: Found {explosionRecords.Count()} records and {explosionDescriptors.Count()} descriptors.");

            foreach (var descriptor in explosionDescriptors)
            {
                var explosionRecord = explosionRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (explosionRecord != null)
                {
                    Logger.LogDebug($"Trying to add {nameof(ExplosionRecord)} '{explosionRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.AddExplosion(explosionRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find an explosion record with id '{descriptor.ItemId}' for the explosion descriptor. Skipping this explosion.");
                }
            }

            return operationResult;
        }

        private static ImportOperationResult LoadFiremodes(string assetFolderPath, IEnumerable<FireModeRecord> firemodeRecords, IEnumerable<CustomFireModeDescriptor> fireModeDescriptors)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadFiremodes)}: Found {firemodeRecords.Count()} records and {fireModeDescriptors.Count()} descriptors.");

            foreach (var descriptor in fireModeDescriptors)
            {
                var firemodeRecord = firemodeRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (firemodeRecord != null)
                {
                    Logger.LogDebug($"Trying to add firemode '{firemodeRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.AddFireMode(firemodeRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find a firemode record with id '{descriptor.ItemId}' for the firemode descriptor. Skipping this firemode.");
                }
            }
            return operationResult;
        }

        private static ImportOperationResult AddCrafts(IEnumerable<ItemTransformationRecord> transformationRecords, IEnumerable<ItemProduceReceipt> craftingRecords)
        {
            var operationResult = new ImportOperationResult();

            foreach (var transformationRecord in transformationRecords)
            {
                var result = ItemCreator.AddItemTransformation(transformationRecord);
                operationResult.Absorb(result);
            }

            foreach (var craftingRecord in craftingRecords)
            {
                var result = ItemCreator.AddItemCraftRecipe(craftingRecord);
                operationResult.Absorb(result);
            }

            return operationResult;
        }

        private static ImportOperationResult LoadWeapons(string assetFolderPath, IEnumerable<WeaponRecord> weaponRecords, IEnumerable<CustomWeaponDescriptor> weaponDescriptors)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadWeapons)}: Found {weaponRecords.Count()} records and {weaponDescriptors.Count()} descriptors.");
            // We should be able to add a weapon even if we don't have a descriptor for it, but only if the ID matches an existing record,
            // Otherwise we should skip it as no descriptor is a crash.

            foreach (var descriptor in weaponDescriptors)
            {
                var weaponRecord = weaponRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (weaponRecord != null)
                {
                    Logger.LogDebug($"Trying to add weapon '{weaponRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.CreateWeapon(weaponRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find a weapon record with id '{descriptor.ItemId}' for the weapon descriptor. Skipping this weapon.");
                }
            }

            // For all weapon records that don't have a descriptor, we will try to add them with default values, but only if they have a valid ID that matches the record.
            var weaponRecordsWithoutDescriptor = weaponRecords
                .Where(wr => !weaponDescriptors.Any(d => d.ItemId.Equals(wr.Id)))
                .ToList();

            foreach (var weaponRecord in weaponRecordsWithoutDescriptor)
            {
                Logger.LogDebug($"Trying to add weapon '{weaponRecord.Id}' (without descriptor) to the game!");
                var opResult = ItemCreator.ReplaceWeapon(weaponRecord, assetFolderPath);
                operationResult.Absorb(opResult);
            }

            return operationResult;
        }

        private static ImportOperationResult LoadArmors(string assetFolderPath, IEnumerable<object> equipmentRecords, IEnumerable<CustomArmorDescriptor> equipmentDescriptors)
        {
            var operationResult = new ImportOperationResult();

            if (!equipmentRecords.Any())
            {
                Logger.LogDebug("No armor records found to load.");
                return operationResult;
            }
            Logger.LogDebug($"Found {equipmentRecords.Count()} equipment records and {equipmentDescriptors.Count()} descriptors.");

            // Filter all armor records.
            var armorRecords = equipmentRecords
                .OfType<ArmorRecord>();

            var bootsRecords = equipmentRecords
                .OfType<BootsRecord>();

            var helmetRecords = equipmentRecords
                .OfType<HelmetRecord>();

            var leggingRecords = equipmentRecords
                .OfType<LeggingsRecord>();

            // And some other shit

            return operationResult;
        }

        private static ImportOperationResult LoadAmmo(string assetFolderPath, IEnumerable<AmmoRecord> ammoRecords, IEnumerable<CustomAmmoDescriptor> ammoDescriptors)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadAmmo)}: Found {ammoRecords.Count()} records and {ammoDescriptors.Count()} descriptors.");
            foreach (var descriptor in ammoDescriptors)
            {
                var ammoRecord = ammoRecords.FirstOrDefault(x => x.Id.Equals(descriptor.ItemId));
                if (ammoRecord != null)
                {
                    Logger.LogDebug($"Trying to add ammo '{ammoRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.AddAmmo(ammoRecord, descriptor, assetFolderPath);
                    operationResult.Absorb(opResult);
                }
                else
                {
                    operationResult.AddWarning($"Could not find an ammo record with id '{descriptor.ItemId}' for the ammo descriptor. Skipping this ammo.");
                }
            }
            return operationResult;
        }

        private static ImportOperationResult LoadDatadisks(string assetFolderPath, IEnumerable<DatadiskRecord> datadisks, IEnumerable<CustomDatadiskDescriptor> customDatadiskDescriptor)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadDatadisks)}: Found {datadisks.Count()} records and {customDatadiskDescriptor.Count()} descriptors.");
            foreach (var singleDataDisk in datadisks)
            {
                var descriptor = customDatadiskDescriptor.FirstOrDefault(x => x.ItemId.Equals(singleDataDisk.Id));
                var opResult = ItemCreator.AddDatadiskItems(singleDataDisk, descriptor, assetFolderPath);
                operationResult.Absorb(opResult);
            }
            return operationResult;
        }

        private static ImportOperationResult LoadConsumables(string assetFolderPath, IEnumerable<ConsumableRecord> consumableRecords, IEnumerable<CustomConsumableDescriptor> customConsumableDescriptors)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadConsumables)}: Found {consumableRecords.Count()} records and {customConsumableDescriptors.Count()} descriptors.");
            foreach (var consumable in consumableRecords)
            {
                var descriptor = customConsumableDescriptors.FirstOrDefault(x => x.ItemId.Equals(consumable.Id));
                var opResult = ItemCreator.AddConsumable(consumable, descriptor, assetFolderPath);
                operationResult.Absorb(opResult);
            }
            return operationResult;
        }

        private static ImportOperationResult LoadCraftingRecipt(IEnumerable<ItemProduceReceipt> craftingRecords)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadCraftingRecipt)}: Found {craftingRecords.Count()} records.");
            foreach (var craftingRecord in craftingRecords)
            {
                var result = ItemCreator.AddItemCraftRecipe(craftingRecord);
                operationResult.Absorb(result);
            }
            return operationResult;
        }

        private static ImportOperationResult LoadFactionRewards(IEnumerable<FactionTemplate> factions)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadFactionRewards)}: Found {factions.Count()} records.");
            foreach (var faction in factions)
            {
                var result = ItemCreator.AddFactionRewards(faction);
                operationResult.Absorb(result);
            }
            return operationResult;
        }

        private static ImportOperationResult LoadTraits(IEnumerable<ItemTraitRecord> traitRecords)
        {
            var operationResult = new ImportOperationResult();
            Logger.LogDebug($"{nameof(LoadTraits)}: Found {traitRecords.Count()} records.");
            foreach (var traitRecord in traitRecords)
            {
                var result = ItemCreator.AddTrait(traitRecord);
                operationResult.Absorb(result);
            }
            return operationResult;
        }
    }
}