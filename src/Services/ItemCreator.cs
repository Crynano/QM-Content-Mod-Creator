using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using System;
using UnityEngine;
using System.Linq;

namespace QM_ImporterAPI.Services
{
    public static class ItemCreator
    {
        private const string MELEE_WEAPON_DESCRIPTOR_KEY = "meleeweapons";
        private const string RANGED_WEAPON_DESCRIPTOR_KEY = "rangeweapons";

        public static ImportOperationResult CreateWeapon(WeaponRecord weapon, CustomWeaponDescriptor weaponDescriptor, string assetFolderPath)
        {
            Logger.LogDebug($"Called {nameof(CreateWeapon)} with ID: " + weapon.Id);
            var operationResult = new ImportOperationResult();

            var weaponPropertiesResult = CheckWeaponPropertiesRestrictions(weapon);
            if (!weaponPropertiesResult.IsSuccess)
            {
                operationResult.AddErrors(weaponPropertiesResult.ErrorMessages);
                return operationResult;
            }

            var descriptorPropertiesResult = SetDescriptorProperties(weapon, weaponDescriptor, assetFolderPath);
            if (!descriptorPropertiesResult.IsSuccess)
            {
                operationResult.AddErrors(descriptorPropertiesResult.ErrorMessages);
                return operationResult;
            }

            AddItemToGame(weapon);
            operationResult.ContentList.Add(weapon.Id);
            Logger.LogDebug("Successfully added weapon with ID: " + weapon.Id);
            return operationResult;
        }

        public static ImportOperationResult AddItemTransformation(ItemTransformationRecord transRecord)
        {
            var operationResult = new ImportOperationResult();
            if (transRecord == null)
            {
                operationResult.AddError("ItemTransformation record is null.");
                return operationResult;
            }

            if (QuasimorphHelper.DoesItemExistInList(transRecord.Id, Data.ItemTransformation))
            {
                Data.ItemTransformation.RemoveRecord(transRecord.Id);
                operationResult.AddError($"Warning: An ItemTransformation with ID: [{transRecord.Id}] was overriden");
            }
            Data.ItemTransformation.AddRecord(transRecord.Id, transRecord);
            return operationResult;
        }

        public static ImportOperationResult AddItemCraftRecipe(ItemProduceReceipt craftingRecipe)
        {
            var operationResult = new ImportOperationResult();

            if (craftingRecipe == null)
            {
                operationResult.AddError("Crafting recipe record is null.");
                return operationResult;
            }

            var foundRecipe = Data.ProduceReceipts.Find(recipe => recipe.OutputItem.Equals(craftingRecipe.OutputItem));
            if (foundRecipe != null)
            {
                Data.ProduceReceipts.Remove(foundRecipe);
                operationResult.AddError($"Warning: A crafting recipe with ID: [{craftingRecipe.OutputItem}] was overriden");
            }
            Data.ProduceReceipts.Add(craftingRecipe);
            return operationResult;
        }

        public static ImportOperationResult AddItemToDatadisk(DatadiskRecord datadisk, WeaponRecord weapon)
        {
            var operationResult = new ImportOperationResult();

            CompositeItemRecord itemRecord = (CompositeItemRecord)MGSC.Data.Items.GetRecord(datadisk.Id);
            if (itemRecord != null)
            {
                DatadiskRecord dataChip = itemRecord.GetRecord<DatadiskRecord>();
                var addOnlyThoseNotInChip = datadisk.UnlockIds.FindAll(id => !dataChip.UnlockIds.Contains(id));
                dataChip.UnlockIds.AddRange(addOnlyThoseNotInChip);
            }
            else
            {
                operationResult.AddError("Adding new datadisks with custom content is not currently supported. Only updating existing datadisks is supported.");
                //ItemContentDescriptor descriptor = GetDescriptor<CustomItemContentDescriptor>(datadisk.Id).GetOriginal<ItemContentDescriptor>();
                //DatadiskRecord diskRecord = datadiskRecord.GetOriginal();
                //diskRecord.ContentDescriptor = descriptor;
                //AddItemToGame(diskRecord, "datadisks");
            }

            return operationResult;
        }

        public static ImportOperationResult AddFactionRewards(FactionTemplate factionTemplate)
        {
            var operationResult = new ImportOperationResult();
            foreach (FactionReward factionReward in factionTemplate.FactionRewardList)
            {
                foreach (ContentDropRecord contentRecord in factionReward.contentRecords)
                {
                    contentRecord.ContentIds
                        .Where(contentId => !QuasimorphHelper.IsGameId(contentId))
                        .ToList()
                        .ForEach(x => operationResult.AddWarning($"Not adding {x} to faction table. Weapon does not exist."));

                    contentRecord.ContentIds = contentRecord.ContentIds
                        .Where(contentId => QuasimorphHelper.IsGameId(contentId))
                        .ToList();

                    if (contentRecord.ContentIds.Count > 0)
                    {
                        Data.FactionDrop.AddRecord(factionReward.GetTableName(), contentRecord);
                    }
                }
            }
            return operationResult;
        }

        private static void AddItemToGame(WeaponRecord weapon)
        {
            if (QuasimorphHelper.DoesItemExistInList(weapon.Id, Data.Items))
            {
                Data.Items.RemoveRecord(weapon.Id);
                Logger.LogWarning("An item with ID: [" + weapon.Id + "] was OVERRIDEN!!!");
            }

            string key = (weapon.IsMelee ? MELEE_WEAPON_DESCRIPTOR_KEY : RANGED_WEAPON_DESCRIPTOR_KEY);

            Data.Descriptors[key].AddDescriptor(weapon.Id, weapon.ItemDesc);
            Data.Items.AddRecord(weapon.Id, weapon);
        }

        private static void AddItemToGame<TRecord>(TRecord item, string key) where TRecord : ItemRecord
        {
            if (QuasimorphHelper.DoesItemExistInList(item.Id, Data.Items))
            {
                Data.Items.RemoveRecord(item.Id);
                Logger.LogWarning("An item with ID: [" + item.Id + "] was OVERRIDEN!!!");
            }

            Data.Descriptors[key].AddDescriptor(item.Id, item.ItemDesc);
            Data.Items.AddRecord(item.Id, item);
        }

        private static ImportOperationResult SetDescriptorProperties(WeaponRecord weapon, CustomWeaponDescriptor customWeaponDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            var weaponDescriptor = ScriptableObject.CreateInstance<WeaponDescriptor>();

            var prefabResult = LoadPrefab(customWeaponDescriptor, assetFolderPath);
            if (!prefabResult.IsSuccess)
            {
                operationResult.AddErrors(prefabResult.ErrorMessages);
                return operationResult;
            }
            weaponDescriptor._prefab = prefabResult.Result;

            var muzzleResult = LoadMuzzle(customWeaponDescriptor, prefabResult.Result, assetFolderPath);
            if (!muzzleResult.IsSuccess)
            {
                operationResult.AddErrors(muzzleResult.ErrorMessages);
                return operationResult;
            }
            weaponDescriptor._muzzles = new Muzzle[1] { muzzleResult.Result };

            LoadSprites(ref weaponDescriptor, customWeaponDescriptor, assetFolderPath);

            var textureResult = LoadTexture(customWeaponDescriptor, assetFolderPath);
            operationResult.AddWarnings(textureResult.ErrorMessages);
            weaponDescriptor._texture = textureResult.Result;

            var soundOpResult = ConfigureSounds(weaponDescriptor, customWeaponDescriptor, assetFolderPath);
            operationResult.AddErrors(soundOpResult.ErrorMessages);

            weaponDescriptor._grip = customWeaponDescriptor.Grip;
            weaponDescriptor._hasHFGOverlay = customWeaponDescriptor.HasHFGOverlay;

            weapon.ContentDescriptor = weaponDescriptor;
            return operationResult;
        }

        private static void LoadSprites(ref WeaponDescriptor weaponDescriptor, CustomWeaponDescriptor customWeaponDescriptor, string assetFolderPath)
        {
            var imageProps = customWeaponDescriptor.ImageProperties;

            weaponDescriptor._icon = LoadSprite(assetFolderPath, imageProps.IconSpriteIdOrPath, "Icon", AssetImporter.LoadNewSprite);
            weaponDescriptor._smallIcon = LoadSprite(assetFolderPath, imageProps.SmallIconSpriteIdOrPath, "SmallIcon", AssetImporter.LoadCenteredSprite);
            weaponDescriptor._shadow = LoadSprite(assetFolderPath, imageProps.ShadowOnFloorSpriteIdOrPath, "Shadow", AssetImporter.LoadCenteredSprite);
        }

        private static Sprite LoadSprite(string assetFolderPath, string path, string propertyName, Func<string, Sprite> loadFunc)
        {
            if (QuasimorphHelper.IsGameId(path))
            {
                var propertyFromItem = QuasimorphHelper.GetPropertyFromItem<WeaponDescriptor>(path, propertyName);
                if (propertyFromItem is Sprite spriteProperty)
                {
                    return QuasimorphHelper.CloneSprite(spriteProperty);
                }
                Logger.LogWarning("Failed to load sprite for property [" + propertyName + "] from existing game item with ID: " + path + ". The property is either missing or not a Sprite.");
            }
            var fullPath = Helper.ResolvePath(assetFolderPath, path);
            return loadFunc(fullPath);
        }

        private static ImportOperationResult<GameObject> LoadPrefab(CustomWeaponDescriptor customWeaponDescriptor, string assetFolderPath)
        {
            var result = new ImportOperationResult<GameObject>();

            if (QuasimorphHelper.IsGameId(customWeaponDescriptor.ModelProperties.PrefabId))
            {
                var prefabFromWeapon = QuasimorphHelper.GetPrefabFromExistingWeapon(customWeaponDescriptor.ModelProperties.PrefabId);
                result.SetResult(prefabFromWeapon);
                return result;
            }

            var bundlePath = Helper.ResolvePath(assetFolderPath, customWeaponDescriptor.ModelProperties.AssetBundlePath);
            var prefabOperationResult = AssetImporter.LoadFileFromBundle<GameObject>(bundlePath, customWeaponDescriptor.ModelProperties.PrefabId);
            if (!prefabOperationResult.IsSuccess)
            {
                result.AddErrors(prefabOperationResult.ErrorMessages);
                return result;
            }

            var prefab = prefabOperationResult.Result;
            ApplyScaleToPrefab(prefab, customWeaponDescriptor.ModelProperties.PrefabScale);
            result.SetResult(prefab);
            return result;
        }

        private static void ApplyScaleToPrefab(GameObject prefab, float scaleValue)
        {
            ItemBone itemBone = prefab?.GetComponent<ItemBone>();
            if (itemBone != null)
            {
                itemBone.Scale = new Vector3(scaleValue, scaleValue, scaleValue);
            }
        }

        private static ImportOperationResult<Texture> LoadTexture(CustomWeaponDescriptor customWeaponDescriptor, string assetFolderPath)
        {
            var assetBundlePath = Helper.ResolvePath(assetFolderPath, customWeaponDescriptor.ModelProperties.AssetBundlePath);

            return LoadAssetFromBundleOrGame<Texture>(
                customWeaponDescriptor.ModelProperties.TextureIdOrPath,
                assetBundlePath,
                QuasimorphHelper.GetTextureFromExistingWeapon);
        }

        private static ImportOperationResult<Muzzle> LoadMuzzle(CustomWeaponDescriptor customWeaponDescriptor, GameObject prefab, string assetFolderPath)
        {
            var modelProperties = customWeaponDescriptor.ModelProperties;
            var muzzleResult = new ImportOperationResult<Muzzle>();

            if (QuasimorphHelper.IsGameId(modelProperties.MuzzleId))
            {
                var muzzle = QuasimorphHelper.GetMuzzleFromExistingWeapon(modelProperties.MuzzleId);
                muzzleResult.SetResult(muzzle);
            }
            else
            {
                var defaultMuzzleResult = LoadDefaultMuzzle(prefab);
                if (!defaultMuzzleResult.IsSuccess)
                {
                    muzzleResult.AddErrors(defaultMuzzleResult.ErrorMessages);
                }
                else
                {
                    muzzleResult.SetResult(defaultMuzzleResult.Result);
                }
            }

            return muzzleResult;
        }

        private static ImportOperationResult<T> LoadAssetFromBundleOrGame<T>(string assetName, string bundlePath, Func<string, T> getFromGameFunc) where T : UnityEngine.Object
        {
            var result = new ImportOperationResult<T>();

            if (QuasimorphHelper.IsGameId(assetName))
            {
                var asset = getFromGameFunc(assetName);
                result.SetResult(asset);
                return result;
            }

            var loadResult = AssetImporter.LoadFileFromBundle<T>(bundlePath, assetName);
            if (!loadResult.IsSuccess)
            {
                result.AddErrors(loadResult.ErrorMessages);
            }
            else
            {
                result.SetResult(loadResult.Result);
            }

            return result;
        }

        private static ImportOperationResult ConfigureSounds(WeaponDescriptor weaponDescriptor, CustomWeaponDescriptor customWeaponDescriptor, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult();
            try
            {
                SetSounds(ref weaponDescriptor._attackSoundBanks, customWeaponDescriptor.AudioProperties.ShootSoundIdOrPath, 0, assetFolderPath);
                SetSounds(ref weaponDescriptor._dryShotSoundBanks, customWeaponDescriptor.AudioProperties.DryShotSoundIdOrPath, 1, assetFolderPath);
                SetSounds(ref weaponDescriptor._failedAttackSoundBanks, customWeaponDescriptor.AudioProperties.FailedAttackSoundIdOrPath, 2, assetFolderPath);
                SetSounds(ref weaponDescriptor._reloadSoundBanks, customWeaponDescriptor.AudioProperties.ReloadSoundIdOrPath, 3, assetFolderPath);
            }
            catch (Exception ex)
            {
                operationResult.AddError("An error occurred while setting up sounds." + ex.Message);
            }
            return operationResult;
        }

        private static ImportOperationResult CheckWeaponPropertiesRestrictions(WeaponRecord weaponRecord)
        {
            var operationResult = new ImportOperationResult();

            if (string.IsNullOrEmpty(weaponRecord.Id))
            {
                operationResult.AddError("Weapon won't load, ID is empty.");
            }

            if (!Data.Items._records.ContainsKey(weaponRecord.DefaultAmmoId))
            {
                operationResult.AddError("Weapon won't load, ammunition \"" + weaponRecord.DefaultAmmoId + "\" does not exist.");
            }

            if (weaponRecord.Firemodes.Count == 0)
            {
                operationResult.AddError("Weapon won't load, it needs atleast a firemode.");
            }
            else if (weaponRecord.Firemodes.Count > 2)
            {
                operationResult.AddError("Weapon won't load, game limits firemodes to 2.");
            }
            else
            {
                if (string.IsNullOrEmpty(weaponRecord.Firemodes[0]))
                {
                    operationResult.AddError("Weapon won't load, firemode 1 is invalid");
                }
                else if (!Data.Firemodes._records.ContainsKey(weaponRecord.Firemodes[0]))
                {
                    operationResult.AddError("Weapon won't load, fireMode \"" + weaponRecord.Firemodes[0] + "\" does not exist in-game.");
                }

                if (weaponRecord.Firemodes.Count == 2)
                {
                    if (string.IsNullOrEmpty(weaponRecord.Firemodes[1]))
                    {
                        operationResult.AddError("Weapon won't load, firemode 2 is invalid");
                    }
                    else if (!Data.Firemodes._records.ContainsKey(weaponRecord.Firemodes[1]))
                    {
                        operationResult.AddError("Weapon won't load, fireMode \"" + weaponRecord.Firemodes[1] + "\" does not exist in-game.");
                    }
                }
            }

            return operationResult;
        }

        private static void SetSounds(ref SoundBank[] soundBank, string soundPath, int category, string assetFolderPath, bool fallbackToDefault = false)
        {
            if (soundBank == null)
            {
                soundBank = new SoundBank[1];
                soundBank[0] = ScriptableObject.CreateInstance(typeof(SoundBank)) as SoundBank;
                soundBank[0]._clips = new AudioClip[1];
            }

            if (QuasimorphHelper.IsGameId(soundPath))
            {
                SoundBank[] audiosFromExistingWeapons = QuasimorphHelper.GetAudiosFromExistingWeapons(soundPath, category, false);
                if (audiosFromExistingWeapons != null)
                {
                    soundBank = audiosFromExistingWeapons;
                }
            }
            else
            {
                var soundFullPath = Helper.ResolvePath(assetFolderPath, soundPath);
                var importAudioResult = AssetImporter.ImportAudio(soundFullPath);
                if (importAudioResult.IsSuccess)
                {
                    soundBank[0]._clips[0] = importAudioResult.Result;
                }
            }

        }

        private static ImportOperationResult<Muzzle> LoadDefaultMuzzle(GameObject parentGO)
        {
            var result = new ImportOperationResult<Muzzle>();
            if (parentGO == null) return null;

            var muzzleTransform = parentGO.transform.Find("Muzzle");
            var muzzle = muzzleTransform.gameObject.GetComponent<Muzzle>() ?? muzzleTransform.gameObject.AddComponent<Muzzle>();
            muzzle._additLightIntencityMult = 0.5f;

            AnimationCurve val2 = new AnimationCurve()
            {
                keys = new Keyframe[3]
                {
                    new Keyframe(0f, 0f),
                    new Keyframe(0.05f, 0.5f),
                    new Keyframe(0.1f, 0f)
                }
            };
            muzzle._muzzleIntensityCurve = val2;

            result.SetResult(muzzle);
            return result;
        }
    }
}