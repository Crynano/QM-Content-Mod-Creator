using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using System;
using System.Reflection;

namespace QM_ImporterAPI.Services.Importing
{
    public static class ImportableJsonDeserializer
    {
        private static readonly Assembly MGSC_ASSEMBLY = typeof(ConfigTableRecord).Assembly;
        private static readonly Assembly OWN_ASSEMBLY = typeof(CustomItemContentDescriptor).Assembly;

        private const string MGSC_NAMESPACE = "MGSC";
        public static object Deserialize(this ImportableJson item)
        {
            // This gets called by the importer
            // It should deserialize the JSON into an Item object
            // It gets data from the ImportableJson object, which has a RecordType and Data property
            // The RecordType is a string that indicates the TYPE of the CLASS to deserialize into
            if (item == null)
            {
                Logger.LogError("ImportableJson item is null");
                return null;
            }

            var mgscRecordType = item.RecordType.Split('.')[0];
            Type recordType;

            if (mgscRecordType.Equals(MGSC_NAMESPACE))
            {
                recordType = MGSC_ASSEMBLY.GetType(item.RecordType);
            }
            else
            {
                recordType = OWN_ASSEMBLY.GetType(item.RecordType);
            }

            if (recordType == null)
            {
                Logger.LogWarning($"Could not find type {item.RecordType}.");
                return null;
            }

            try
            {
                var deserializedItem = JsonConvert.DeserializeObject(item.Data.ToString(), recordType, settings: JsonExporterSettings.DeserializerSettings);
                if (deserializedItem == null)
                {
                    Logger.LogError($"Deserialization of {item.RecordType} returned null");
                    return null;
                }
                return deserializedItem;
            }
            catch (Exception ex)
            {
                //Logger.LogError($"Exception during deserialization of {item.RecordType}: {ex.Message}");
                UnityEngine.Debug.LogError(ex.Message);
            }
            return null;
        }
    }
}
