using Godot;
using Game.Entities;
using Game.Core;
using Game.Enums;

namespace Game.Objs
{
    public partial class GameObjBase : Area2D
    {
        #region PUBLIC ATTRIBUTES
        public bool IsReady => _ready;
        public ulong ObjId => _objId;
        public EntityBase OwnerEntity;
        #endregion

        private bool _ready = false;
        private ulong _objId;

        public GameObjBase()
        {
            _objId = GameObjID.GetNextID();
        }

        #region GODOT METHODS
        public override void _Ready()
        {
            _ready = true;
            Init();
            CollisionLayer = CollisionLayers.Objects;
            CollisionMask = 0xFFFFFFFF & ~CollisionLayers.Objects;

            BodyEntered += HandleBodyEntered;
            BodyExited += HandleBodyExited;
            AreaEntered += HandleAreaEntered;
            AreaExited += HandleAreaExited;
        }

        #region PRIVATE FIELDS
        #endregion

        public override void _EnterTree()
        {

        }

        public override void _PhysicsProcess(double delta)
        {
            OnUpdate(delta);
        }
        #endregion

        #region PUBLIC API
        public GameObjBase(EntityBase ownerEntity) : this()
        {
            OwnerEntity = ownerEntity;
        }

        virtual public void InitCfg(uint CfgId)
        {
            // TODO
        }

        virtual public void Init()
        {
        }

        virtual public void OnEntityEntered(EntityBase entity)
        {
        }

        virtual public void OnEntityExited(EntityBase entity)
        {
        }

        virtual public void OnObjEntered(GameObjBase obj)
        {
        }

        virtual public void OnObjExited(GameObjBase obj)
        {
        }

        virtual public void OnUpdate(double delta)
        {
        }
        #endregion

        private void HandleBodyEntered(Node2D body)
        {
            if (body is EntityBase entity)
            {
                OnEntityEntered(entity);
            }
            else
            {
                GameLogger.LogError($"GameObjBase.HandleBodyEntered: Body entered is not an EntityBase: {body}");
            }
        }

        private void HandleBodyExited(Node2D body)
        {
            if (body is EntityBase entity)
            {
                OnEntityExited(entity);
            }
            else
            {
                GameLogger.LogError($"GameObjBase.HandleBodyExited: Body exited is not an EntityBase: {body}");
            }
        }

        private void HandleAreaEntered(Area2D area)
        {
            if (area is GameObjBase obj)
            {
                OnObjEntered(obj);
            }
            else
            {
                GameLogger.LogError($"GameObjBase.HandleAreaEntered: Area entered is not a GameObjBase: {area}");
            }
        }

        private void HandleAreaExited(Area2D area)
        {
            if (area is GameObjBase obj)
            {
                OnObjExited(obj);
            }
            else
            {
                GameLogger.LogError($"GameObjBase.HandleAreaExited: Area exited is not a GameObjBase: {area}");
            }
        }
    }
}
