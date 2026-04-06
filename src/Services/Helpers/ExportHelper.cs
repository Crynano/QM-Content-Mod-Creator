using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Services.Importing;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using System.IO;
using UnityEngine;

namespace QM_ImporterAPI.Services.Helpers
{
    internal static class ExportHelper
    {

        internal static void ExportItem<TRecord>(TRecord item, string basePath) where TRecord : ConfigTableRecord
        {
            if (item == null) { Debug.LogWarning($"Item null for \"{basePath}\", skipping export."); return; }
            var classType = item.GetType();
            var result = new ImportableJson()
            {
                RecordType = classType.FullName,
                Data = item
            };
            var pathCombined = Path.Combine(basePath, $"{item.Id}.json");
            File.WriteAllText(pathCombined, JsonConvert.SerializeObject(result, JsonExporterSettings.SerializerSettings));
        }

        internal static void ExportCustomDescriptor<TDesc>(TDesc descriptor, string basePath) where TDesc : CustomBaseDescriptor
        {
            if (descriptor == null) { Debug.LogWarning($"Item null for \"{basePath}\", skipping export."); return; }
            var classType = descriptor.GetType();
            var result = new ImportableJson()
            {
                RecordType = classType.FullName,
                Data = descriptor
            };
            var pathCombined = Path.Combine(basePath, $"{descriptor.ItemId}_descriptor.json");
            File.WriteAllText(pathCombined, JsonConvert.SerializeObject(result, JsonExporterSettings.SerializerSettings));
        }

        internal static void ExportCustom<T>(string fullPath, T item) where T : class, new()
        {
            if (item == null) { Debug.LogWarning($"Item null for \"{fullPath}\", skipping export."); return; }
            var classType = item.GetType();
            var result = new ImportableJson()
            {
                RecordType = classType.FullName,
                Data = item
            };
            File.WriteAllText(fullPath, JsonConvert.SerializeObject(result, JsonExporterSettings.SerializerSettings));
        }

        internal static void ExportCustom<T>(T item, string fileName, string basePath) where T : class, new()
        {
            if (item == null) { Debug.LogWarning($"Item null for \"{basePath}\", skipping export."); return; }
            var classType = item.GetType();
            var result = new ImportableJson()
            {
                RecordType = classType.FullName,
                Data = item
            };
            var pathCombined = Path.Combine(basePath, $"{fileName}.json");
            File.WriteAllText(pathCombined, JsonConvert.SerializeObject(result, JsonExporterSettings.SerializerSettings));
        }
    }
}
