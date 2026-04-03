using MGSC;
using QM_ImporterAPI.Services.ErrorManagement;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace QM_ImporterAPI.Services.Helpers
{
    public static class QuasimorphHelper
    {

        #region QUASI
        public static bool IsGameId(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.LogError("ID is not game ID because its empty or null.");
                return false;
            }
            return Data.Items.Ids.Contains(id);
        }

        public static bool IsGameId<TRecord>(string id, ConfigRecordCollection<TRecord> list) where TRecord : ConfigTableRecord
        {
            if (string.IsNullOrEmpty(id))
            {
                Logger.LogError("ID is not game ID because its empty or null.");
                return false;
            }
            return list.Ids.Contains(id);
        }

        public static SoundBank[] GetAudiosFromExistingWeapons(string id, int category)
        {
            WeaponDescriptor existingWeaponDescriptor = GetExistingWeaponDescriptor(id);
            if (existingWeaponDescriptor == null)
            {
                return new SoundBank[0];
            }

            switch (category)
            {
                case 0: return existingWeaponDescriptor._attackSoundBanks;
                case 1: return existingWeaponDescriptor._dryShotSoundBanks;
                case 2: return existingWeaponDescriptor._failedAttackSoundBanks;
                case 3: return existingWeaponDescriptor._reloadSoundBanks;
                default: return new SoundBank[0];
            }
        }

        public static GameObject GetPrefabFromExistingWeapon(string id)
        {
            return GetExistingWeaponDescriptor(id)?.Prefab;
        }

        public static Texture GetTextureFromExistingWeapon(string id)
        {
            return GetExistingWeaponDescriptor(id)?.Texture;
        }

        public static Muzzle GetMuzzleFromExistingWeapon(string id)
        {
            return GetExistingWeaponDescriptor(id)?._muzzles.FirstOrDefault();
        }

        public static bool IsItemInFactionTable(string tableName, string rewardId)
        {
            Dictionary<int, List<ContentDropRecord>>.ValueCollection values = Data.FactionDrop.GetRawData(tableName).Values;
            List<string> list = new List<string>();
            foreach (List<ContentDropRecord> item in values)
            {
                foreach (ContentDropRecord item2 in item)
                {
                    item2.ContentIds.ForEach(list.Add);
                }
            }
            bool num = list.Contains(rewardId);
            if (num)
            {
                Logger.LogInfo("Item " + rewardId + " already found in " + tableName);
            }
            return num;
        }

        public static void AddLocalization(LocalizationTemplate localization)
        {
            Dictionary<MGSC.Localization.Lang, Dictionary<string, string>> db = Singleton<MGSC.Localization>.Instance.db;

            foreach (var fullKey in localization.Keys)
            {
                foreach (var langLocalization in fullKey.Value.Where(langLocalization => !db[langLocalization.Key].ContainsKey(fullKey.Key)))
                {
                    db[langLocalization.Key].Add(fullKey.Key, langLocalization.Value);
                }
            }
        }
        #endregion

        #region Audio

        public static ImportOperationResult<AudioClip> GetAudioFromConsumableOrPath(string id, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult<AudioClip>();

            if (IsGameId(id))
            {
                Logger.LogDebug($"ID {id} is a valid consumable game ID. Attempting to load audio from existing consumable.");
                ConsumableDescriptor descriptor = GetPropertyFromItem<ConsumableDescriptor>(id, "UseSound") as ConsumableDescriptor;
                if (descriptor != null)
                {
                    operationResult.SetResult(descriptor._useSound);
                }
            }
            else
            {
                Logger.LogDebug($"ID {id} is not a game ID. Attempting to load audio from path.");
                var audioResult = LoadAudioClipFromExternalFile(id, assetFolderPath);
                operationResult.Absorb(audioResult);
                if (audioResult.IsSuccess)
                {
                    operationResult.SetResult(audioResult.Result);
                }
            }

            return operationResult;
        }

        internal static ImportOperationResult<AudioClip> LoadAudioClipFromExternalFile(string soundPath, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult<AudioClip>();

            if (!QuasimorphHelper.IsGameId(soundPath))
            {
                var soundFullPath = Helper.ResolvePath(assetFolderPath, soundPath);
                var importAudioResult = AssetImporter.ImportAudio(soundFullPath);
                if (importAudioResult.IsSuccess)
                {
                    operationResult.SetResult(importAudioResult.Result);
                }
            }
            else
            {
                operationResult.AddWarning($"The provided sound path '{soundPath}' is a game ID. Use the appropriate method to load audio from existing weapons.");
            }

            return operationResult;
        }

        internal static ImportOperationResult<IEnumerable<AudioClip>> LoadAudioClipsFromExternalFiles(IEnumerable<string> externalFiles, string assetFolderPath)
        {
            var operationResult = new ImportOperationResult<IEnumerable<AudioClip>>();

            var audioClips = new List<AudioClip>();
            foreach (var file in externalFiles)
            {
                var audioClipResult = LoadAudioClipFromExternalFile(file, assetFolderPath);
                if (audioClipResult.IsSuccess)
                {
                    audioClips.Add(audioClipResult.Result);
                }
                else
                {
                    operationResult.Absorb(audioClipResult);
                }
            }

            if (audioClips.Count > 0)
            {
                operationResult.SetResult(audioClips);
            }
            else
            {
                operationResult.AddError("No valid audio clips were loaded from the provided external files.");
            }

            return operationResult;
        }

        public static ImportOperationResult ResolveSoundBank<TRecord, TDescriptor>(ref SoundBank soundBank, string soundPath, string assetFolderPath, ConfigRecordCollection<TRecord> list)
    where TRecord : ConfigTableRecord where TDescriptor : ScriptableObject
        {
            var operationResult = new ImportOperationResult();
            if (soundBank == null)
            {
                soundBank = ScriptableObject.CreateInstance(typeof(SoundBank)) as SoundBank;
                soundBank._clips = new AudioClip[1];
            }

            if (QuasimorphHelper.IsGameId(soundPath, list))
            {
                var existingProperty = QuasimorphHelper.GetPropertyFromList<TRecord, TDescriptor>(soundPath, "explosionSoundBank", list);
                if (existingProperty is SoundBank existingSoundBank)
                {
                    soundBank = existingSoundBank;
                }
                else
                {
                    operationResult.AddError($"Unable to load sound from existing game item with ID: {soundPath}");
                }
            }
            else
            {
                var soundFullPath = Helper.ResolvePath(assetFolderPath, soundPath);
                var importAudioResult = AssetImporter.ImportAudio(soundFullPath);
                operationResult.Absorb(importAudioResult);
                if (importAudioResult.IsSuccess)
                {
                    soundBank._clips[0] = importAudioResult.Result;
                }
            }
            return operationResult;
        }

        #endregion

        #region Components
        public static T CopyComponent<T>(T original, GameObject destination) where T : Component
        {
            Type type = ((object)original).GetType();
            Component val = destination.AddComponent(type);
            FieldInfo[] fields = type.GetFields();
            foreach (FieldInfo fieldInfo in fields)
            {
                fieldInfo.SetValue(val, fieldInfo.GetValue(original));
            }
            return (T)(object)((val is T) ? val : null);
        }

        public static object GetPropertyFromItem<TDescriptor>(string id, string propertyName) where TDescriptor : ScriptableObject
        {
            Logger.LogDebug($"{nameof(GetPropertyFromItem)}: with {id} and {propertyName}");
            return GetPropertyFromList<BasePickupItemRecord, TDescriptor>(id, propertyName, Data.Items);
        }

        public static object GetPropertyFromList<TRecord, TDescriptor>(string id, string propertyName, ConfigRecordCollection<TRecord> list)
            where TRecord : ConfigTableRecord where TDescriptor : ScriptableObject
        {
            Logger.LogDebug($"{nameof(GetPropertyFromList)}: with {id}, {propertyName} and list of type {list.GetType()}");
            TDescriptor descriptor = GetExistingItem<TRecord, TDescriptor>(id, list);
            if (descriptor == null)
            {
                Logger.LogError($"Couldn't get property from {id}. Item does not exist in-game.");
                return null;
            }

            Logger.LogDebug($"Getting the \"{propertyName}\" from \"{id}\" from \"{descriptor.GetType()}\"");

            var type = descriptor.GetType();
            var bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase;

            var properties = type.GetProperties(bindingAttr);

            foreach (var item in properties)
            {
                Logger.LogDebug($"Listing property {item} for {id}");
            }

            object returnValue;
            if (properties.ToList().Find(x => x.Name.Equals(propertyName)) == null)
            {
                var fields = type.GetFields(bindingAttr);
                fields.ToList().ForEach(x => Logger.LogDebug($"Listing field {x} for {id}"));
                if (fields.ToList().Find(x => x.Name.Equals(propertyName)) == null)
                {
                    Logger.LogError($"Field or Property \"{propertyName}\" has not been found.");
                    return null;
                }
                else
                {
                    returnValue = fields.First(x => x.Name.Equals(propertyName)).GetValue(descriptor);
                }
            }
            else
            {
                returnValue = properties.First(x => x.Name.Equals(propertyName)).GetValue(descriptor, null);
            }
        
            Logger.LogDebug($"Successfully obtained the \"{propertyName}\" from \"{id}\"");
            return returnValue;
        }

        public static TScriptable GetExistingItem<TRecord, TScriptable>(string id, ConfigRecordCollection<TRecord> list)
            where TRecord : ConfigTableRecord where TScriptable : ScriptableObject
        {
            if(string.IsNullOrEmpty(id))
            {
                Logger.LogError("ID is empty or null. Cannot get existing item.");
            }
            else if (list.Ids.Contains(id))
            {
                Logger.LogDebug($"GetExistingItem({id}) good.");
                var record = list.GetRecord(id, false);
                if (record is CompositeItemRecord compositeRecord)
                { 
                    Logger.LogDebug($"Record is a CompositeItemRecord");
                    var primaryRecord = compositeRecord.PrimaryRecord;
                    if (primaryRecord == null)
                    {
                        Logger.LogDebug($"Primary record for {id} NOT found.");
                        return null;
                    }

                    Logger.LogDebug($"Record for {id} found. Type of record: {primaryRecord.GetType()}");
                    var contentDesc = primaryRecord.ContentDescriptor;
                    if (contentDesc != null)
                    {
                        Logger.LogDebug($"ContentDescriptor for {id} found. Type of ContentDescriptor: {contentDesc.GetType()}");
                        return contentDesc as TScriptable;
                    }
                    else
                    {
                        Logger.LogDebug($"ContentDescriptor is null.");
                    }
                }
                else if (record is ConfigTableRecord itemRecord)
                {
                    Logger.LogDebug($"Record is a ConfigTableRecord");
                    var contentDesc = itemRecord.ContentDescriptor;
                    if (contentDesc != null)
                    {
                        Logger.LogDebug($"ContentDescriptor for {id} found. Type of ContentDescriptor: {contentDesc.GetType()}");
                        return contentDesc as TScriptable;
                    }
                    else
                    {
                        Logger.LogDebug($"ContentDescriptor is null.");
                    }
                }
                else
                {
                    Logger.LogError($"Record for {id} not found in list.");
                }
            }

            return null;
        }
        #endregion

        public static WeaponDescriptor GetExistingWeaponDescriptor(string id)
        {
            WeaponDescriptor result = null;
            if (string.IsNullOrEmpty(id))
            {
                Logger.LogDebug("ID is empty or null. Cannot get existing weapon descriptor.");
            }
            else if (Data.Items.Ids.Contains(id))
            {
                result = Data.Items.GetSimpleRecord<WeaponRecord>(id).ContentDescriptor as WeaponDescriptor;
            }
            return result;
        }

        public static Sprite CloneSprite(Sprite sprite)
        {
            return sprite;
        }
    }
}
