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

        public static object GetPropertyFromItem<TRecord>(string id, string propertyName) where TRecord : ScriptableObject
        {
            TRecord descriptor = GetExistingItem<TRecord>(id);
            if (descriptor == null)
            {
                Logger.LogWarning($"Couldn't get {propertyName} from {id}. Item does not exist in-game.");
                return null;
            }

            Logger.LogDebug($"Getting the \"{propertyName}\" from \"{id}\" from \"{descriptor.GetType()}\"");
            Type type = descriptor.GetType();
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.FlattenHierarchy;
            var a = type.GetProperties(bindingAttr);
            if (a.ToList().Find(x => x.Name.Equals(propertyName)) == null)
            {
                Logger.LogError($"Property \"{propertyName}\" is not found!");
                return null;
            }

            var b = a.First(x => x.Name.Equals(propertyName));
            var retVal = b.GetValue(descriptor, null);
            Logger.LogDebug($"Successfully obtained the \"{propertyName}\" from \"{id}\"");
            return retVal;
        }

        public static object GetPropertyFromList<T, T2>(string id, string propertyName, ConfigRecordCollection<T> list)
            where T : ConfigTableRecord where T2 : ScriptableObject
        {
            T2 descriptor = GetExistingItem<T, T2>(id, list);
            if (descriptor == null)
            {
                Logger.LogError($"Couldn't get property from {id}. Item does not exist in-game.");
                return null;
            }

            Logger.LogDebug($"Getting the \"{propertyName}\" from \"{id}\" from \"{descriptor.GetType()}\"");
            Type type = descriptor.GetType();
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                       BindingFlags.FlattenHierarchy | BindingFlags.IgnoreCase;
            var a = type.GetProperties(bindingAttr);
            foreach (var item in a)
            {
                Logger.LogDebug($"Listing property {item} for {id}");
            }

            if (a.ToList().Find(x => x.Name.Equals(propertyName)) == null)
            {
                Logger.LogError($"Property \"{propertyName}\" is not found!");
                return null;
            }

            var b = a.First(x => x.Name.Equals(propertyName));
            var retVal = b.GetValue(descriptor, null);
            Logger.LogDebug($"Successfully obtained the \"{propertyName}\" from \"{id}\"");
            return retVal;
        }

        public static TDescriptor GetExistingItem<TDescriptor>(string id) where TDescriptor : ScriptableObject
        {
            var list = Data.Items;
            if (!string.IsNullOrEmpty(id) && list._records.ContainsKey(id))
            {
                var returnVal = list.GetSimpleRecord<BasePickupItemRecord>(id).ContentDescriptor as TDescriptor;
                return returnVal;
            }
            return null;
        }

        public static T2 GetExistingItem<T, T2>(string id, ConfigRecordCollection<T> list)
            where T : ConfigTableRecord where T2 : ScriptableObject
        {
            if (!string.IsNullOrEmpty(id) && list._records.ContainsKey(id))
            {
                Logger.LogDebug($"GetExistingItem({id}) result? {list._records.ContainsKey(id)}");
                var returnVal = ((T)list.GetRecord(id)).ContentDescriptor as T2;
                if (returnVal != null)
                    Logger.LogDebug($"Type of ReturnVal {returnVal.GetType()}");
                return returnVal;
                //T2 returnResult = record.ContentDescriptor as T2;
                //return returnResult;
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
