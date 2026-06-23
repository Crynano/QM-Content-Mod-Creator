namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomTooltipImage
    {
        public string SpritePathOrId { get; set; }
        public string Tag { get; set; }

        public static CustomTooltipImage GetExample(string tag = null)
        {
            return new CustomTooltipImage
            {
                SpritePathOrId = $"Sprites/{tag ?? "example_tag"}.png",
                Tag = tag ?? "example_tag"
            };
        }
    }
}