namespace Game.Enums
{
    public static class CollisionLayers
    {
        public const uint Default = 1;
        public const uint Entities = 1 << 1;
        public const uint Player = 1 << 2;
        public const uint Objects = 1 << 3;
    }
}