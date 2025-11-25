using Godot;
using System;
using Game.Entities;
using Game.Core;
using Game.Enums;
using System.Collections.Generic;


namespace Game.Components
{
    public class AnimationComponent : ComponentBase
    {
        private class AnimationInfo
        {
            public string StartAnimation;
            public float StartAnimationSpeed;
            public string LoopAnimation;

            public float LoopAnimationSpeed;
            public AnimationInfo(string start, float startSpeed, string loop, float loopSpeed)
            {
                StartAnimation = start;
                LoopAnimation = loop;
                StartAnimationSpeed = startSpeed;
                LoopAnimationSpeed = loopSpeed;
            }
        }

        private static Dictionary<MotionStates, AnimationInfo> _animationMap = new()
        {
            { MotionStates.Stopped, new AnimationInfo("idle", 1, "idle", 1) },
            { MotionStates.Rising, new AnimationInfo("rise_begin", 1, "rise", 1) },
            { MotionStates.Falling, new AnimationInfo("fall_begin", 1, "fall", 1) },
            { MotionStates.Landing, new AnimationInfo("land", 1, "land", 1) },
            { MotionStates.Walking, new AnimationInfo("run_begin", 1, "run", 1) },
            { MotionStates.Running, new AnimationInfo("run", 2, "run", 2) },
        };

        private AnimationPlayer _animationPlayer;
        private AnimationInfo _currentAnimationInfo;

        #region PUBLIC API
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
            if (_animationPlayer == null)
                return;

            AnimationInfo animationInfo = GetAnimationInfo();
            if (animationInfo == null)
            {
                _animationPlayer.Stop();
                return;
            }

            int newAnimationPart = _currentAnimationPart;
            if (_currentAnimationInfo != animationInfo)
            {
                _currentAnimationInfo = animationInfo;
                newAnimationPart = 0;
            }
            else if (_currentAnimationInfo != null)
            {
                if (_animationPlayer.CurrentAnimationPosition >= _animationPlayer.CurrentAnimationLength)
                    newAnimationPart++;
            }

            var animationName = newAnimationPart == 0 ? animationInfo.StartAnimation : animationInfo.LoopAnimation;
            var animationSpeed = newAnimationPart == 0 ? animationInfo.StartAnimationSpeed : animationInfo.LoopAnimationSpeed;

            if (animationName == _animationPlayer.CurrentAnimation && animationSpeed != _animationPlayer.SpeedScale)
            {
                _animationPlayer.SpeedScale = animationSpeed;
                return;
            }

            if (animationName != _animationPlayer.CurrentAnimation || newAnimationPart != _currentAnimationPart)
            {
                var animation = _animationPlayer.GetAnimation(animationName);
                if (animation != null)
                {
                    GameLogger.Log($"AnimationComponent: Playing animation '{animationName}' on Entity {Entity.EntityId} type: {Entity.GetType().Name}.");
                    animation.LoopMode = Animation.LoopModeEnum.None;
                    _animationPlayer.SpeedScale = animationSpeed;
                    _animationPlayer.Play(animationName);
                }
                else
                {
                    GameLogger.LogError($"AnimationComponent: Animation '{animationName}' not found on Entity {Entity.EntityId} type: {Entity.GetType().Name}.");
                }
            }

            _currentAnimationPart = newAnimationPart;
        }
        #endregion

        private int _currentAnimationPart = 0; // 0 = start, 1+ = loop

        private AnimationInfo GetAnimationInfo()
        {
            var moveComponent = Entity.GetComponent<MoveComponent>();
            if (moveComponent == null)
                return null;

            if (_animationMap.TryGetValue(moveComponent.State, out var animationInfo))
                return animationInfo;

            return null;
        }
    }

}