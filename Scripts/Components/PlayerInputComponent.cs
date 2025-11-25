using Game.Entities;
using Game.Enums;
using Game.Core;
using Godot;

namespace Game.Components
{
    public class PlayerInputComponent : ComponentBase
    {
        #region PUBLIC API
        public PlayerInputComponent(Player player) : base(player) { }

        public override void Update(double _delta)
        {
            var player = (Player)Entity;

            Vector2 inputVector = Input.GetVector(InputActions.MoveLeft, InputActions.MoveRight, InputActions.MoveUp, InputActions.MoveDown);
            bool jump = Input.IsActionJustPressed(InputActions.Jump);
            bool sprint = Input.IsActionPressed(InputActions.Sprint);
            bool crouch = Input.IsActionPressed(InputActions.Crouch);
            player.GetComponent<MoveComponent>().SetInput(inputVector, jump, sprint, crouch);

            if (Input.IsActionJustPressed(InputActions.Attack))
            {
                player.GetComponent<AbilityComponent>().CastAbility(AbilityEnum.Attack);
            }
        }
        #endregion
    }
}