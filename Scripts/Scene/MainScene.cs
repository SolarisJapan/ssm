using Godot;
using Game.Core;
using Game.Entities;

namespace Game.Scene
{
    public partial class MainScene : SceneBase
    {
        public override void Init()
        {
            GameLogger.Log($"MainScene Init called. Name: {SceneID}");
        }

        public override void OnEntityEnterScene(EntityBase entity)
        {
            GameLogger.Log($"Entity {entity.EntityId} entered MainScene {SceneID}");
        }

    }
}