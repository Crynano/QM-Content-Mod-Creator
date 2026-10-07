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
                IconSpriteIdOrPath = "Sprites/92x92Sprite.png",
                SmallIconSpriteIdOrPath = "Sprites/24x24Sprite.png"
            };
        }
    }
}
