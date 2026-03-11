using MGSC;
using QM_ImporterAPI.Services.Helpers;
using QM_ImporterAPI.Services.Localization;
using UnityEngine;

namespace QM_ImporterAPI.Services
{
    public static class ImporterApi
    {
        /// <summary>
        /// Path where the Assets folder is contained in!
        /// Usually you want this to be the mod's root folder, so that you can have an Assets folder inside it with all your assets and json files.
        /// </summary>
        /// <param name="givenPath">String path where the Assets folder is. Make sure is valid and not null.</param>
        public static void LoadModFromDirectory(string givenPath)
        {
            var modLoader = new ModLoader();
            modLoader.LoadModFromDirectory(givenPath);
        }

        /// <summary>
        /// Loads a mod using the specified mod context. Easily callable from any in-game hook.
        /// Make sure to only call this once. And its heavily recommended to only call from the AfterConfig hook.
        /// </summary>
        /// <param name="modContext">The mod context that provides the necessary information for loading the mod. This parameter cannot be null.</param>
        public static void LoadModFromContext(IModContext modContext)
        {
            var modLoader = new ModLoader();
            modLoader.LoadModFromContext(modContext);
        }

        #region Helper Methods

        /// <summary>
        /// Gets a property from an existing in-game item from the MGSC.Data.Items list.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <param name="propertyName">Property Name you want to obtain. CASE-SENSITIVE</param>
        /// <typeparam name="T">Descriptor type. e.g. ItemDescriptor, WoundDescriptor, etc</typeparam>
        /// <returns></returns>
        public static object GetPropertyFromItem<T>(string id, string propertyName) where T : ScriptableObject
        {
            return QuasimorphHelper.GetPropertyFromItem<T>(id, propertyName);
        }

        /// <summary>
        /// Gets a property from an existing item contained in a given list.
        /// </summary>
        /// <param name="id">The id of the item.</param>
        /// <param name="propertyName">Property Name you want to obtain. CASE-SENSITIVE</param>
        /// <param name="list">The list where the item is contained. Usually MGSC.Data.Items or similar.</param>
        /// <typeparam name="T">Record type. For example WoundRecord. Must match the Record type from parameter T2.
        /// e.g. T:WoundRecord, T2:WoundDescriptor.</typeparam>
        /// <typeparam name="T2">Descriptor type. For example WoundDescriptor. Must match the Record type from parameter T.
        /// e.g. T:WoundRecord, T2:WoundDescriptor.</typeparam>
        /// <returns></returns>
        public static object GetPropertyFromList<T, T2>(string id, string propertyName, ConfigRecordCollection<T> list)
            where T : BasePickupItemRecord where T2 : ScriptableObject
        {
            return QuasimorphHelper.GetPropertyFromList<T, T2>(id, propertyName, list);
        }
    
        /// <summary>
        /// Adds localized text to a specific language.
        /// </summary>
        /// <param name="fullyQualifiedKey">The full key, containing category and identifier.
        /// e.g. item.example_id.name, perk.item_example.desc</param>
        /// <param name="text">The localized text to be displayed.</param>
        /// <param name="language">Language enum from MGSC.Localization.Lang</param>
        public static void AddLocalization(string fullyQualifiedKey, string text, MGSC.Localization.Lang language)
        {
            LocalizationHelper.AddLocalization(fullyQualifiedKey, text, language);
        }

        /// <summary>
        /// Adds the same text to all languages.
        /// </summary>
        /// <param name="fullyQualifiedKey">The full key, containing category and identifier.
        /// e.g. item.example_id.name, perk.item_example.desc</param>
        /// <param name="text">The localized text to be displayed.</param>
        public static void AddLocalizationToAllDictionaries(string fullyQualifiedKey, string text)
        {
            LocalizationHelper.AddLocToAllDictionaries(fullyQualifiedKey, text);
        }

        #endregion
    }
}