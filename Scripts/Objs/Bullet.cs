using Game.Core;
using Game.Enums;
using Game.Entities;
using Godot;

namespace Game.Objs
{
    public partial class Bullet : GameObjBase
    {
        #region PUBLIC ATTRIBUTES
        public EntityBase TargetEntity { get { return _targetEntity; } }
        public float Speed;
        #endregion

        private EntityBase _targetEntity;
        private Vector2 _targetPosition;
        private Vector2 _direction;
        private float _lifetime;
        private ProjectileTargetType _targetType;
        private BulletManager _manager;

        private BulletCfg _cfg;

        #region PUBLIC API
        public override void OnUpdate(double delta)
        {
            float dt = (float)delta;
            _lifetime -= dt;
            if (_lifetime <= 0)
            {
                Destroy();
                return;
            }

            switch (_targetType)
            {
                case ProjectileTargetType.FollowEntity:
                    if (TargetEntity != null)
                    {
                        Vector2 toTarget = TargetEntity.GlobalPosition - GlobalPosition;
                        _direction = toTarget.Normalized();
                        Position += _direction * Speed * dt;
                    }
                    else
                    {
                        Position += _direction * Speed * dt;
                    }
                    break;
                case ProjectileTargetType.FixedDirection:
                    Position += _direction * Speed * dt;
                    break;
                case ProjectileTargetType.FixLocation:
                    Vector2 remaining = _targetPosition - GlobalPosition;
                    if (remaining.Length() < Speed * dt)
                    {
                        GlobalPosition = _targetPosition;
                    }
                    else
                    {
                        Position += _direction * Speed * dt;
                    }
                    break;
            }

            // update rotation
            Rotation = _direction.Angle();
        }

        public override void Init()
        {
        }

        public void OnArrive()
        {
            GameLogger.Log($"Bullet: Arrived at target {GlobalPosition}, destroying bullet.");
            Destroy();
        }

        public override void OnEntityEntered(EntityBase entity)
        {
            if (entity == OwnerEntity)
            {
                return;
            }
            OnHit(entity);
        }

        public void OnHit(EntityBase entity)
        {
            entity.OnHitByObj(this);
            GameLogger.Log($"Bullet: Hit entity {entity.EntityId}, destroying bullet.");
            Destroy();
        }

        public void Destroy()
        {
            // TODO: testing SFX
            var sfx = GameState.Instance.SFXMgr.CreateSFX("whatever");
            sfx.GlobalPosition = this.GlobalPosition;
            sfx.Scale = new Vector2(1.5f, 1.5f);
            sfx.Emitting = true;

            _manager.OnDestroyBullet(this);
            QueueFree();
        }

        public void Initialize(BulletManager manager, EntityBase owner)
        {
            _manager = manager;
            OwnerEntity = owner;
        }

        public void SetTargetEntity(EntityBase target)
        {
            _targetEntity = target;
            _targetType = ProjectileTargetType.FollowEntity;
        }

        public void SetDirection(Vector2 dir)
        {
            _direction = dir.Normalized();
            _targetType = ProjectileTargetType.FixedDirection;
        }

        public void SetTargetPosition(Vector2 pos)
        {
            _targetPosition = pos;
            _direction = (_targetPosition - GlobalPosition).Normalized();
            _targetType = ProjectileTargetType.FixLocation;
        }

        #endregion

        public override void InitCfg(uint cfgId)
        {
            _lifetime = 5.0f;
            Speed = 800.0f;
        }
    }
}
