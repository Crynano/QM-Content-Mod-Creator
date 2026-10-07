namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomImplantDescriptor : CustomItemContentDescriptor
    {
        public CustomImplantDescriptor() : base()
        {
        
        }
    
        public string UseSoundPath {get; set;} = string.Empty;

        public static CustomImplantDescriptor GetExample(string id)
        {
            return new CustomImplantDescriptor
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