using Game.Entities;

namespace Game.Components
{
    public class ComponentBase
    {
        private EntityBase _entity;

        #region PUBLIC API
        public EntityBase Entity
        {
            get { return _entity; }
            private set { _entity = value; }
        }

        public ComponentBase(EntityBase entity)
        {
            Entity = entity;
        }

        virtual public void Initialize()
        {

        }

        virtual public void Update(double delta)
        {

        }
        #endregion
    }
}
