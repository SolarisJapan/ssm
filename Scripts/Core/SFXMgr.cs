using Godot;

namespace Game.Core
{
    public class SFXManager
    {
        #region PUBLIC API
        // TODO: should not return the raw godot type
        public CpuParticles2D CreateSFX(string sfxName)
        {
            var sfx = GD.Load<PackedScene>("res://Assets//Scenes/SFX/Explosion.tscn").Instantiate<CpuParticles2D>();
            sfx.Finished += () => sfx.QueueFree();
            sfx.Emitting = false;
            GameState.Instance.CurrentScene.AddChild(sfx);
            return sfx;
        }
        #endregion
    }
}