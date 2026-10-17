namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomConsumableDescriptor : CustomItemContentDescriptor
    {
        public string UseSoundPathOrId { get; set; }

        public CustomConsumableDescriptor() { }

        public static CustomConsumableDescriptor GetExample(string id)
        {
            return new CustomConsumableDescriptor
            {
                ItemId = id ?? "example_consumableid",
                UseSoundPathOrId = id ?? "Sounds/example_use_sound.wav",
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = "Sprites/ExampleWeapon.png",
                    SmallIconSpriteIdOrPath = "Sprites/ExampleWeaponSmall.png",
                    ShadowOnFloorSpriteIdOrPath = "Sprites/ExampleWeaponShadow.png"
                },
            };
        }
    }
}