using Newtonsoft.Json;

namespace QM_ImporterAPI.Templates.Descriptors
{
    public abstract class CustomBaseDescriptor
    {
        [JsonProperty(Order = 1)]
        public string ItemId { get; set; } = string.Empty;
    }
}