using Newtonsoft.Json;
using System;

namespace QM_ImporterAPI.Templates.Descriptors
{
    [Serializable]
    public class CustomAmmoDescriptor : CustomItemContentDescriptor
    {
        [JsonProperty(Order = 15)]
        public CustomGibsDescriptor Gibs { get; set; }

        [JsonProperty(Order = 5)]
        public bool MeleeMakeBlood { get; set; }

        public CustomAmmoDescriptor()
        {
            
        }

        public static CustomAmmoDescriptor GetExample(string id = null)
        {
            return new CustomAmmoDescriptor
            {
                ItemId = id ?? "example_weaponid",
                MeleeMakeBlood = true,
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = "Sprites/ExampleWeapon.png",
                    SmallIconSpriteIdOrPath = "Sprites/ExampleWeaponSmall.png",
                    ShadowOnFloorSpriteIdOrPath = "Sprites/ExampleWeaponShadow.png"
                },
                Gibs = new CustomGibsDescriptor()
                {
                    BulletSpritesId = "in_game_id",
                    BulletShadowsId = "in_game_id",
                    FlightDurationMsMin = 0.25f,
                    FlightDurationMsMax = 0.35f,
                    AnimationFramerate = 10
                }
            };
        }
    }

    public class CustomGibsDescriptor
    {
        public float FlightDurationMsMin { get; set; } = 0.25f;
        public float FlightDurationMsMax { get; set; } = 0.35f;
        public string BulletSpritesId { get; set; }
        public string BulletShadowsId { get; set; }
        public int AnimationFramerate{ get; set; } = 10;
    }
}
