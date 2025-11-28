using Game.Entities;
using Game.Core;
using Game.Enums;
using Game.Objs;
using Godot;

namespace Game.Components
{
    public partial class AbilityComponent : ComponentBase
    {
        #region PUBLIC ATTRIBUTES
        public AbilityStates State { get { return _state; } }

        #endregion

        private float _currentAbilityRemainTime = 0f;
        private AbilityStates _state = AbilityStates.Ready;

        private AbilityObj _currentAbilityObj;
        private AbilityEnum _currentAbility;

        #region PUBLIC API
        public AbilityComponent(EntityBase entity) : base(entity) { }

        public override void Update(double delta)
        {
            if (_currentAbilityObj != null)
            {
                _currentAbilityObj.Update(delta);
                if (_currentAbilityObj.Done)
                {
                    _currentAbilityObj = null;
                    _state = AbilityStates.Ready;
                }
                return;
            }

            if (_currentAbilityRemainTime > 0)
            {
                _currentAbilityRemainTime -= (float)delta;
                if (_currentAbilityRemainTime <= 0)
                {
                    // we shouldnt need to concern about whether we successfully change state back to idle here
                    Entity.GetComponent<StateComponent>().SetState(EntityStates.Idle);
                    _currentAbilityRemainTime = 0;
                }
            }
        }

        public bool CanCast(AbilityEnum abilityType)
        {
            return _currentAbilityRemainTime <= 0;
        }

        public bool CastAbility(AbilityEnum abilityType)
        {
            if (!CanCast(abilityType))
                return false;

            var stateComponent = Entity.GetComponent<StateComponent>();
            if (!stateComponent.SetState(EntityStates.CastingAbility))
                return false;

            _state = AbilityStates.Casting;
            var moveComponent = Entity.GetComponent<MoveComponent>();

            _currentAbility = abilityType;

            if (abilityType == AbilityEnum.Attack)
            {
                GameState.Instance.BulletMgr.FireInDirection(1, Entity, Entity.Position,
                    moveComponent.Direction == Direction.Forward ? new Vector2(1, 0) : new Vector2(-1, 0)
                );
            }
            else if (abilityType == AbilityEnum.Attack1)
            {
                _currentAbilityObj = new AbilityObj(Entity);
            }

            GameLogger.Log($"Enity {Entity.EntityId} cast ability {abilityType}");
            _currentAbilityRemainTime = 0.5f;
            return true;
        }

        #endregion


    }
}