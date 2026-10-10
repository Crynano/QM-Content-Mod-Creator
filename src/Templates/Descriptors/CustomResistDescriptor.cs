using System;
using System.Collections.Generic;

namespace QM_ImporterAPI.Templates.Descriptors
{
    [Serializable]
    public class CustomResistDescriptor : CustomItemContentDescriptor
    {
        public CustomArmorPartInfo Part { get; set; }

        public CustomResistDescriptor()
        {

        }

        public static CustomResistDescriptor GetExample(string id = null)
        {
            return new CustomResistDescriptor
            {
                ItemId = id ?? "example_weaponid",
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = "Sprites/ExampleWeapon.png",
                    SmallIconSpriteIdOrPath = "Sprites/ExampleWeaponSmall.png",
                    ShadowOnFloorSpriteIdOrPath = "Sprites/ExampleWeaponShadow.png"
                },
                Part = new CustomArmorPartInfo()
                {
                    ArmorType = "ClothCommon",
                    ArmorPart = "Hip",
                    TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                }
            };
        }
    }
}

[Serializable]
public class CustomArmorPartInfo
{
    public string ArmorType { get; set; }
    public string ArmorPart { get; set; }
    public string TextureIdOrPath { get; set; }
}
