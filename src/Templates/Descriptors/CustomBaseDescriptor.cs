using Newtonsoft.Json;
using System;

namespace QM_ImporterAPI.Templates.Descriptors
{
    [Serializable]
    public abstract class CustomBaseDescriptor
    {
        [JsonProperty(Order = 1)]
        public string ItemId { get; set; } = string.Empty;
    }
}