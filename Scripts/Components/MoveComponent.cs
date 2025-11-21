using Game.Entities;
using Game.Core;
using Game.Enums;
using Godot;

namespace Game.Components
{
    public class MoveComponent : ComponentBase
    {

        public MovingStates State { get { return _state; } }

        private float JumpForce = 400f;
        private MovingStates _state = MovingStates.Stopped;
        private Vector2 _velocity = new Vector2(800, 800);
        private float _gravity = 20f;
        private float _sprintMultiplier = 2f;

        private Vector2 _inputVector = Vector2.Zero;
        private bool _jump;
        private bool _sprint;
        private bool _crouch;

        public MoveComponent(EntityBase entity) : base(entity) { }

        public override void Update(double delta)
        {
            if (Entity is not Player player)
                throw new System.Exception("MoveComponent can only be used with Player entities.");

            // TODO status component to get states like grounded, climbing, etc.
            bool onFloor = player.IsOnFloor();
            if (_inputVector == Vector2.Zero)
            {
                _state = MovingStates.Stopped;
            }
            else
            {
                _state = MovingStates.Walking;
            }

            player.MoveAndCollide(_velocity * _inputVector * (float)delta);
        }

        public void SetInput(Vector2 input, bool jump, bool sprint, bool crouch)
        {
            _jump = jump;
            _sprint = sprint;
            _crouch = crouch;
            _inputVector = input;
            if (input.X != 0)
            {
                _inputVector.X = input.X > 0 ? 1 : -1;
            }

            if (input.Y != 0)
            {
                _inputVector.Y = input.Y > 0 ? 1 : -1;
            }

        }

        private void TryJump()
        {
        }
    }
}