using MGSC;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace QM_ImporterAPI.Services
{
    public static class ModCreator
    {
        private const string ASSETS_FOLDER_NAME = "Assets";

        public static void CreateWeaponMod(string rootPath)
        {
            var rangedWeapon = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<WeaponRecord>(id))
                .Where(x => x != null)
                .FirstOrDefault(x => !x.IsMelee);

            var ammoItem = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<AmmoRecord>(id))
                .FirstOrDefault(x => x != null);

            var fireModeRecord = Data.Firemodes.Ids
                .Select(id => Data.Firemodes.GetRecord(id))
                .FirstOrDefault(x => x != null);

            //var rangedWeaponTransform = Data.ItemTransformation.Ids
            //   .Select(id => Data.ItemTransformation.GetRecord(rangedWeapon.Id))
            //   .First(x => x != null);

            var rangedWeaponReceipt = Data.ProduceReceipts
                .Find(x => x.OutputItem == rangedWeapon.Id) ?? Data.ProduceReceipts[0];

            var oneDatadisk = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .FirstOrDefault(x => x != null);

            var customWeaponDescriptor = CustomWeaponDescriptor.GetExample(rangedWeapon.Id);
            var customAmmoDescriptor = CustomAmmoDescriptor.GetExample(ammoItem.Id);
            var fireModeDescriptor = CustomFireModeDescriptor.GetExample(fireModeRecord.Id);

            var factionTemplate = FactionTemplate.GetExample(rangedWeapon.Id);
            var localizationItem = LocalizationTemplate.GetExample(rangedWeapon.Id);

            // If everything went right, now create structure

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);

            var weaponsFolder = Path.Combine(assetsFolder, "Weapons");
            var armorFolder = Path.Combine(assetsFolder, "Armors");
            var ammoFolder = Path.Combine(assetsFolder, "Ammo");
            var firemodesFolder = Path.Combine(assetsFolder, "Firemodes");

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
            Directory.CreateDirectory(ammoFolder);
            Directory.CreateDirectory(firemodesFolder);

            Directory.CreateDirectory(transformFolder);
            Directory.CreateDirectory(craftingReceiptsFolder);
            Directory.CreateDirectory(datadiskFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(factionRewardsFolder);

            Directory.CreateDirectory(soundFolder);
            Directory.CreateDirectory(bundlesFolder);

            ExportItems(rangedWeapon, weaponsFolder);
            ExportItems(ammoItem, ammoFolder);
            ExportItems(fireModeRecord, firemodesFolder);

            ExportItems(oneDatadisk, datadiskFolder);

            ExportCustomDescriptor(customWeaponDescriptor, descriptorsFolder);
            ExportCustomDescriptor(customAmmoDescriptor, descriptorsFolder);
            ExportCustomDescriptor(fireModeDescriptor, descriptorsFolder);

            ExportCustom(localizationItem, $"{rangedWeapon.Id}_localization", localizationFolder);
            ExportCustom(factionTemplate, $"{rangedWeapon.Id}_factionReward", factionRewardsFolder);
            ExportCustom(rangedWeaponReceipt, $"{rangedWeapon.Id}_craftingReceipt", craftingReceiptsFolder);
        }

        public static void CreateExampleMod(string rootPath)
        {
            var meleeWeapon = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<WeaponRecord>(id))
                .Where(x => x != null)
                .First(x => x.IsMelee);

            var rangedWeapon = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<WeaponRecord>(id))
                .Where(x => x != null)
                .First(x => !x.IsMelee);

            var armorItem = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<ArmorRecord>(id))
                .First(x => x != null);

            var ammoItem = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<AmmoRecord>(id))
                .First(x => x != null);

            var fireModeRecord = Data.Firemodes.Ids
                .Select(id => Data.Firemodes.GetRecord(id))
                .First(x => x != null);

            var explosionRecord = Data.Explosions.Ids
                .Select(id => Data.Explosions.GetRecord(id))
                .First(x => x != null);

            var rangedWeaponReceipt = Data.ProduceReceipts
                .Find(x => x.OutputItem == rangedWeapon.Id) ?? Data.ProduceReceipts[0];

            var oneDatadisk = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .First(x => x != null);

            var consumable = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<ConsumableRecord>(id) ?? null)
                .First(x => x != null);

            var customWeaponDescriptor = CustomWeaponDescriptor.GetExample(rangedWeapon.Id);
            var customAmmoDescriptor = CustomAmmoDescriptor.GetExample(ammoItem.Id);
            var fireModeDescriptor = CustomFireModeDescriptor.GetExample(fireModeRecord.Id);
            var explosionDescriptor = CustomExplosionDescriptor.GetExample(explosionRecord.Id);
            var consumableDescriptor = CustomConsumableDescriptor.GetExample(consumable.Id);
            var datadiskDescriptor = CustomDatadiskDescriptor.GetExample(oneDatadisk.Id);

            var factionTemplate = FactionTemplate.GetExample(rangedWeapon.Id);
            var localizationItem = LocalizationTemplate.GetExample(rangedWeapon.Id);

            // If everything went right, now create structure

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);

            var weaponsFolder = Path.Combine(assetsFolder, "Weapons");
            var armorFolder = Path.Combine(assetsFolder, "Armors");
            var ammoFolder = Path.Combine(assetsFolder, "Ammo");
            var firemodesFolder = Path.Combine(assetsFolder, "Firemodes");
            var explosionsFolder = Path.Combine(assetsFolder, "Explosions");
            var consumablesFolder = Path.Combine(assetsFolder, "Consumables");

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
            Directory.CreateDirectory(ammoFolder);
            Directory.CreateDirectory(firemodesFolder);
            Directory.CreateDirectory(explosionsFolder);
            Directory.CreateDirectory(consumablesFolder);

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
            ExportItems(ammoItem, ammoFolder);
            ExportItems(fireModeRecord, firemodesFolder);
            ExportItems(explosionRecord, explosionsFolder);
            ExportItems(consumable, consumablesFolder);

            ExportItems(armorItem, armorFolder);
            ExportItems(oneDatadisk, datadiskFolder);

            ExportCustomDescriptor(customWeaponDescriptor, descriptorsFolder);
            ExportCustomDescriptor(customAmmoDescriptor, descriptorsFolder);
            ExportCustomDescriptor(fireModeDescriptor, descriptorsFolder);
            ExportCustomDescriptor(explosionDescriptor, descriptorsFolder);
            ExportCustomDescriptor(consumableDescriptor, descriptorsFolder);
            ExportCustomDescriptor(datadiskDescriptor, descriptorsFolder);

            ExportCustom(localizationItem, $"{rangedWeapon.Id}_localization", localizationFolder);
            ExportCustom(factionTemplate, $"{rangedWeapon.Id}_factionReward", factionRewardsFolder);
            ExportCustom(rangedWeaponReceipt, $"{rangedWeapon.Id}_craftingReceipt", craftingReceiptsFolder);

            CreateTraitMod(rootPath);
            CreateTooltipImage(rootPath);
        }

        public static void CreateMercMod(string providedPath)
        {
            var mercenaryClass = Data.MercenaryClasses.Ids
                .Select(id => Data.MercenaryClasses.GetRecord(id))
                .FirstOrDefault(x => x != null);

            if (mercenaryClass == null)
            {
                throw new Exception("No mercenary class found in game data to use as an example.");
            }

            // Search for the datadisk that unlocks that mercenaryClass
            // So in UnlockIds of the datadisk, there is the mercenaryClass.Id

            var mercenaryDatadisk = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .FirstOrDefault(x => x != null && x.UnlockIds != null && x.UnlockIds.Contains(mercenaryClass.Id));

            var mercenaryClassDescriptor = CustomMercenaryClassDescriptor.GetExample(mercenaryClass.Id);
            var datadiskDescriptor = mercenaryDatadisk != null ? CustomDatadiskDescriptor.GetExample(mercenaryDatadisk.Id) : null;
            var localizationItem = LocalizationTemplate.GetExample(mercenaryClass.Id);

            var assetsFolder = Path.Combine(providedPath, ASSETS_FOLDER_NAME);
            var datadiskFolder = Path.Combine(assetsFolder, "Datadisks");

            var mercenaryClassesFolder = Path.Combine(assetsFolder, "MercenaryClasses");
            var descriptorsFolder = Path.Combine(assetsFolder, "Descriptors");
            var localizationFolder = Path.Combine(assetsFolder, "Localization");
            var spritesFolder = Path.Combine(assetsFolder, "Sprites");

            Directory.CreateDirectory(assetsFolder);
            Directory.CreateDirectory(mercenaryClassesFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(spritesFolder);

            ExportItems(mercenaryClass, mercenaryClassesFolder);
            ExportCustomDescriptor(mercenaryClassDescriptor, descriptorsFolder);
            if (mercenaryDatadisk != null)
            {
                Directory.CreateDirectory(datadiskFolder);
                ExportItems(mercenaryDatadisk, datadiskFolder);
                ExportCustomDescriptor(datadiskDescriptor, descriptorsFolder);
            }
            ExportCustom(localizationItem, $"{mercenaryClass.Id}_localization", localizationFolder);
        }

        public static void CreateTooltipImage(string rootPath)
        {
            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);
            var tooltipsFolder = Path.Combine(assetsFolder, "Tooltips");

            Directory.CreateDirectory(assetsFolder);
            Directory.CreateDirectory(tooltipsFolder);

            var testTooltipImage = CustomTooltipImage.GetExample("test_tooltip_image");

            ExportCustom(testTooltipImage, $"{testTooltipImage.Tag}_tooltip", tooltipsFolder);
        }

        public static void CreateTraitMod(string rootPath)
        {
            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);
            var traitsFolder = Path.Combine(assetsFolder, "Traits");

            Directory.CreateDirectory(assetsFolder);
            Directory.CreateDirectory(traitsFolder);

            var traitRecord = Data.ItemTraits.Ids
                .Select(id => Data.ItemTraits.GetRecord(id))
                .First(x => x != null);

            ExportItems(traitRecord, traitsFolder);
        }

        public static void CreateConsumableMod(string rootPath)
        {
            var oneDatadisk = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .First(x => x != null);

            var consumable = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<ConsumableRecord>(id) ?? null)
                .First(x => x != null);

            oneDatadisk.UnlockIds = new List<string> { consumable.Id };

            var consumableReceipt = Data.ProduceReceipts
                .Find(x => x.OutputItem == consumable.Id) ?? Data.ProduceReceipts[0];

            consumableReceipt.OutputItem = consumable.Id;

            var consumableDescriptor = CustomConsumableDescriptor.GetExample(consumable.Id);
            var factionTemplate = FactionTemplate.GetExample(consumable.Id);
            var localizationItem = LocalizationTemplate.GetExample(consumable.Id);

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);

            var consumablesFolder = Path.Combine(assetsFolder, "Consumables");

            var transformFolder = Path.Combine(assetsFolder, "Transforms");
            var craftingReceiptsFolder = Path.Combine(assetsFolder, "Crafting Recipes");
            var datadiskFolder = Path.Combine(assetsFolder, "Datadisks");

            var descriptorsFolder = Path.Combine(assetsFolder, "Descriptors");
            var localizationFolder = Path.Combine(assetsFolder, "Localization");
            var factionRewardsFolder = Path.Combine(assetsFolder, "FactionRewards");

            var soundFolder = Path.Combine(assetsFolder, "Sounds");

            Directory.CreateDirectory(assetsFolder);

            Directory.CreateDirectory(consumablesFolder);

            Directory.CreateDirectory(transformFolder);
            Directory.CreateDirectory(craftingReceiptsFolder);
            Directory.CreateDirectory(datadiskFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(factionRewardsFolder);

            Directory.CreateDirectory(soundFolder);

            ExportItems(consumable, consumablesFolder);
            ExportItems(oneDatadisk, datadiskFolder);
            ExportCustomDescriptor(consumableDescriptor, descriptorsFolder);

            ExportCustom(localizationItem, $"{consumable.Id}_localization", localizationFolder);
            ExportCustom(factionTemplate, $"{consumable.Id}_factionReward", factionRewardsFolder);
            ExportCustom(consumableReceipt, $"{consumable.Id}_craftingReceipt", craftingReceiptsFolder);
        }

        public static void CreateGrenadeMod(string rootPath)
        {
            var oneDatadisk = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .FirstOrDefault(x => x != null);

            var grenade = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<GrenadeRecord>(id) ?? null)
                .FirstOrDefault(x => x != null);

            if (grenade == null)
            {
                throw new Exception("No grenade found in game data to use as an example.");
            }

            if (oneDatadisk != null)
            {
                oneDatadisk.UnlockIds = new List<string> { grenade.Id };
            }

            var grenadeReceipt = Data.ProduceReceipts
                .Find(x => x.OutputItem == grenade.Id) ?? Data.ProduceReceipts[0];

            grenadeReceipt.OutputItem = grenade.Id;

            var grenadeDescriptor = CustomGrenadeDescriptor.GetExample(grenade.Id);
            var factionTemplate = FactionTemplate.GetExample(grenade.Id);
            var localizationItem = LocalizationTemplate.GetExample(grenade.Id);

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);

            var grenadesFolder = Path.Combine(assetsFolder, "Grenades");

            var transformFolder = Path.Combine(assetsFolder, "Transforms");
            var craftingReceiptsFolder = Path.Combine(assetsFolder, "Crafting Recipes");
            var datadiskFolder = Path.Combine(assetsFolder, "Datadisks");

            var descriptorsFolder = Path.Combine(assetsFolder, "Descriptors");
            var localizationFolder = Path.Combine(assetsFolder, "Localization");
            var factionRewardsFolder = Path.Combine(assetsFolder, "FactionRewards");

            var soundFolder = Path.Combine(assetsFolder, "Sounds");
            var spritesFolder = Path.Combine(assetsFolder, "Sprites");

            Directory.CreateDirectory(assetsFolder);

            Directory.CreateDirectory(grenadesFolder);

            Directory.CreateDirectory(transformFolder);
            Directory.CreateDirectory(craftingReceiptsFolder);
            if (oneDatadisk != null)
            {
                Directory.CreateDirectory(datadiskFolder);
            }
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(factionRewardsFolder);

            Directory.CreateDirectory(soundFolder);
            Directory.CreateDirectory(spritesFolder);

            ExportItems(grenade, grenadesFolder);
            if (oneDatadisk != null)
            {
                ExportItems(oneDatadisk, datadiskFolder);
            }
            ExportCustomDescriptor(grenadeDescriptor, descriptorsFolder);

            ExportCustom(localizationItem, $"{grenade.Id}_localization", localizationFolder);
            ExportCustom(factionTemplate, $"{grenade.Id}_factionReward", factionRewardsFolder);
            ExportCustom(grenadeReceipt, $"{grenade.Id}_craftingReceipt", craftingReceiptsFolder);
        }

        private static void ExportItems<TRecord>(TRecord item, string basePath) where TRecord : ConfigTableRecord
        {
            ExportHelper.ExportItem(item, basePath);
        }

        private static void ExportCustomDescriptor<TDesc>(TDesc descriptor, string basePath) where TDesc : CustomBaseDescriptor
        {
            ExportHelper.ExportCustomDescriptor(descriptor, basePath);
        }

        private static void ExportCustom<T>(T item, string fileName, string basePath) where T : class, new()
        {
            ExportHelper.ExportCustom(item, fileName, basePath);
        }
    }
}