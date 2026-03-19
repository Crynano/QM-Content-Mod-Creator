namespace QM_ImporterAPI.Templates.Descriptors
{
    public class CustomExplosionDescriptor : CustomBaseDescriptor
    {
        public string ExplosionVisualId { get; set; }
        public string ExplosionSoundIdOrPath { get; set; }
        public float VisualExplosionDelay { get; set; }
        public float VisualReachCellDuration { get; set; }
        public int ClearGibsRadiusInPixels { get; set; }
        public bool ShakeCameraOnExplosion { get; set; }
        public float VisualExplosionOffsetX { get; set; }
        public float VisualExplosionOffsetY { get; set; }
        public float VisualExplosionOffsetZ { get; set; }

        public static CustomExplosionDescriptor GetExample(string id)
        {
            return new CustomExplosionDescriptor()
            {
                ItemId = id ?? "example_id",
                ExplosionVisualId = "in_game_id",
                ExplosionSoundIdOrPath = "Sounds/ExplosionSound.mp3",
                VisualExplosionDelay = 0.5f,
                VisualReachCellDuration = 0.3f,
                ClearGibsRadiusInPixels = 25,
                ShakeCameraOnExplosion = true,
                VisualExplosionOffsetX = 0f,
                VisualExplosionOffsetY = 0f,
                VisualExplosionOffsetZ = 0f
            };
        }
    }
}
