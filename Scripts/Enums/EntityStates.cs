namespace Game.Enums
{
    public enum EntityStates
    {
        Idle,
        Moving,
        Attacking,
        Dead
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