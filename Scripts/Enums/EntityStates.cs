namespace Game.Enums
{
    public enum EntityStates
    {
        Idle,
        Moving,
        Attacking,
        Dead
    }

    public enum MovingStates
    {
        Stopped,

        JumpStart,
        Rising,
        Falling,
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