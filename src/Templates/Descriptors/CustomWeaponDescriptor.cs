using MGSC;
using Newtonsoft.Json;
using System;

namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomWeaponDescriptor : CustomItemContentDescriptor
    {
        [JsonProperty(Order = 11)]
        public AudioProperties AudioProperties { get; set; }
        [JsonProperty(Order = 12)]
        public ModelProperties ModelProperties { get; set; }
        [JsonProperty(Order = 7)]
        public HandsGrip Grip { get; set; }
        [JsonProperty(Order = 8)]
        public bool HasHFGOverlay { get; set; }

        public CustomWeaponDescriptor()
        {

        }

        public static CustomWeaponDescriptor GetExample(string id = null)
        {
            return new CustomWeaponDescriptor
            {
                ItemId = id ?? "example_weaponid",
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = "Sprites/ExampleWeapon.png",
                    SmallIconSpriteIdOrPath = "Sprites/ExampleWeaponSmall.png",
                    ShadowOnFloorSpriteIdOrPath = "Sprites/ExampleWeaponShadow.png"
                },
                AudioProperties = new AudioProperties
                {
                    ShootSoundIdOrPath = "Sounds/weapon_shoot_sound.mp3",
                    ReloadSoundIdOrPath = "Sounds/weapon_reload_sound.wav",
                    DryShotSoundIdOrPath = "Sounds/weapon_dry_sound.wav",
                    FailedAttackSoundIdOrPath = "combat_knife_1"
                },
                ModelProperties = new ModelProperties
                {
                    AssetBundlePath = "Bundles/ExampleWeapon.bundle",
                    TextureIdOrPath = "example_texturename_inside_assetbundle",
                    MuzzleId = "in_game_id",
                    PrefabId = "example_prefabname_inside_assetbundle or in_game_id",
                    PrefabScale = 0.12f
                },
                Grip = HandsGrip.Rifle,
                HasHFGOverlay = false
            };
        }
    }

    [Serializable]
    public class AudioProperties
    {
        public string ShootSoundIdOrPath { get; set; }
        public string ReloadSoundIdOrPath { get; set; }
        public string DryShotSoundIdOrPath { get; set; }
        public string FailedAttackSoundIdOrPath { get; set; }
    }

    [Serializable]
    public class ModelProperties
    {
        public string AssetBundlePath { get; set; }
        public string TextureIdOrPath { get; set; }
        public string MuzzleId { get; set; }
        public string PrefabId { get; set; }
        public float PrefabScale { get; set; }
    }
}
