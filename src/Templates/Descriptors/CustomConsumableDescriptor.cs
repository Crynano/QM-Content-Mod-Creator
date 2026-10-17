namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomUsableDescriptor : CustomItemContentDescriptor
    {
        public CustomUsableDescriptor() { }

        public static CustomUsableDescriptor GetExample(string id)
        {
            return new CustomUsableDescriptor
            {
                ItemId = id ?? "example_usableid",
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = id ?? "Sprites/ExampleWeapon.png",
                    SmallIconSpriteIdOrPath = id ?? "Sprites/ExampleWeaponSmall.png",
                    ShadowOnFloorSpriteIdOrPath = id ?? "Sprites/ExampleWeaponShadow.png"
                },
            };
        }
    }
}