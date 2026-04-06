using MGSC;
using System.IO;
using System.Linq;

namespace QM_ImporterAPI
{
    public static class Plugin
    {
        public static ConfigDirectories ConfigDirectories = new ConfigDirectories();

        public static ModConfig Config { get; private set; }

        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void AfterConfig(IModContext context)
        {
            Directory.CreateDirectory(ConfigDirectories.ModPersistenceFolder);
            Config = ModConfig.LoadConfig(ConfigDirectories.ConfigPath);
        }

        private static void PrintCategories()
        {
            // Print all traits in a list?
            var categories = MGSC.Data.Items.Ids
                .Select(id => MGSC.Data.Items.GetSimpleRecord<WeaponRecord>(id))
                .Where(x => x != null)
                .SelectMany(x => x.Categories)
                .Distinct();

            var singleString = string.Join("\n", categories);
            UnityEngine.Debug.Log(singleString);
        }
    }
}
