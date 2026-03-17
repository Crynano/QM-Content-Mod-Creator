using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using System.IO;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Services
{
    public static class ModCreator
    {
        public static void CreateExampleMod(string rootPath)
        {
            var meleeWeapon = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<WeaponRecord>(id))
                .First(x => x.IsMelee);

            var rangedWeapon = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<WeaponRecord>(id))
                .First(x => !x.IsMelee);

            var armorItem = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<ArmorRecord>(id))
                .First(x => x != null);

            var rangedWeaponTransform = Data.ItemTransformation.Ids
                .Select(id => Data.ItemTransformation.GetRecord(rangedWeapon.Id))
                .First(x => x != null);

            var rangedWeaponReceipt = Data.ProduceReceipts
                .Find(x => x.OutputItem == rangedWeapon.Id) ?? Data.ProduceReceipts[0];

            var oneDatadisk = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .First(x => x != null);

            var descriptorsItem = CustomWeaponDescriptor.GetExample(rangedWeapon.Id);
            var factionTemplate = FactionTemplate.GetExample(rangedWeapon.Id);
            var localizationItem = LocalizationTemplate.GetExample(rangedWeapon.Id);

            // If everything went right, now create structure

            var assetsFolder = Path.Combine(rootPath, "Assets");

            var weaponsFolder = Path.Combine(assetsFolder, "Weapons");
            var armorFolder = Path.Combine(assetsFolder, "Armors");
            var transformFolder = Path.Combine(assetsFolder, "Transforms");
            var craftingReceiptsFolder = Path.Combine(assetsFolder, "Crafting Recipes");
            var datadiskFolder = Path.Combine(assetsFolder, "Datadisks");

            var descriptorsFolder = Path.Combine(assetsFolder, "Descriptors");
            var localizationFolder = Path.Combine(assetsFolder, "Localization");
            var factionRewardsFolder = Path.Combine(assetsFolder, "FactionRewards");

            var soundFolder = Path.Combine(assetsFolder, "Sounds");
            var bundlesFolder = Path.Combine(assetsFolder, "Bundles");

            Directory.CreateDirectory(assetsFolder);

            Directory.CreateDirectory(weaponsFolder);
            Directory.CreateDirectory(armorFolder);
            Directory.CreateDirectory(transformFolder);
            Directory.CreateDirectory(craftingReceiptsFolder);
            Directory.CreateDirectory(datadiskFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(factionRewardsFolder);

            Directory.CreateDirectory(soundFolder);
            Directory.CreateDirectory(bundlesFolder);

            ExportItems(meleeWeapon, weaponsFolder);
            ExportItems(rangedWeapon, weaponsFolder);
            ExportItems(armorItem, armorFolder);
            ExportItems(oneDatadisk, datadiskFolder);

            ExportCustom(descriptorsItem, $"{rangedWeapon.Id}_descriptor", descriptorsFolder);
            ExportCustom(localizationItem, $"{rangedWeapon.Id}_localization", localizationFolder);
            ExportCustom(factionTemplate, $"{rangedWeapon.Id}_factionReward", factionRewardsFolder);
            ExportCustom(rangedWeaponTransform, $"{rangedWeapon.Id}_transform", transformFolder);
            ExportCustom(rangedWeaponReceipt, $"{rangedWeapon.Id}_craftingReceipt", craftingReceiptsFolder);
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

        private static void ExportCustom<T>(T item, string fileName, string basePath) where T : class, new()
        {
            if (item == null) { Debug.LogWarning($"Item null for \"{basePath}\", skipping export."); return; }
            var classType = item.GetType();
            var result = new ImportableJson()
            {
                RecordType = classType.FullName,
                Data = item
            };
            var pathCombined = Path.Combine(basePath, $"{fileName}.json");
            File.WriteAllText(pathCombined, JsonConvert.SerializeObject(result, JsonExporterSettings.SerializerSettings));
        }
    }
}
