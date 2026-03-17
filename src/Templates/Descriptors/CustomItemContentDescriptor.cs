using Newtonsoft.Json;
using System;

namespace QM_ImporterAPI.Templates.Descriptors
{
    [Serializable]
    public class CustomItemContentDescriptor : CustomBaseDescriptor
    {
        public CustomItemContentDescriptor() : base() { }
        [JsonProperty(Order = 10)]
        public ImageProperties ImageProperties { get; set; }
    }

    [Serializable]
    public class ImageProperties
    {
        public string IconSpriteIdOrPath { get; set; }
        public string SmallIconSpriteIdOrPath { get; set; }
        public string ShadowOnFloorSpriteIdOrPath { get; set; }
    }
}