using System;
using System.Runtime.CompilerServices;
using Game.Entities;
using Game.Enums;
using Game.Objs;
using Godot;

namespace Game.Components
{
    public class MobAIComponent : ComponentBase
    {
        private float _actionTimer = 2.0f;

        #region PUBLIC API
        public MobAIComponent(Mob entity) : base(entity) { }

        public override void Update(double delta)
        {
            _actionTimer -= (float)delta;
            var moveComponent = Entity.GetComponent<MoveComponent>();

            if (_actionTimer > 0)
            {
                moveComponent.SetInput(Vector2.Zero, false, false, false);
                return;
            }

            _actionTimer = 2.0f;

            Random ran = new Random();
            int probability = ran.Next(0, 100);
            bool turn = false;

            if (probability <= 30)
            {
                return;
            }
            else if (probability <= 70)
            {
                // Turn
                turn = true;
            }
            else if (probability <= 100)
            {
                // Attack
                Entity.GetComponent<AbilityComponent>().CastAbility(AbilityEnum.Attack);
            }

            Vector2 input = Vector2.Zero;
            if (turn)
            {
                input = moveComponent.Direction == Direction.Forward ? new Vector2(-1, 0) : new Vector2(1, 0);
            }

            moveComponent.SetInput(input, false, false, false);
        }

        #endregion
    }
}