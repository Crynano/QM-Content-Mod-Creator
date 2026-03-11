using HarmonyLib;
using MGSC;
using System.IO;

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

            new Harmony("Crynano_" + ConfigDirectories.ModAssemblyName).PatchAll();
        }
    }
}
