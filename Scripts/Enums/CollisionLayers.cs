namespace Game.Enums
{
    public static class CollisionLayers
    {
        public const uint Platforms = 1;
        public const uint Entities = 1 << 2;
        public const uint Player = 1 << 3;
        public const uint Objects = 1 << 4;
    }
}