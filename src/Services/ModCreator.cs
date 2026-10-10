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
        private const string SPRITES_FOLDER_NAME = "Sprites";
        private const string RECIPES_FOLDER_NAME = "Crafting Recipes";
        private const string DESCRIPTORS_FOLDER_NAME = "Descriptors";
        private const string LOCALIZATION_FOLDER_NAME = "Localization";
        private const string FACTIONREWARDS_FOLDER_NAME = "FactionRewards";
        private const string DATADISKS_FOLDER_NAME = "Datadisks";
        private const string SOUNDS_FOLDER_NAME = "Sounds";
        private const string BUNDLE_FOLDER_NAME = "Bundles";
        private const string WEAPONS_FOLDER_NAME = "Weapons";
        private const string FIREMODES_FOLDER_NAME = "Firemodes";
        private const string AMMO_FOLDER_NAME = "Ammo";
        private const string ARMORS_FOLDER_NAME = "Armors";
        private const string AUGMENTATIONS_FOLDER_NAME = "Augmentations";
        private const string TRANSFORMS_FOLDER_NAME = "Transforms";
        private const string CONSUMABLES_FOLDER_NAME = "Consumables";
        private const string IMPLANTS_FOLDER_NAME = "Implants";
        private const string HELMETS_FOLDER_NAME = "Helmets";

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

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);

            var weaponsFolder = Path.Combine(assetsFolder, WEAPONS_FOLDER_NAME);
            var armorFolder = Path.Combine(assetsFolder, ARMORS_FOLDER_NAME);
            var ammoFolder = Path.Combine(assetsFolder, AMMO_FOLDER_NAME);
            var firemodesFolder = Path.Combine(assetsFolder, FIREMODES_FOLDER_NAME);

            var transformFolder = Path.Combine(assetsFolder, TRANSFORMS_FOLDER_NAME);
            var craftingReceiptsFolder = Path.Combine(assetsFolder, RECIPES_FOLDER_NAME);
            var datadiskFolder = Path.Combine(assetsFolder, DATADISKS_FOLDER_NAME);

            var descriptorsFolder = Path.Combine(assetsFolder, DESCRIPTORS_FOLDER_NAME);
            var localizationFolder = Path.Combine(assetsFolder, LOCALIZATION_FOLDER_NAME);
            var factionRewardsFolder = Path.Combine(assetsFolder, FACTIONREWARDS_FOLDER_NAME);

            var soundFolder = Path.Combine(assetsFolder, SOUNDS_FOLDER_NAME);
            var bundlesFolder = Path.Combine(assetsFolder, BUNDLE_FOLDER_NAME);

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

            ExportHelper.ExportItem(rangedWeapon, weaponsFolder);
            ExportHelper.ExportItem(ammoItem, ammoFolder);
            ExportHelper.ExportItem(fireModeRecord, firemodesFolder);

            ExportHelper.ExportItem(oneDatadisk, datadiskFolder);

            ExportHelper.ExportCustomDescriptor(customWeaponDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(customAmmoDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(fireModeDescriptor, descriptorsFolder);

            ExportHelper.ExportCustom(localizationItem, $"{rangedWeapon.Id}_localization", localizationFolder);
            ExportHelper.ExportCustom(factionTemplate, $"{rangedWeapon.Id}_factionReward", factionRewardsFolder);
            ExportHelper.ExportCustom(rangedWeaponReceipt, $"{rangedWeapon.Id}_craftingReceipt", craftingReceiptsFolder);
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

            var weaponsFolder = Path.Combine(assetsFolder, WEAPONS_FOLDER_NAME);
            var armorFolder = Path.Combine(assetsFolder, ARMORS_FOLDER_NAME);
            var ammoFolder = Path.Combine(assetsFolder, AMMO_FOLDER_NAME);
            var firemodesFolder = Path.Combine(assetsFolder, FIREMODES_FOLDER_NAME);
            var explosionsFolder = Path.Combine(assetsFolder, "Explosions");
            var consumablesFolder = Path.Combine(assetsFolder, CONSUMABLES_FOLDER_NAME);

            var transformFolder = Path.Combine(assetsFolder, TRANSFORMS_FOLDER_NAME);
            var craftingReceiptsFolder = Path.Combine(assetsFolder, RECIPES_FOLDER_NAME);
            var datadiskFolder = Path.Combine(assetsFolder, DATADISKS_FOLDER_NAME);

            var descriptorsFolder = Path.Combine(assetsFolder, DESCRIPTORS_FOLDER_NAME);
            var localizationFolder = Path.Combine(assetsFolder, LOCALIZATION_FOLDER_NAME);
            var factionRewardsFolder = Path.Combine(assetsFolder, FACTIONREWARDS_FOLDER_NAME);

            var soundFolder = Path.Combine(assetsFolder, SOUNDS_FOLDER_NAME);
            var bundlesFolder = Path.Combine(assetsFolder, BUNDLE_FOLDER_NAME);

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

            ExportHelper.ExportItem(meleeWeapon, weaponsFolder);
            ExportHelper.ExportItem(rangedWeapon, weaponsFolder);
            ExportHelper.ExportItem(ammoItem, ammoFolder);
            ExportHelper.ExportItem(fireModeRecord, firemodesFolder);
            ExportHelper.ExportItem(explosionRecord, explosionsFolder);
            ExportHelper.ExportItem(consumable, consumablesFolder);

            ExportHelper.ExportItem(armorItem, armorFolder);
            ExportHelper.ExportItem(oneDatadisk, datadiskFolder);

            ExportHelper.ExportCustomDescriptor(customWeaponDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(customAmmoDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(fireModeDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(explosionDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(consumableDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(datadiskDescriptor, descriptorsFolder);

            ExportHelper.ExportCustom(localizationItem, $"{rangedWeapon.Id}_localization", localizationFolder);
            ExportHelper.ExportCustom(factionTemplate, $"{rangedWeapon.Id}_factionReward", factionRewardsFolder);
            ExportHelper.ExportCustom(rangedWeaponReceipt, $"{rangedWeapon.Id}_craftingReceipt", craftingReceiptsFolder);

            CreateTraitMod(rootPath);
            CreateTooltipImage(rootPath);
        }

        public static void CreateMercMod(string providedPath)
        {
            var mercenaryClass = Data.MercenaryClasses.Ids
                .Select(id => Data.MercenaryClasses.GetRecord(id))
                .FirstOrDefault(x => x != null);

            var mercenaryDatadisk = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .FirstOrDefault(x => x != null && x.UnlockIds != null && x.UnlockIds.Contains(mercenaryClass.Id));

            var mercenaryProfile = Data.MercenaryProfiles.Ids
                .Select(id => Data.MercenaryProfiles.GetRecord(id))
                .Where(x => x != null)
                .ElementAtOrDefault(5);

            var mercenaryDatadiskProfile = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .FirstOrDefault(x => x != null && x.UnlockIds != null && x.UnlockIds.Contains(mercenaryProfile.Id));

            var mercenaryClassDescriptor = CustomMercenaryClassDescriptor.GetExample(mercenaryClass.Id);
            var datadiskDescriptor = mercenaryDatadisk != null ? CustomDatadiskDescriptor.GetExample(mercenaryDatadisk.Id) : null;
            var datadiskProfileDescriptor = mercenaryDatadiskProfile != null ? CustomDatadiskDescriptor.GetExample(mercenaryDatadiskProfile.Id) : null;
            var localizationItem = LocalizationTemplate.GetExample(mercenaryClass.Id, "class");
            var locForProfile = LocalizationTemplate.GetExample(mercenaryProfile.Id, "spec");

            var assetsFolder = Path.Combine(providedPath, ASSETS_FOLDER_NAME);
            var datadiskFolder = Path.Combine(assetsFolder, DATADISKS_FOLDER_NAME);

            var mercenaryClassesFolder = Path.Combine(assetsFolder, "MercenaryClasses");
            var mercenaryProfilesFolder = Path.Combine(assetsFolder, "MercenaryProfiles");
            var descriptorsFolder = Path.Combine(assetsFolder, DESCRIPTORS_FOLDER_NAME);
            var localizationFolder = Path.Combine(assetsFolder, LOCALIZATION_FOLDER_NAME);
            var spritesFolder = Path.Combine(assetsFolder, SPRITES_FOLDER_NAME);

            Directory.CreateDirectory(assetsFolder);
            Directory.CreateDirectory(mercenaryClassesFolder);
            Directory.CreateDirectory(mercenaryProfilesFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(spritesFolder);
            Directory.CreateDirectory(datadiskFolder);

            ExportHelper.ExportItem(mercenaryClass, mercenaryClassesFolder);
            ExportHelper.ExportItem(mercenaryProfile, mercenaryProfilesFolder);
            ExportHelper.ExportCustomDescriptor(mercenaryClassDescriptor, descriptorsFolder);

            if (mercenaryDatadisk != null)
            {
                ExportHelper.ExportItem(mercenaryDatadisk, datadiskFolder);
                ExportHelper.ExportCustomDescriptor(datadiskDescriptor, descriptorsFolder);
            }

            if (mercenaryDatadiskProfile != null)
            {
                ExportHelper.ExportItem(mercenaryDatadiskProfile, datadiskFolder);
                ExportHelper.ExportCustomDescriptor(datadiskProfileDescriptor, descriptorsFolder);
            }

            ExportHelper.ExportCustom(localizationItem, $"{mercenaryClass.Id}_localization", localizationFolder);
            ExportHelper.ExportCustom(locForProfile, $"{mercenaryProfile.Id}_localization", localizationFolder);

            ExportHelper.CreateVoidFile("92x92_Icon_Sprite", spritesFolder);
            ExportHelper.CreateVoidFile("24x24_SmallIcon_Sprite", spritesFolder);
        }

        public static void CreateTooltipImage(string rootPath)
        {
            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);
            var tooltipsFolder = Path.Combine(assetsFolder, "Tooltips");

            Directory.CreateDirectory(assetsFolder);
            Directory.CreateDirectory(tooltipsFolder);

            var testTooltipImage = CustomTooltipImage.GetExample("test_tooltip_image");

            ExportHelper.ExportCustom(testTooltipImage, $"{testTooltipImage.Tag}_tooltip", tooltipsFolder);
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

            ExportHelper.ExportItem(traitRecord, traitsFolder);
        }

        public static void CreateConsumableMod(string rootPath)
        {
            var oneDatadisk = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<DatadiskRecord>(id) ?? null)
                .First(x => x != null);

            var consumable = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<ConsumableRecord>(id) ?? null)
                .First(x => x != null);

            var trash = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<TrashRecord>(id) ?? null)
                .First(x => x != null);

            oneDatadisk.UnlockIds = new List<string> { consumable.Id };

            var consumableReceipt = Data.ProduceReceipts
                .Find(x => x.OutputItem == consumable.Id) ?? Data.ProduceReceipts[0];

            consumableReceipt.OutputItem = consumable.Id;

            var consumableDescriptor = CustomConsumableDescriptor.GetExample(consumable.Id);
            var trashDescriptor = CustomTrashDescriptor.GetExample(trash.Id);
            var factionTemplate = FactionTemplate.GetExample(consumable.Id);
            var localizationItem = LocalizationTemplate.GetExample(consumable.Id);

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);

            var consumablesFolder = Path.Combine(assetsFolder, CONSUMABLES_FOLDER_NAME);
            var trashFolder = Path.Combine(assetsFolder, "Trash");

            var transformFolder = Path.Combine(assetsFolder, TRANSFORMS_FOLDER_NAME);
            var craftingReceiptsFolder = Path.Combine(assetsFolder, RECIPES_FOLDER_NAME);
            var datadiskFolder = Path.Combine(assetsFolder, DATADISKS_FOLDER_NAME);

            var descriptorsFolder = Path.Combine(assetsFolder, DESCRIPTORS_FOLDER_NAME);
            var localizationFolder = Path.Combine(assetsFolder, LOCALIZATION_FOLDER_NAME);
            var factionRewardsFolder = Path.Combine(assetsFolder, FACTIONREWARDS_FOLDER_NAME);

            var soundFolder = Path.Combine(assetsFolder, SOUNDS_FOLDER_NAME);

            Directory.CreateDirectory(assetsFolder);

            Directory.CreateDirectory(trashFolder);
            Directory.CreateDirectory(consumablesFolder);

            Directory.CreateDirectory(transformFolder);
            Directory.CreateDirectory(craftingReceiptsFolder);
            Directory.CreateDirectory(datadiskFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(factionRewardsFolder);

            Directory.CreateDirectory(soundFolder);

            ExportHelper.ExportItem(trash, trashFolder);
            ExportHelper.ExportItem(consumable, consumablesFolder);
            ExportHelper.ExportItem(oneDatadisk, datadiskFolder);

            ExportHelper.ExportCustomDescriptor(consumableDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(trashDescriptor, descriptorsFolder);

            ExportHelper.ExportCustom(localizationItem, $"{consumable.Id}_localization", localizationFolder);
            ExportHelper.ExportCustom(factionTemplate, $"{consumable.Id}_factionReward", factionRewardsFolder);
            ExportHelper.ExportCustom(consumableReceipt, $"{consumable.Id}_craftingReceipt", craftingReceiptsFolder);
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

            var transformFolder = Path.Combine(assetsFolder, TRANSFORMS_FOLDER_NAME);
            var craftingReceiptsFolder = Path.Combine(assetsFolder, RECIPES_FOLDER_NAME);
            var datadiskFolder = Path.Combine(assetsFolder, DATADISKS_FOLDER_NAME);

            var descriptorsFolder = Path.Combine(assetsFolder, DESCRIPTORS_FOLDER_NAME);
            var localizationFolder = Path.Combine(assetsFolder, LOCALIZATION_FOLDER_NAME);
            var factionRewardsFolder = Path.Combine(assetsFolder, FACTIONREWARDS_FOLDER_NAME);

            var soundFolder = Path.Combine(assetsFolder, SOUNDS_FOLDER_NAME);
            var spritesFolder = Path.Combine(assetsFolder, SPRITES_FOLDER_NAME);

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

            ExportHelper.ExportItem(grenade, grenadesFolder);
            if (oneDatadisk != null)
            {
                ExportHelper.ExportItem(oneDatadisk, datadiskFolder);
            }
            ExportHelper.ExportCustomDescriptor(grenadeDescriptor, descriptorsFolder);

            ExportHelper.ExportCustom(localizationItem, $"{grenade.Id}_localization", localizationFolder);
            ExportHelper.ExportCustom(factionTemplate, $"{grenade.Id}_factionReward", factionRewardsFolder);
            ExportHelper.ExportCustom(grenadeReceipt, $"{grenade.Id}_craftingReceipt", craftingReceiptsFolder);
        }

        public static void CreateAugmentationMod(string rootPath)
        {
            var augmentation = Data.Items.GetRecord("spider_claw") as CompositeItemRecord 
                ?? throw new InvalidDataException("Augmentation record 'spider_claw' not found in game data.");

            foreach (var rec in augmentation.Records)
            {
                Logger.LogDebug($"Record: {rec.GetType()} - {rec.Id}");
            }

            var augmentationWeapon = augmentation.GetRecord<WeaponRecord>();
            augmentationWeapon.Id = $"*{augmentationWeapon.Id}";
            var augmentationRecord = augmentation.GetRecord<AugmentationRecord>();
            augmentationRecord.Id = $"*{augmentationRecord.Id}";

            var augmentationDescriptor = CustomAugmentationDescriptor.GetExample(augmentation.Id + "_aug");
            var augmentationWeaponDescriptor = CustomWeaponDescriptor.GetExample(augmentation.Id + "_weap");

            var localizationItem = LocalizationTemplate.GetExample(augmentation.Id);
            var weaponLocalizationItem = LocalizationTemplate.GetExample(augmentationWeapon.Id);

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);
            var weaponsFolder = Path.Combine(assetsFolder, WEAPONS_FOLDER_NAME);
            var augmentationsFolder = Path.Combine(assetsFolder, AUGMENTATIONS_FOLDER_NAME);
            var descriptorsFolder = Path.Combine(assetsFolder, DESCRIPTORS_FOLDER_NAME);
            var localizationFolder = Path.Combine(assetsFolder, LOCALIZATION_FOLDER_NAME);
            var spritesFolder = Path.Combine(assetsFolder, SPRITES_FOLDER_NAME);

            Directory.CreateDirectory(augmentationsFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(spritesFolder);
            Directory.CreateDirectory(weaponsFolder);

            ExportHelper.ExportItem(augmentationWeapon, weaponsFolder);
            ExportHelper.ExportItem(augmentationRecord, augmentationsFolder);

            ExportHelper.ExportCustomDescriptor(augmentationDescriptor, descriptorsFolder);
            ExportHelper.ExportCustomDescriptor(augmentationWeaponDescriptor, descriptorsFolder);

            ExportHelper.ExportCustom(localizationItem, $"{augmentation.Id.TrimId()}_localization", localizationFolder);
            ExportHelper.ExportCustom(weaponLocalizationItem, $"{augmentationWeapon.Id.TrimId()}_localization", localizationFolder);
        }

        public static void CreateHelmetMod(string rootPath)
        {
            var helmet = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<HelmetRecord>(id))
                .First(x => x != null);

            var helmetDescriptor = CustomHelmetDescriptor.GetExample(helmet.Id);
            var localizationItem = LocalizationTemplate.GetExample(helmet.Id);

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);
            var helmetsFolder = Path.Combine(assetsFolder, HELMETS_FOLDER_NAME);
            var descriptorsFolder = Path.Combine(assetsFolder, DESCRIPTORS_FOLDER_NAME);
            var localizationFolder = Path.Combine(assetsFolder, LOCALIZATION_FOLDER_NAME);
            var spritesFolder = Path.Combine(assetsFolder, SPRITES_FOLDER_NAME);
            var modelsFolder = Path.Combine(assetsFolder, "Models");

            Directory.CreateDirectory(helmetsFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(spritesFolder);
            Directory.CreateDirectory(modelsFolder);

            ExportHelper.ExportItem(helmet, helmetsFolder);
            ExportHelper.ExportCustomDescriptor(helmetDescriptor, descriptorsFolder);
            ExportHelper.ExportCustom(localizationItem, $"{helmet.Id}_localization", localizationFolder);

            File.WriteAllText(Path.Combine(rootPath, "guide.txt"), HELMET_GUIDE);
        }

        private const string HELMET_GUIDE =
@"HELMET MOD GUIDE
===============

1. UNIQUE ArmorPart PER HELMET
   The game picks the helmet model by prefab name. The prefab name is the
   'ArmorPart' value of the helmet descriptor. If two helmets share the same
   ArmorPart (the default is 'Head'), the last one loaded overwrites the others
   and every item shows the same model.
   -> Give every helmet a unique ArmorPart (e.g. Helmet_1, Helmet_2, ...).
   -> A warning is logged when a prefab name is replaced.

2. BLENDER EXPORT (OBJ)
   - Blender forward is Y and up is Z. When exporting, Forward Axis must be set to -Z and Up Axis to Y.
   - Export as .obj (only .obj is supported).
   - Enable: UV Coordinates, Normals.
   - Disable: Write Materials (the .mtl is ignored).
   - Triangulated Mesh is optional (quads and n-gons are fan-triangulated).
   - Apply Modifiers: on.
   - Make sure the mesh has a UV map. Faces without UVs all sample texel (0,0).
   - Only the active UV map is exported.
   - Importing an FBX into Blender first and exporting as OBJ is fine.

3. TEXTURE
   - Use a PNG or JPG. The path (TextureIdOrPath) is relative to the mod's assets folder.
   - Unwrap UVs against the exact image you ship. Keep UVs inside 0-1
     (the texture uses Clamp wrapping and Point filtering).
   - Several models can share the same texture file.
   - If the file is missing a warning is logged and the model renders white.

4. DESCRIPTOR REQUIREMENTS
   - PrefabPath: path to the .obj file.
   - Part.ArmorType and Part.ArmorPart must both be set when using PrefabPath.

5. TROUBLESHOOTING
   - Model is white: the texture did not reach the descriptor (missing file or
     wrong TextureIdOrPath). Check the log.
   - All helmets look the same: duplicate ArmorPart (see 1).
   - Helmet not offered to an actor: see the warnings logged by the loader.
";

        public static void CreateImplantMod(string rootPath)
        {
            var implant = Data.Items.Ids
                .Select(id => Data.Items.GetSimpleRecord<ImplantRecord>(id))
                .First(x => x != null);

            var implantDescriptor = CustomImplantDescriptor.GetExample(implant.Id);
            var localizationItem = LocalizationTemplate.GetExample(implant.Id);

            var assetsFolder = Path.Combine(rootPath, ASSETS_FOLDER_NAME);
            var implantsFolder = Path.Combine(assetsFolder, IMPLANTS_FOLDER_NAME);
            var descriptorsFolder = Path.Combine(assetsFolder, DESCRIPTORS_FOLDER_NAME);
            var localizationFolder = Path.Combine(assetsFolder, LOCALIZATION_FOLDER_NAME);
            var spritesFolder = Path.Combine(assetsFolder, SPRITES_FOLDER_NAME);
            var soundFolder = Path.Combine(assetsFolder, SOUNDS_FOLDER_NAME);

            Directory.CreateDirectory(implantsFolder);
            Directory.CreateDirectory(descriptorsFolder);
            Directory.CreateDirectory(localizationFolder);
            Directory.CreateDirectory(spritesFolder);
            Directory.CreateDirectory(soundFolder);

            ExportHelper.ExportItem(implant, implantsFolder);
            ExportHelper.ExportCustomDescriptor(implantDescriptor, descriptorsFolder);
            ExportHelper.ExportCustom(localizationItem, $"{implant.Id}_localization", localizationFolder);
        }
    }
}