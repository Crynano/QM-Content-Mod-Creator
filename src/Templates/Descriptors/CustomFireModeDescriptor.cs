namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomFireModeDescriptor : CustomBaseDescriptor
    {
        public string SpriteIdOrPath { get; set; }

        public static CustomFireModeDescriptor GetExample(string id)
        {
            return new CustomFireModeDescriptor
            {
                ItemId = id ?? "example_id",
                SpriteIdOrPath = "Sprites/FiremodeSpriteExample.png"
            };
        }
    }
}