using Godot;
using Game.Core;
using Game.Components;
using Game.Scene;
using System.Data;

namespace Game.Entities
{
	public partial class Player : EntityBase
	{
		private int _scale = 3;
		public override void OnEnterScene(SceneBase scene)
		{
			GameLogger.Log($"Player {EntityId} entered scene {scene.SceneID}");
			Scale = new Vector2(_scale, _scale);
		}

		protected override void InitializeComponents()
		{
			_components.Add(new PlayerInputComponent(this));
			_components.Add(new MoveComponent(this));
			_components.Add(new StateComponent(this));
			_components.Add(new AnimationComponent(this));

			for (int i = 0; i < _components.Count; i++)
			{
				_components[i].Initialize();
			}
		}

		public override void OnUpdate(double delta)
		{
			GetNode<Label>("StatusText").Text = GetComponent<MoveComponent>().State.ToString();
		}
	}
}
