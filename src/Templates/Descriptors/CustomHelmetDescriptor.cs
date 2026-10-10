using System;

namespace QM_ImporterAPI.Templates.Descriptors
{
    [Serializable]
    internal class CustomHelmetDescriptor : CustomResistDescriptor
    {
        public string PrefabIdOrPath { get; set; }

        public new static CustomHelmetDescriptor GetExample(string id = null)
        {
            return new CustomHelmetDescriptor()
            {
                ItemId = id ?? "custom_helmet",
                PrefabIdOrPath = "Models/ExampleHelmet.obj",
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = "Sprites/Icon/icon.png",
                    SmallIconSpriteIdOrPath = "Sprites/SmallIcon/SmallIcon.png",
                    ShadowOnFloorSpriteIdOrPath = "Sprites/Shadow/shadow.png",
                },
                Part = new CustomArmorPartInfo()
                {
                    ArmorPart = "Head",
                    ArmorType = "ArmorLight",
                    TextureIdOrPath = "Textures/ExampleHelmetTexture.png"
                }
            };
        }
    }
}
