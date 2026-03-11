using MGSC;
using Newtonsoft.Json;
using QM_ImporterAPI.Templates;
using QM_ImporterAPI.Templates.Descriptors;
using System;

namespace QM_ImporterAPI.Services.Importing
{
    public static class ImportableJsonDeserializer
    {
        public static object Deserialize(this ImportableJson item)
        {
            // This gets called by the importer
            // It should deserialize the JSON into an Item object
            // It gets data from the ImportableJson object, which has a RecordType and Data property
            // The RecordType is a string that indicates the TYPE of the CLASS to deserialize into
            if (item == null)
            {
                //Logger.LogError("ImportableJson item is null");
                return null;
            }

            var type = typeof(ConfigTableRecord).Assembly.GetType(item.RecordType);
            if (type == null)
            {
                //Logger.LogError($"Could not find type {item.RecordType}");
                type = typeof(CustomItemContentDescriptor).Assembly.GetType(item.RecordType);
                if (type == null)
                {
                    return null;
                }
            }

            try
            {
                var deserializedItem = JsonConvert.DeserializeObject(item.Data.ToString(), type, settings: JsonExporterSettings.DeserializerSettings);
                if (deserializedItem == null)
                {
                    //Logger.LogError($"Deserialization of {item.RecordType} returned null");
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
