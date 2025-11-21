using Godot;

namespace Game.Core
{
    class GameLogger
    {
        private GameLogger() { }
        public static void Log(string message)
        {
            GD.Print(message);
        }

        public static void Debug(string message)
        {
            GD.Print(message);
        }

        public static void LogError(string message)
        {
            GD.PrintErr(message);
        }

        public static void LogWarning(string message)
        {
            GD.PushWarning(message);
        }
    }
}