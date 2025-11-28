using Godot;
using System.Collections.Generic;
using Game.Core;
using Game.Components;
using Game.Scene;
using Game.Core;
using Game.Enums;

namespace Game.Entities
{
	public partial class Player : EntityBase
	{
		[Export]
		public Vector2 InitialPosition
		{
			get => Position;
			set => Position = value;
		}

		[Export]
		public float Acceleration
		{
			get => GetComponent<MoveComponent>().Acceleration.X;
			set => GetComponent<MoveComponent>().Acceleration = new Vector2(value, 0);
		}

		[Export]
		public float AccelerationInAir
		{
			get => GetComponent<MoveComponent>().AccelerationInAir.X;
			set => GetComponent<MoveComponent>().AccelerationInAir = new Vector2(value, 0);
		}

		[Export]
		public float Gravity
		{
			get => GameState.Instance.Settings.DefaultGravity;
			set => GameState.Instance.Settings.DefaultGravity = value;
		}

		[Export]
		public float MaxVelocityX
		{
			get => GetComponent<MoveComponent>().MaxVelocity.X;
			set
			{
				GameLogger.Log($"Setting MaxVelocity.X to {value}");
				GetComponent<MoveComponent>().MaxVelocity.X = value;
			}
		}

		[Export]
		public float Friction
		{
			get => GetComponent<MoveComponent>().Friction;
			set => GetComponent<MoveComponent>().Friction = value;
		}

		[Export]
		public float FrictionInAir
		{
			get => GetComponent<MoveComponent>().FrictionInAir;
			set => GetComponent<MoveComponent>().FrictionInAir = value;
		}

		[Export]
		public float JumpSpeed
		{
			get => GetComponent<MoveComponent>().JumpSpeed;
			set => GetComponent<MoveComponent>().JumpSpeed = value;
		}

		[Export]
		public float CoyoteTime
		{
			get => GetComponent<MoveComponent>().CoyoteTime;
			set => GetComponent<MoveComponent>().CoyoteTime = value;
		}

		#region PUBLIC API
		public override void OnEnterScene(SceneBase scene)
		{
			GameLogger.Log($"Player {EntityId} entered scene {scene.SceneID}");
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
