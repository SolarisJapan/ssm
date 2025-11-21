using Godot;
using System;
using Game.Entities;
using Game.Core;
using Game.Enums;


namespace Game.Components
{
    public class AnimationComponent : ComponentBase
    {
        AnimationPlayer _animationPlayer;

        public AnimationComponent(EntityBase entity) : base(entity) { }

        public override void Initialize()
        {
            _animationPlayer = Entity.GetNode<AnimationPlayer>("AnimationPlayer");
            if (_animationPlayer == null)
            {
                GameLogger.LogError($"AnimationComponent: AnimationPlayer not found on Entity {Entity.EntityId} type: {Entity.GetType().Name}.");
            }
        }

        public override void Update(double _delta)
        {
            if (_animationPlayer != null)
            {
                string animationName = GetAnimationName();
                if (_animationPlayer.CurrentAnimation != animationName)
                {
                    if (animationName == null)
                    {
                        _animationPlayer.Stop();
                    }
                    else
                    {
                        var animation = _animationPlayer.GetAnimation(animationName);
                        if (animation != null)
                        {
                            animation.LoopMode = Animation.LoopModeEnum.Linear;
                            _animationPlayer.Play(animationName);
                        }
                        else
                        {
                            GameLogger.LogError($"AnimationComponent: Animation '{animationName}' not found on Entity {Entity.EntityId} type: {Entity.GetType().Name}.");
                        }
                    }
                }
            }
        }

        private string GetAnimationName()
        {
            var player = Entity as Player;
            var moveComponent = player.GetComponent<MoveComponent>();
            if (moveComponent.State == MovingStates.Stopped)
            {
                return "idle";
            }

            return null;
        }
    }

}