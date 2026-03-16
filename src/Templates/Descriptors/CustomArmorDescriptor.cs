using System.Collections.Generic;

namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomArmorDescriptor : CustomItemContentDescriptor
    {
        public List<ArmorPartInfo> Parts { get; set; }

        public CustomArmorDescriptor()
        {

        }

        public static CustomArmorDescriptor GetExample(string id = null)
        {
            return new CustomArmorDescriptor
            {
                ItemId = id ?? "example_weaponid",
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = "Sprites/ExampleWeapon.png",
                    SmallIconSpriteIdOrPath = "Sprites/ExampleWeaponSmall.png",
                    ShadowOnFloorSpriteIdOrPath = "Sprites/ExampleWeaponShadow.png"
                },
                Parts = new List<ArmorPartInfo>()
                {
                    new ArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "Hip",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                    new ArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "RThigh",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                    new ArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "LThigh",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                    new ArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "RLeg",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                    new ArmorPartInfo()
                    {
                        ArmorType = "ClothCommon",
                        ArmorPart = "LLeg",
                        TextureIdOrPath = "Textures/ExampleTextureAtlas.png"
                    },
                }
            };
        }
    }

    public class ArmorPartInfo
    {
        public string ArmorType { get; set; }
        public string ArmorPart { get; set; }
        public string TextureIdOrPath { get; set; }
    }
}
