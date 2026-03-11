using Newtonsoft.Json;
using System;

namespace QM_ImporterAPI.Templates
{
    [Serializable]
    public class ImportableJson
    {
        /// <summary>
        /// This is the stringified version of the record type. It is used to determine which record type to deserialize the Data property into.
        /// </summary>
        [JsonProperty(Order = 1)]
        public string RecordType { get; set; } = string.Empty;

        /// <summary>
        /// This will contain a JSON-structure to deserialize as ANY record type.
        /// </summary>
        [JsonProperty(Order = 2)]
        public object Data { get; set; }
    }
}