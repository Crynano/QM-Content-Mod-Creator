using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Extensions.Descriptors;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates.Descriptors;
using System;
using System.Linq;
using UnityEngine;

namespace QM_ImporterAPI.Services.Extensions.Records
{
    internal static class WeaponRecordExtensions
    {
        internal static ImportOperationResult SetDescriptorProperties(this WeaponRecord weapon, CustomWeaponDescriptor customWeaponDescriptor, string assetFolderPath)
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

            var muzzleResult = LoadMuzzle(customWeaponDescriptor, prefabResult.Result);
            operationResult.Absorb(muzzleResult);
            if (!muzzleResult.IsSuccess)
            {
                return operationResult;
            }
            weaponDescriptor._muzzles = new Muzzle[1] { muzzleResult.Result };

            weaponDescriptor.LoadSprites(customWeaponDescriptor, assetFolderPath);

            var textureResult = LoadTexture(customWeaponDescriptor, assetFolderPath);
            operationResult.AddWarnings(textureResult.ErrorMessages);
            weaponDescriptor._texture = textureResult.Result;

            var soundOpResult = LoadWeaponSounds(weaponDescriptor, customWeaponDescriptor, assetFolderPath);
            operationResult.Absorb(soundOpResult);

            weaponDescriptor._grip = customWeaponDescriptor.Grip;
            weaponDescriptor._hasHFGOverlay = customWeaponDescriptor.HasHFGOverlay;

            weapon.ContentDescriptor = weaponDescriptor;
            return operationResult;
        }

        #region Weapon Stuff
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

        private static ImportOperationResult<Muzzle> LoadMuzzle(CustomWeaponDescriptor customWeaponDescriptor, GameObject prefab)
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
                muzzleResult.AddWarning($"Muzzle ID \"{modelProperties.MuzzleId}\" is not a valid game ID. Attempting to load default muzzle.");
                var defaultMuzzleResult = LoadDefaultMuzzle(prefab);
                muzzleResult.Absorb(defaultMuzzleResult);
                muzzleResult.SetResult(defaultMuzzleResult.Result);
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

        private static ImportOperationResult LoadWeaponSounds(WeaponDescriptor weaponDescriptor, CustomWeaponDescriptor customWeaponDescriptor, string assetFolderPath)
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

        private static void SetSounds(ref SoundBank[] soundBank, string soundPath, int category, string assetFolderPath)
        {
            if (soundBank == null)
            {
                soundBank = new SoundBank[1];
                soundBank[0] = ScriptableObject.CreateInstance(typeof(SoundBank)) as SoundBank;
                soundBank[0]._clips = new AudioClip[1];
            }

            if (QuasimorphHelper.IsGameId(soundPath))
            {
                SoundBank[] audiosFromExistingWeapons = QuasimorphHelper.GetAudiosFromExistingWeapons(soundPath, category);
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
            if (parentGO is null)
            {
                result.AddError($"Prefab for muzzle is null!");
                return result;
            }

            var muzzleTransform = parentGO.transform.Find("Muzzle") ?? parentGO.transform.GetComponentInChildren<Muzzle>()?.transform;
            if (muzzleTransform is null)
            {
                result.AddWarning($"Prefab for muzzle is missing a child named \"Muzzle\". Added a default one in prefab root.");
                muzzleTransform = new GameObject("Muzzle").transform;
                muzzleTransform.SetParent(parentGO.transform);
                muzzleTransform.localPosition = Vector3.zero;

                var itemBone = muzzleTransform.gameObject.AddComponent<ItemBone>();
                itemBone.TargetBoneId = "Muzzle";
            }

            var muzzle = muzzleTransform.gameObject.GetComponent<Muzzle>();
            if (muzzle is null)
            {
                result.AddWarning($"Prefab for muzzle is missing a Muzzle component. Added a default one.");
                muzzle = muzzleTransform.gameObject.AddComponent<Muzzle>();
            }

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
        #endregion
        internal static ImportOperationResult CheckWeaponPropertiesRestrictions(this WeaponRecord weaponRecord)
        {
            var operationResult = new ImportOperationResult();

            if (string.IsNullOrEmpty(weaponRecord.Id))
            {
                operationResult.AddError($"Invalid weapon, ID is empty.");
            }

            if (!Data.Items.Ids.Contains(weaponRecord.DefaultAmmoId))
            {
                operationResult.AddError($"Invalid weapon \"{weaponRecord.Id}\", ammunition \"{weaponRecord.DefaultAmmoId}\" does not exist.");
            }

            if (weaponRecord.Firemodes.Count == 0)
            {
                operationResult.AddError($"Invalid weapon \"{weaponRecord.Id}\", it needs atleast a firemode.");
            }
            else if (weaponRecord.Firemodes.Count > 2)
            {
                operationResult.AddError($"Invalid weapon \"{weaponRecord.Id}\", game limits firemodes to 2.");
            }
            else
            {
                CheckFiremode(weaponRecord.Id, weaponRecord.Firemodes[0], ref operationResult);

                if (weaponRecord.Firemodes.Count == 2)
                {
                    CheckFiremode(weaponRecord.Id, weaponRecord.Firemodes[1], ref operationResult);
                }
            }

            return operationResult;
        }

        private static void CheckFiremode(string weaponId, string fireMode, ref ImportOperationResult opResult)
        {
            if (string.IsNullOrEmpty(fireMode))
            {
                opResult.AddError($"Invalid weapon \"{weaponId}\", a firemode is empty or null.");
            }
            else if (!Data.Firemodes.Ids.Contains(fireMode))
            {
                opResult.AddError($"Invalid weapon \"{weaponId}\", fireMode \"{fireMode}\" does not exist in-game.");
            }
        }
    }
}
