using MGSC;
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
            return !string.IsNullOrEmpty(id) && Data.Items._records.ContainsKey(id);
        }

        public static bool IsGameId<TRecord>(string id, ConfigRecordCollection<TRecord> list) where TRecord : ConfigTableRecord
        {
            return !string.IsNullOrEmpty(id) && list._records.ContainsKey(id);
        }

        public static bool DoesItemExistInList<T>(string id, ConfigRecordCollection<T> list)
            where T : ConfigTableRecord
        {
            try
            {
                return !string.IsNullOrEmpty(id) && list.Ids.Contains(id);
            }
            catch (Exception ex)
            {
                Debug.LogWarning(ex);
                return false;
            }
        }

        public static SoundBank[] GetAudiosFromExistingWeapons(string id, int category, bool fallbackToDefault)
        {
            WeaponDescriptor existingWeaponDescriptor = GetExistingWeaponDescriptor(id, fallbackToDefault);
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
            object propertyFromItem = GetPropertyFromItem<WeaponDescriptor>(id, "Prefab");
            return (GameObject)((propertyFromItem is GameObject) ? propertyFromItem : null);
        }

        public static Texture GetTextureFromExistingWeapon(string id)
        {
            object propertyFromItem = GetPropertyFromItem<WeaponDescriptor>(id, "Texture");
            return (Texture)((propertyFromItem is Texture) ? propertyFromItem : null);
        }

        public static Muzzle GetMuzzleFromExistingWeapon(string id)
        {
            WeaponDescriptor existingWeaponDescriptor = GetExistingWeaponDescriptor(id);
            if (existingWeaponDescriptor == null || existingWeaponDescriptor._muzzles.Length == 0)
            {
                return null;
            }
            return existingWeaponDescriptor._muzzles[0];
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
            return GetPropertyFromList<BasePickupItemRecord, TDescriptor>(id, propertyName, Data.Items);
        }

        public static object GetPropertyFromList<TRecord, TDescriptor>(string id, string propertyName, ConfigRecordCollection<TRecord> list)
            where TRecord : ConfigTableRecord where TDescriptor : ScriptableObject
        {
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
            if (!string.IsNullOrEmpty(id) && list._records.ContainsKey(id))
            {
                Logger.LogDebug($"GetExistingItem({id}) result? {list._records.ContainsKey(id)}");
                var returnVal = ((TRecord)list.GetRecord(id)).ContentDescriptor as TScriptable;
                if (returnVal != null)
                    Logger.LogDebug($"Type of ReturnVal {returnVal.GetType()}");
                return returnVal;
            }

            return null;
        }

        public static WeaponDescriptor GetExistingWeaponDescriptor(string id, bool getDefault = true)
        {
            WeaponDescriptor result = null;
            if (!string.IsNullOrEmpty(id) && Data.Items._records.ContainsKey(id))
            {
                result = Data.Items.GetSimpleRecord<WeaponRecord>(id).ContentDescriptor as WeaponDescriptor;
            }
            else if (getDefault)
            {
                Data.Descriptors.TryGetValue("rangeweapons", out var value);
                if (string.IsNullOrEmpty(id))
                {
                    Logger.LogWarning("ID is empty or null. Using <" + value._ids[0] + "> as default.");
                }
                else
                {
                    Logger.LogWarning("Item with ID: <" + id + "> not found in-game. Using <" + value._ids[0] + "> as default.");
                }
                return value._descriptors[0] as WeaponDescriptor;
            }
            return result;
        }

        public static Sprite CloneSprite(Sprite sprite)
        {
            return sprite;
        }
    }
}
