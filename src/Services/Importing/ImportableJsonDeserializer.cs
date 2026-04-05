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
            var recordTypeName = item.RecordType.Split('.')[1];
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
                // If somehow the record is null, replace the start with my own namespace and try again, to account for people using the wrong namespace
                var correctedRecordTypeName = $"{OWN_ASSEMBLY.GetName().Name}.{recordTypeName}";
                recordType = OWN_ASSEMBLY.GetType(correctedRecordTypeName);
                Logger.LogWarning($"Could not find type {item.RecordType}. Attempting to use corrected type {correctedRecordTypeName}.");

                if (recordType == null)
                {
                    Logger.LogError($"Could not find type {correctedRecordTypeName}.");
                    return null;
                }
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
