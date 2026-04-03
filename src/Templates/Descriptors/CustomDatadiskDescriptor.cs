namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomDatadiskDescriptor : CustomItemContentDescriptor
    {
        public static CustomDatadiskDescriptor GetExample(string id)
        {
            return new CustomDatadiskDescriptor()
            {
                ItemId = id ?? "example_id",
                ImageProperties = new ImageProperties()
                {
                    IconSpriteIdOrPath = $"Sprites/{id}Icon.png",
                    SmallIconSpriteIdOrPath = $"Sprites/{id}Ground.png",
                    ShadowOnFloorSpriteIdOrPath = $"Sprites/{id}Shadow.png",
                },
            };
        }
    }
}