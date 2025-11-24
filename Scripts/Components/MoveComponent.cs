using Game.Entities;
using Game.Core;
using Game.Enums;
using Godot;

namespace Game.Components
{
    public class MoveComponent : ComponentBase
    {

        public MovingStates State { get { return _state; } }

        private float JumpSpeed = 1200;
        public Vector2 MaxVelocity = new Vector2(1200, 0);
        public Vector2 SprintVelocity = new Vector2(2400, 0);
        public Vector2 Acceleration = new Vector2(4000, 0);
        public Vector2 AccelerationInAir = new Vector2(1000, 0);
        public float Friction = 10000;
        public float FrictionInAir = 8000;

        public float Gravity = 3200;

        private MovingStates _state = MovingStates.Stopped;
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

            var velocity = player.Velocity;

            // TODO status component to get states like grounded, climbing, etc.
            bool onFloor = player.IsOnFloor();

            if (!onFloor)
            {
                velocity.Y += Gravity * (float)delta;
            }
            else if (_jump)
            {
                velocity.Y = -JumpSpeed;
            }
            else
            {
                velocity.Y = 0;
            }

            // TODO: we need to use input.y for climbing
            Vector2 effectiveAcceleration = onFloor ? Acceleration : AccelerationInAir;
            if (_inputVector.X == 0)
            {
                var effectiveFriction = onFloor ? Friction : FrictionInAir;
                GameLogger.Log($"Input X zzzero Velocity X: {velocity.X} ");
                // friction
                if (velocity.X != 0)
                {
                    velocity.X -= (velocity.X > 0 ? 1 : -1) * effectiveFriction * (float)delta;
                    velocity.X = Mathf.Max(velocity.X, 0);
                }
            }
            else
            {
                velocity.X += effectiveAcceleration.X * _inputVector.X * (float)delta;
                velocity.X = Mathf.Clamp(velocity.X, -MaxVelocity.X, MaxVelocity.X);
                GameLogger.Log($"Input X: {_inputVector.X} Velocity X: {velocity.X} ");
            }

            player.Velocity = velocity;

            if (player.Velocity == Vector2.Zero)
            {
                _state = MovingStates.Stopped;
            }
            else
            {
                _state = MovingStates.Walking;
            }

            player.MoveAndSlide();

            if (player.Position.Y >= 3000)
            {
                GameLogger.Log($"Player fell out of bounds, resetting position. {player.Position}");
                // reset
                player.Position = new Vector2(0, 0);
            }
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