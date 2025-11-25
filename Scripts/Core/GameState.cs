using Game.Scene;

namespace Game.Core
{
    public class GameState
    {
        private static GameState _instance;
        public static GameState Instance => _instance ??= new GameState();

        public BulletManager BulletMgr { get; private set; }

        public SceneBase CurrentScene { get; private set; }

        public SFXManager SFXMgr { get; private set; }

        public GameState()
        {
            BulletMgr = new BulletManager();
            SFXMgr = new SFXManager();
        }

        public void SetCurrentScene(SceneBase scene)
        {
            CurrentScene = scene;
            BulletMgr.SetScene(scene);
        }
    }
}

