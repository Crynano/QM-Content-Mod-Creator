namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomTrashDescriptor : CustomItemContentDescriptor
    {
        public static CustomTrashDescriptor GetExample(string itemId = "example_id")
        {
            return new CustomTrashDescriptor()
            {
                ItemId = itemId,
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = $"Sprites/{itemId}_icon.png",
                    SmallIconSpriteIdOrPath = $"Sprites/{itemId}_small_icon.png",
                    ShadowOnFloorSpriteIdOrPath = $"Sprites/{itemId}_shadow_on_floor.png"
                }
            };
        }
    }
}
