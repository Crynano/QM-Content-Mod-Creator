using System;
using System.Collections.Generic;

namespace QM_ImporterAPI.Templates.Descriptors
{
    [Serializable]
    public class CustomResistDescriptor : CustomItemContentDescriptor
    {
        public List<CustomArmorPartInfo> Parts { get; set; }

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
                Parts = new List<CustomArmorPartInfo>()
                {
                    new CustomArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "Hip",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                    new CustomArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "RThigh",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                    new CustomArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "LThigh",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                    new CustomArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "RLeg",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                    new CustomArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "LLeg",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                }
            };
        }
    }

    [Serializable]
    public struct CustomArmorPartInfo
    {
        public string ArmorType { get; set; }
        public string ArmorPart { get; set; }
        public string TextureIdOrPath { get; set; }
    }
}
