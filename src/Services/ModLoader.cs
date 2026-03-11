using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QM_ImporterAPI.Services
{
    public class ModLoader
    {
        private const string ASSETS_FOLDER_NAME = "Assets";

        private List<ImportableJson> importableJsons = new List<ImportableJson>();

        public void LoadModFromContext(IModContext modContext)
        {
            LoadModFromDirectory(modContext.ModContentPath);
        }

        public void LoadModFromDirectory(string givenPath)
        {
            var operationResult = new ImportOperationResult();
            // Search for the ASSET folder in the given path
            // Search for all the json assets in the subfolders.
            // Maybe add a specific name for the Folders?
            // Load the json assets and add them to the game
            // Report all added and unadded items to the game.
            if (!Directory.Exists(givenPath))
            {
                operationResult.AddError($"The given path '{givenPath}' does not exist.");
                return;
            }

            // Because it exists, we can search for the ASSET folder
            var assetFolderPath = Path.Combine(givenPath, ASSETS_FOLDER_NAME);
            if (!Directory.Exists(assetFolderPath))
            {
                operationResult.AddError($"The ASSET folder was not found in the given path '{givenPath}'.");
            }

            var jsonFiles = Directory.GetFiles(assetFolderPath, "*.json", SearchOption.AllDirectories);
            LoadImportableJsons(jsonFiles);
            ProcessImportableJsons(assetFolderPath);
        }

        private void LoadImportableJsons(string[] jsonFilesPath)
        {
            // We know the file exists because we found it in the previous step, so we can try to load it
            // Parse json into ImportableJson. Store it into a list of ImportableJsons. Then we can process the list and add the items to the game.
            foreach (var jsonFile in jsonFilesPath)
            {
                var json = File.ReadAllText(jsonFile);
                var importableJson = JsonConvert.DeserializeObject<ImportableJson>(json, JsonExporterSettings.DeserializerSettings);
                if (importableJson != null)
                {
                    importableJsons.Add(importableJson);
                }
            }
        }

        private void ProcessImportableJsons(string assetFolderPath)
        {
            // We have two major types of JSONs. Records and Descriptors.
            // Descriptors have to be parsed before the record.
            // If no descriptor is found, we SKIP the record and report an error.
            // To know the descriptor type, we need to parse the RecordType into a type and check its inheritance.
            // If it inherits from ItemRecordDescriptor, then we know it's a descriptor.
            // If it inherits from ConfigTableRecord, then we know it's a record.
            var deserializedImportableJsons = importableJsons
                .Select(json => json.Deserialize())
                .ToList();

            var descriptors = deserializedImportableJsons
                .Where(obj => obj.GetType().IsSubclassOf(typeof(CustomItemContentDescriptor)))
                .ToList();

            var weaponDescriptors = descriptors
                .OfType<CustomWeaponDescriptor>()
                .ToList();

            var records = deserializedImportableJsons
               .Where(obj => obj.GetType().IsSubclassOf(typeof(ConfigTableRecord)))
               .ToList();

            var datadisks = deserializedImportableJsons
                .OfType<DatadiskRecord>()
                .ToList();

            var transformationRecords = deserializedImportableJsons
                .OfType<ItemTransformationRecord>()
                .ToList();

            var craftingRecords = deserializedImportableJsons
                .OfType<ItemProduceReceipt>()
                .ToList();

            var factionRecords = deserializedImportableJsons
                .OfType<FactionTemplate>()
                .ToList();

            var localizationFiles = deserializedImportableJsons
                .OfType<LocalizationTemplate>()
                .ToList();

            // After parsing the records, we shoud determine which specific Type are, so we can add them to the game. We can do this by checking their inheritance as well.
            // So we should filter by weapons, armors and consumablers, and add them to the game with the corresponding method.
            var weaponRecords = records
                .OfType<WeaponRecord>()
                .ToList();

            foreach (var descriptor in weaponDescriptors)
            {
                var weaponRecord = weaponRecords.First(x => x.Id.Equals(descriptor.ItemId));
                if (weaponRecord != null)
                {
                    UnityEngine.Debug.Log($"Trying to add weapon '{weaponRecord.Id}' (with descriptor) to the game!");
                    var opResult = ItemCreator.CreateWeapon(weaponRecord, descriptor, assetFolderPath);
                    UnityEngine.Debug.LogWarning(opResult.Print());

                    if (!opResult.IsSuccess) continue;

                    // Add to datadisks if the weapon is unlockable by any of them
                    datadisks
                        .Where(dd => dd.UnlockIds.Contains(weaponRecord.Id))
                        .ToList()
                        .ForEach(dataDisk => ItemCreator.AddItemToDatadisk(dataDisk, weaponRecord));

                    // Add destroy recipe
                    var transformationRecord = transformationRecords
                        .First(tr => tr.Id.Equals(weaponRecord.Id));

                    ItemCreator.AddItemTransformation(transformationRecord);

                    // Add craft and upgrade recipe
                    var craftingRecord = craftingRecords.First(cr => cr.OutputItem.Equals(weaponRecord.Id));
                    ItemCreator.AddItemCraftRecipe(craftingRecord);
                    UnityEngine.Debug.Log($"Weapon '{weaponRecord.Id}' added to the game successfully!");
                }
            }

            factionRecords.ForEach(faction => ItemCreator.AddFactionRewards(faction));
            localizationFiles.ForEach(loc => QuasimorphHelper.AddLocalization(loc));

            //var equipableArmorRecords = records
            //    .Where(record => record.GetType().IsSubclassOf(typeof(ResistRecord)))
            //    .ToList();

            //var armorRecords = equipableArmorRecords
            //    .OfType<ArmorRecord>()
            //    .ToList();

            // Let's see how many records we have to debug lawl.
            //Console.WriteLine($"Found {weaponRecords.Count} weapon records and {armorRecords.Count} armor records.");
            UnityEngine.Debug.Log($"Found {weaponRecords.Count} weapon records and {descriptors.Count} descriptors records.");
        }
    }
}
