using Godot;
using System;

public class Desertlvl0 : Node2D
{
	static private LevelEndPanel level;
	public override void _Ready()
	{
		level = GetNode<LevelEndPanel>("LevelEndPanel");
	}
	
	private void _on_NextLevelArea_body_entered(object body)
	{
		GetTree().Paused = true;
		level.changeVisible();
	}
}


