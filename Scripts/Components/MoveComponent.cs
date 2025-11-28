using Game.Entities;
using Game.Core;
using Game.Enums;
using Godot;
using System.Runtime.Serialization;

namespace Game.Components
{
    public class MoveComponent : ComponentBase
    {

        #region PUBLIC ATTRIBUTES
        public MotionStates State { get { return _state; } }
        public Direction Direction
        {
            get { return _direction; }
        }
        public float JumpSpeed;
        public Vector2 MaxVelocity;
        public Vector2 SprintVelocity;
        public Vector2 Acceleration;
        public Vector2 AccelerationInAir;
        public float Friction;
        public float FrictionInAir;

        public float LandingTime = 0.1f;
        public float CoyoteTime;
        #endregion

        private MotionStates _state = MotionStates.Stopped;
        private float _sprintMultiplier = 2f;
        private Vector2 _velocity = Vector2.Zero;
        private Direction _direction = Direction.Forward;
        private float _coyoteTimer = 0f;

        private Vector2 _inputVector = Vector2.Zero;
        private bool _jump;
        private bool _sprint;
        private bool _crouch;
        private bool _onFloor;
        private float _stickyTime;

        private uint _collisionMaskOld;
        private float _dashTimer = 0;
        private Vector2 _dashVelocity = Vector2.Zero;

        #region PUBLIC API
        public MoveComponent(EntityBase entity) : base(entity)
        {
        }

        public override void Update(double delta)
        {
            _velocity = Entity.Velocity;

            var gravity = GameState.Instance.Settings.DefaultGravity;
            bool wasOnFloor = _onFloor;
            _onFloor = Entity.IsOnFloor();

            // TODO status component to get states like grounded, climbing, etc.
            if (wasOnFloor && !_onFloor)
            {
                GameLogger.Log($"Coyote time started {CoyoteTime}");
                _coyoteTimer = CoyoteTime;
            }
            else
            {
                _coyoteTimer = Mathf.MoveToward(_coyoteTimer, 0, (float)delta);
            }

            if (_state == MotionStates.Dashing)
            {
                Entity.Velocity = _dashVelocity;
                Entity.MoveAndSlide();
                _dashTimer -= (float)delta;
                if (_dashTimer >= 0)
                    return;
                EndDash();
            }

            if ((_coyoteTimer > 0 && _jump) || (_onFloor && _jump))
            {
                _velocity.Y = -JumpSpeed;
                _coyoteTimer = 0;
            }
            else if (!_onFloor)
            {
                _velocity.Y += gravity * (float)delta;
            }
            else
            {
                _velocity.Y = 0;
            }

            if (_inputVector.X != 0)
            {
                _direction = _inputVector.X > 0 ? Direction.Forward : Direction.Backward;
                Entity.SetDirection(_direction);
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

            Entity.Velocity = _velocity;
            Entity.MoveAndSlide();

            // TODO Debugging only, remove later
            if (Entity.Position.Y >= 3000)
            {
                GameLogger.Log($"Player fell out of bounds, resetting position. {Entity.Position}");
                // reset
                Entity.Position = new Vector2(0, 0);
                Entity.Velocity = Vector2.Zero;
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

        public void Dash(float time, Vector2 velocity)
        {
            if (_state == MotionStates.Dashing)
                return;
            _dashVelocity = velocity;
            _state = MotionStates.Dashing;
            _dashTimer = time;
            // during dashing we disable collision temporarily
            _collisionMaskOld = Entity.GetCollisionMask();
            Entity.SetCollisionMask(0);
        }

        public void EndDash()
        {
            // restore collision mask
            Entity.Velocity = Vector2.Zero;
            _dashVelocity = Vector2.Zero;
            _state = MotionStates.Stopped;
            _dashTimer = 0;
            Entity.SetCollisionMask(_collisionMaskOld);
        }

        #endregion

        private void UpdateState(double delta)
        {
            var currentState = _state;
            _stickyTime = Mathf.Max(_stickyTime - (float)delta, 0);
            MotionStates newState = currentState;

            if (_velocity == Vector2.Zero)
            {
                newState = MotionStates.Stopped;
            }
            else if (_velocity.Y < 0)
            {
                newState = MotionStates.Rising;
            }
            else if (_velocity.Y > 0)
            {
                newState = MotionStates.Falling;
            }
            else if (Mathf.Abs(_velocity.X) > MaxVelocity.X)
            {
                newState = MotionStates.Running;
            }
            else if (_velocity.X != 0)
            {
                newState = MotionStates.Walking;
            }

            if (currentState == MotionStates.Falling && _onFloor)
            {
                newState = MotionStates.Landing;
                _stickyTime = LandingTime;
            }

            // if the player is landing and there is no further input, remain in landing state for a while
            if (_stickyTime != 0 && currentState == MotionStates.Landing && newState == MotionStates.Stopped)
            {
                return;
            }

            _state = newState;
        }
    }
}