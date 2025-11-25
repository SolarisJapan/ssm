using Game.Objs;
using Game.Entities;
using System.Collections.Generic;
using Godot;
using Game.Scene;
using System;

namespace Game.Core
{

    public struct BulletCfg
    {
        public int BulletCfgId;
        public float Speed;
        public float Lifetime;
        public Vector2 Direction;
        public Vector2 SpawnPosition;

        public string PrefabPath;

        public BulletCfg(int bulletCfgId)
        {
            // TODO: test data, load from config file later
            BulletCfgId = bulletCfgId;
            Speed = 400.0f;
            Lifetime = 5.0f;
            Direction = new Vector2(1, 0);
            SpawnPosition = new Vector2(0, 0);

        }
    }

    public class BulletManager
    {
        private SceneBase Scene;

        private Dictionary<ulong, HashSet<ulong>> _bulletsByOwner = new();

        #region PUBLIC API

        public BulletManager()
        {
        }

        public void SetScene(SceneBase scene)
        {
            Scene = scene;
        }

        public Bullet FireAt(uint BulletCfgId, EntityBase owner, EntityBase target, Vector2 spawnPosition)
        {
            var bullet = CreateBullet(BulletCfgId, owner);
            bullet.SetTargetEntity(target);
            bullet.SetPosition(spawnPosition);
            return bullet;
        }

        public Bullet FireTowards(uint BulletCfgId, EntityBase owner, Vector2 spawnPosition, Vector2 targetPosition)
        {
            var bullet = CreateBullet(BulletCfgId, owner);
            bullet.SetTargetPosition(targetPosition);
            bullet.SetPosition(spawnPosition);
            return bullet;
        }

        public Bullet FireInDirection(uint BulletCfgId, EntityBase owner, Vector2 spawnPosition, Vector2 direction)
        {
            var bullet = CreateBullet(BulletCfgId, owner);
            bullet.SetDirection(direction);
            bullet.SetPosition(spawnPosition);
            return bullet;
        }

        public void OnDestroyBullet(Bullet bullet)
        {
            if (_bulletsByOwner.TryGetValue(bullet.OwnerEntity.EntityId, out var bulletSet))
            {
                bulletSet.Remove(bullet.ObjId);
                if (bulletSet.Count == 0)
                {
                    _bulletsByOwner.Remove(bullet.OwnerEntity.EntityId);
                }
            }
        }

        #endregion

        private Bullet CreateBullet(uint cfgId, EntityBase owner)
        {
            if (Scene == null)
            {
                throw new System.Exception("BulletManager: Scene is not set. Cannot create bullet.");
            }

            var prefabPath = PrefabPath(cfgId);
            var bulletNode = GD.Load<PackedScene>(prefabPath).Instantiate();
            if (bulletNode is not Bullet bullet)
            {
                throw new InvalidCastException($"BulletManager: Prefab at '{prefabPath}' must have a root script of type {nameof(Bullet)}.");
            }

            bullet.Initialize(this, owner);
            GameLogger.Log($"BulletManager: Creating bullet {bullet}");
            bullet.InitCfg(cfgId);
            _bulletsByOwner.TryAdd(owner.EntityId, new HashSet<ulong>());
            _bulletsByOwner[owner.EntityId].Add(bullet.ObjId);

            Scene.AddChild(bullet);
            return bullet;
        }

        private string PrefabPath(uint cfgId)
        {
            return "res://Assets/Scenes/Bullets/Bullet.tscn";
        }
    }
}
