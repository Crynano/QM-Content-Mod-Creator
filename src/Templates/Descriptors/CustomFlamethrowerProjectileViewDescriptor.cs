namespace QM_ImporterAPI.Templates.Descriptors
{
    /// <summary>
    /// Describes a FlamethrowerProjectileView. The view is cloned from an in-game projectile (BaseProjectileId)
    /// and registered under ItemId, so weapons can reference it through OverrideProjectileId.
    /// </summary>
    public class CustomFlamethrowerProjectileViewDescriptor : CustomBaseDescriptor
    {
        public string BaseProjectileId { get; set; }

        // ProjectileView
        public float BulletSpeed { get; set; }
        public bool MakeBloodDecals { get; set; }
        public bool PutShotDecalsOnWalls { get; set; }
        public bool PutBulletShellsOnFloor { get; set; }
        public bool RotateBulletInShotDir { get; set; }
        public float ShakeDuration { get; set; }
        public float ShakeStrength { get; set; }

        // FlamethrowerProjectileView
        public float BulletLifetime { get; set; }
        public float EmitUntilTime { get; set; }
        public float EmitFireInterval { get; set; }
        public float FlameDropSpeed { get; set; }

        public static CustomFlamethrowerProjectileViewDescriptor GetExample(string id)
        {
            return new CustomFlamethrowerProjectileViewDescriptor()
            {
                ItemId = id ?? "example_id",
                BaseProjectileId = "in_game_flamethrower_projectile_id",
                BulletSpeed = 10f,
                MakeBloodDecals = false,
                PutShotDecalsOnWalls = false,
                PutBulletShellsOnFloor = false,
                RotateBulletInShotDir = false,
                ShakeDuration = 0.2f,
                ShakeStrength = 0.5f,
                BulletLifetime = 1f,
                EmitUntilTime = 0.1f,
                EmitFireInterval = 0.01f,
                FlameDropSpeed = 1.5f
            };
        }
    }
}
