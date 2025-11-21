using Godot;
using System.Collections.Generic;
using Game.Entities;

namespace Game.Scene
{
	public partial class SceneBase : Node
	{
		public ulong SceneID => _sceneID;
		public bool IsReady => _ready;

		private ulong _sceneID;
		private bool _ready = false;
		private List<EntityBase> _entities = [];

		public SceneBase()
		{
			_sceneID = Core.GameObjID.GetNextID();
		}

		// GODOT METHODS --- BEGIN ---
		public override void _Ready()
		{
			_ready = true;
			Init();
		}

		public override void _Process(double delta)
		{
		}

		public override void _PhysicsProcess(double delta)
		{
			Update(delta);
		}

		public override void _EnterTree()
		{
		}

		// GODOT METHODS --- END ---

		virtual public void Init()
		{
		}

		virtual public void OnReady()
		{

		}

		protected void Update(double delta)
		{
			OnUpdate(delta);
		}

		virtual public void OnUpdate(double delta)
		{

		}

		public void Destroy()
		{
			for (int i = 0; i < _entities.Count; i++)
			{
				RemoveEntity(_entities[i]);
			}

			OnDestroy();
		}

		virtual public void OnDestroy()
		{

		}

		public void AddEntity(EntityBase entity)
		{
			_entities.Add(entity);
			entity.EnterScene(this);
			OnEntityEnterScene(entity);
		}

		public void RemoveEntity(EntityBase entity)
		{
			_entities.Remove(entity);
			entity.ExitScene(this);
			OnEntityExitScene(entity);
		}

		virtual public void OnEntityEnterScene(EntityBase entity)
		{
		}

		virtual public void OnEntityExitScene(EntityBase entity)
		{
		}
	}
}