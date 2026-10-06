namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomAugmentationDescriptor : CustomItemContentDescriptor
    {
        public static CustomAugmentationDescriptor GetExample(string id)
        {
            return new CustomAugmentationDescriptor
            {
                ItemId = id,
                ImageProperties = new ImageProperties
                {
                    IconSpriteIdOrPath = "Sprites/Icon.png",
                    SmallIconSpriteIdOrPath = "Sprites/SmallIcon.png",
                    ShadowOnFloorSpriteIdOrPath = "Sprites/ShadowOnFloor.png",
                },
            };
        }
    }
}
