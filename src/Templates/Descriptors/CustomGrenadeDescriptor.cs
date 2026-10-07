using Newtonsoft.Json;
using System;

namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomGrenadeDescriptor : CustomItemContentDescriptor
    {
        [JsonProperty(Order = 11)]
        public SoundProperties SoundProperties { get; set; }

        [JsonProperty(Order = 12)]
        public SpriteArrayProperties SpriteArrayProperties { get; set; }

        public CustomGrenadeDescriptor() { }

        public static CustomGrenadeDescriptor GetExample(string id)
        {
            return new CustomGrenadeDescriptor
            {
                ItemId = id ?? "example_grenadeid",
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = "Sprites/ExampleGrenade.png",
                    SmallIconSpriteIdOrPath = "Sprites/ExampleGrenadeSmall.png",
                    ShadowOnFloorSpriteIdOrPath = "Sprites/ExampleGrenadeShadow.png"
                },
                SoundProperties = new SoundProperties
                {
                    ThrowSoundIdOrPath = "Sounds/grenade_throw_sound.wav",
                    FallSoundIdOrPath = "Sounds/grenade_fall_sound.wav",
                    RicochetSoundIdOrPath = "Sounds/grenade_ricochet_sound.wav"
                },
                SpriteArrayProperties = new SpriteArrayProperties
                {
                    EntityFlySpriteIdOrPath = "Sprites/grenade_fly_sprites.png",
                    EntityShadowSpriteIdOrPath = "Sprites/grenade_shadow_sprites.png",
                    EntityActivationSpriteIdOrPath = "Sprites/grenade_activation_sprites.png"
                }
            };
        }
    }

    [Serializable]
    public class SoundProperties
    {
        public string ThrowSoundIdOrPath { get; set; }
        public string FallSoundIdOrPath { get; set; }
        public string RicochetSoundIdOrPath { get; set; }
    }

    [Serializable]
    public class SpriteArrayProperties
    {
        public string EntityFlySpriteIdOrPath { get; set; }
        public string EntityShadowSpriteIdOrPath { get; set; }
        public string EntityActivationSpriteIdOrPath { get; set; }
    }
}
