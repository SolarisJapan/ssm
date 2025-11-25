using Godot;
using System.Collections.Generic;
using Game.Core;
using Game.Components;
using Game.Scene;
using Game.Enums;

namespace Game.Entities
{
	public partial class Player : EntityBase
	{
		private int _scale = 3;

		#region PUBLIC API
		public override void OnEnterScene(SceneBase scene)
		{
			GameLogger.Log($"Player {EntityId} entered scene {scene.SceneID}");
			Scale = new Vector2(_scale, _scale);
		}
		public override void OnUpdate(double delta)
		{
			GetNode<Label>("StatusText").Text = $"{GetComponent<StateComponent>().State}-{GetComponent<MoveComponent>().State}";
		}

		public override void Init()
		{
			CollisionLayer = CollisionLayers.Player;
			CollisionMask = 0xFFFFFFFF & ~CollisionLayers.Player;
		}

		#endregion

		protected override List<ComponentBase> CreateAllComponents()
		{
			return new List<ComponentBase>
			{
				new PlayerInputComponent(this),
				new MoveComponent(this),
				new StateComponent(this),
				new AnimationComponent(this),
				new AbilityComponent(this)
			};
		}
	}
}
