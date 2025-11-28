using Godot;
using Game.Components;
using Game.Enums;
using Game.Entities;
using System;

namespace Game.Core
{
    public class AbilityObj
    {
        public float _currentPipeTimeRemain = 0f;
        public bool Done { get { return _done; } }
        private int _currentPipe;
        private int _totalPipes = 3;
        public EntityBase _caster;
        private bool _done = false;

        public AbilityObj(EntityBase caster)
        {
            _caster = caster;
            _currentPipe = 0;
            _currentPipeTimeRemain = 0;
        }

        public void Update(double delta)
        {
            if (_currentPipe > _totalPipes)
                return;

            _currentPipeTimeRemain = Mathf.MoveToward(_currentPipeTimeRemain, 0f, (float)delta);

            if (_currentPipeTimeRemain == 0)
            {
                if (_currentPipe > _totalPipes)
                {
                    _currentPipeTimeRemain = 0;
                }
                else
                {
                    _currentPipeTimeRemain = 0.2f;
                }

                GameLogger.Log($"Ability Pipe {_currentPipe} activated");
                _currentPipe += 1;
                if (_currentPipe > _totalPipes)
                {
                    _done = true;
                    return;
                }

                var moveComponent = _caster.GetComponent<MoveComponent>();
                switch (_currentPipe)
                {
                    case 1:
                        _currentPipeTimeRemain = 0.2f;
                        moveComponent.Dash(0.15f, new Vector2(
                            moveComponent.Direction == Direction.Forward ? 1000 : -1000,
                            0));
                        GameLogger.Log("Dash forward");
                        break;
                    case 2:
                        GameState.Instance.BulletMgr.FireInDirection(1, _caster, _caster.Position,
                            moveComponent.Direction == Direction.Forward ? new Vector2(1, 0) : new Vector2(-1, 0)
                        );

                        _currentPipeTimeRemain = 0;
                        GameLogger.Log("Fire bullet");
                        break;
                    case 3:
                        _currentPipeTimeRemain = 0.2f;
                        moveComponent.Dash(0.15f, new Vector2(
                            moveComponent.Direction == Direction.Forward ? -1000 : 1000,
                            0));
                        GameLogger.Log("Dash backward");
                        break;
                }
            }
        }
    }
}