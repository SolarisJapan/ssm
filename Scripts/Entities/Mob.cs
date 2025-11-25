using System.Collections.Generic;
using Godot;
using Game.Components;
using Game.Scene;

namespace Game.Entities
{
    public partial class Mob : EntityBase
    {
        private int _scale = 3;

        #region PUBLIC API
        public override void OnEnterScene(SceneBase scene)
        {
            Scale = new Vector2(_scale, _scale);
        }
        #endregion
        protected override List<ComponentBase> CreateAllComponents()
        {
            return new List<ComponentBase>
            {
                new MobAIComponent(this),
                new MoveComponent(this),
                new StateComponent(this),
                new AnimationComponent(this),
                new MobAIComponent(this),
                new AbilityComponent(this)
            };
        }
    }

}
