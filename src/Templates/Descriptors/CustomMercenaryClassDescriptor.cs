using System;

namespace QM_ImporterAPI.Templates.Descriptors
{
    [Serializable]
    public class CustomMercenaryClassDescriptor : CustomBaseDescriptor
    {
        public string IconSpriteIdOrPath { get; set; }
        public string SmallIconSpriteIdOrPath { get; set; }
        public CustomMercenaryClassDescriptor() { }
        public static CustomMercenaryClassDescriptor GetExample(string id)
        {
            return new CustomMercenaryClassDescriptor
            {
                ItemId = id ?? "example_mercenaryclass",
                IconSpriteIdOrPath = "example_icon_sprite",
                SmallIconSpriteIdOrPath = "example_small_icon_sprite"
            };
        }
    }
}
