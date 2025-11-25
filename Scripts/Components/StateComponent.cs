using Game.Entities;
using Game.Enums;

namespace Game.Components
{
    public class StateComponent : ComponentBase
    {
        #region PUBLIC ATTRIBUTES
        public EntityStates State { get { return _state; } }

        #endregion

        #region PUBLIC API
        public StateComponent(EntityBase entity) : base(entity) { }
        #endregion

        private EntityStates _state = EntityStates.Idle;

        #region PUBLIC API
        public bool CanChangeState(EntityStates newState)
        {
            return true;
        }

        public bool SetState(EntityStates state)
        {
            if (!CanChangeState(state))
                return false;

            _state = state;
            return true;
        }

        #endregion
    }
}