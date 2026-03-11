using MGSC;
using System;

namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomWeaponDescriptor : CustomItemContentDescriptor
    {
        public AudioProperties AudioProperties { get; set; } 
        public ModelProperties ModelProperties { get; set; }
        public HandsGrip Grip { get; set; }
        public bool HasHFGOverlay { get; set; }

        public CustomWeaponDescriptor()
        {

        }

        public static CustomWeaponDescriptor GetExample()
        {
            return new CustomWeaponDescriptor
            {
                ItemId = "example_weaponid",
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
                    FailedAttackSoundIdOrPath = "army_knife"
                },
                ModelProperties = new ModelProperties
                {
                    AssetBundlePath = "Assets/Models/ExampleWeapon.bundle",
                    TextureIdOrPath = "Textures/ExampleWeapon.png",
                    MuzzleId = "in_game_id",
                    PrefabId = "in_game_id",
                    PrefabScale = 0.12f
                },
                Grip = HandsGrip.Rifle,
                HasHFGOverlay = false
            };
        }

        public static CustomWeaponDescriptor GetExample(string id)
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
                    FailedAttackSoundIdOrPath = "army_knife"
                },
                ModelProperties = new ModelProperties
                {
                    AssetBundlePath = "Assets/Models/ExampleWeapon.bundle",
                    TextureIdOrPath = "Textures/ExampleWeapon.png",
                    MuzzleId = "in_game_id",
                    PrefabId = "in_game_id",
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
