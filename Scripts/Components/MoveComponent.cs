using Game.Entities;
using Game.Core;
using Game.Enums;
using Godot;

namespace Game.Components
{
    public class MoveComponent : ComponentBase
    {

        public MovingStates State { get { return _state; } }
        public Direction Direction
        {
            get { return _direction; }
        }

        private float JumpSpeed = 1200;
        public Vector2 MaxVelocity = new Vector2(1200, 0);
        public Vector2 SprintVelocity = new Vector2(2400, 0);
        public Vector2 Acceleration = new Vector2(4000, 0);
        public Vector2 AccelerationInAir = new Vector2(1000, 0);
        public float Friction = 10000;
        public float FrictionInAir = 8000;
        public float Gravity = 3200;
        public float LandingTime = 0.1f;

        private MovingStates _state = MovingStates.Stopped;
        private float _sprintMultiplier = 2f;
        private Vector2 _velocity = Vector2.Zero;
        private Direction _direction = Direction.Forward;

        private Vector2 _inputVector = Vector2.Zero;
        private bool _jump;
        private bool _sprint;
        private bool _crouch;
        private bool _onFloor;
        private float _stickyTime;

        public MoveComponent(EntityBase entity) : base(entity) { }

        public override void Update(double delta)
        {
            if (Entity is not Player player)
                throw new System.Exception("MoveComponent can only be used with Player entities.");

            _velocity = player.Velocity;

            // TODO status component to get states like grounded, climbing, etc.
            _onFloor = player.IsOnFloor();

            if (!_onFloor)
            {
                _velocity.Y += Gravity * (float)delta;
            }
            else if (_jump)
            {
                _velocity.Y = -JumpSpeed;
            }
            else
            {
                _velocity.Y = 0;
            }

            if (_inputVector.X != 0)
            {
                _direction = _inputVector.X > 0 ? Direction.Forward : Direction.Backward;
            }

            Vector2 effectiveAcceleration = _onFloor ? Acceleration : AccelerationInAir;

            Vector2 effectiveMaxVelocity = _sprint ? (MaxVelocity * _sprintMultiplier) : MaxVelocity;
            effectiveMaxVelocity = (_inputVector.X != 0) ? effectiveMaxVelocity : Vector2.Zero;

            float effectiveFriction = _onFloor ? Friction : FrictionInAir;

            if (Mathf.Abs(_velocity.X) > effectiveMaxVelocity.X)
            {
                // friction
                _velocity.X += (_velocity.X > 0 ? -1 : 1) * effectiveFriction * (float)delta;
                _velocity.X = Mathf.Clamp(_velocity.X, -effectiveMaxVelocity.X, effectiveMaxVelocity.X);
            }
            else
            {
                _velocity.X += effectiveAcceleration.X * _inputVector.X * (float)delta;
                _velocity.X = Mathf.Clamp(_velocity.X, -effectiveMaxVelocity.X, effectiveMaxVelocity.X);
            }

            UpdateState(delta);

            player.Velocity = _velocity;
            player.MoveAndSlide();

            // TODO Debugging only, remove later
            if (player.Position.Y >= 3000)
            {
                GameLogger.Log($"Player fell out of bounds, resetting position. {player.Position}");
                // reset
                player.Position = new Vector2(0, 0);
            }
        }

        private void UpdateState(double delta)
        {
            var currentState = _state;
            _stickyTime = Mathf.Max(_stickyTime - (float)delta, 0);
            MovingStates newState = currentState;

            if (_velocity == Vector2.Zero)
            {
                newState = MovingStates.Stopped;
            }
            else if (_velocity.Y < 0)
            {
                newState = MovingStates.Rising;
            }
            else if (_velocity.Y > 0)
            {
                newState = MovingStates.Falling;
            }
            else if (Mathf.Abs(_velocity.X) > MaxVelocity.X)
            {
                newState = MovingStates.Running;
            }
            else if (_velocity.X != 0)
            {
                newState = MovingStates.Walking;
            }

            if (currentState == MovingStates.Falling && _onFloor)
            {
                newState = MovingStates.Landing;
                _stickyTime = LandingTime;
            }

            // if the player is landing and there is no further input, remain in landing state for a while
            if (_stickyTime != 0 && currentState == MovingStates.Landing && newState == MovingStates.Stopped)
            {
                return;
            }

            _state = newState;
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
    }
}