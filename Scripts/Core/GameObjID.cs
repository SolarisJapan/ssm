namespace Game.Core
{
    class GameObjID
    {
        private static GameObjID _instance;
        public static GameObjID Instance => _instance ??= new GameObjID();

        private static ulong _nextID = 1;

        public static ulong GetNextID()
        {
            return _nextID++;
        }
    }

}