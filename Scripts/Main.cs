using Godot;
using Game.Core;
using Game.Entities;
using Game.Scene;


public partial class Main : Node
{
	[Export] public PackedScene MainPlayerScene;
	[Export] public PackedScene Scene;

	public override void _EnterTree()
	{
		GameLogger.Log("Game Main Inited.");
	}

	public override void _Ready()
	{
		GameLogger.Log("Game Main _Ready.");

		var scene = Scene.Instantiate<SceneBase>();
		AddChild(scene);

		var player = MainPlayerScene.Instantiate<Player>();
		AddChild(player);

		scene.AddEntity(player);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		TickMgr.Instance.Update(delta);
	}
}
