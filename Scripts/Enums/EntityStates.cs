namespace Game.Enums
{
    public enum EntityStates
    {
        Idle,
        CastingAbility,
        Dead
    }

    public enum AbilityStates
    {
        Ready,
        Casting,
        Cooldown
    }

    public enum MotionStates
    {
        Stopped,

        JumpStart,
        Rising,
        Falling,
        Flying,
        Hovering,
        Landing,
        Walking,
        Running,
        Climbing,
        CrouchDown,
        CrouchIdle,
        CrouchMoving,
        CrouchUp
    }

    public enum Direction
    {
        Forward,
        Backward
    }
}