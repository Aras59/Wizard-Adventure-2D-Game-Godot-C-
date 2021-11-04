using Godot;
using System;

public class Cemeterylvl3 : Node2D
{
	private Area2D portal;
	static private LevelEndPanel level;
	private Node2D Player;
	private Movement player;
	public override void _Ready()
	{
		portal = GetNode<Area2D>("ToTown");
		level = GetNode<LevelEndPanel>("LevelEndPanel");
		Player = GetNode<Node2D>("Player");
		player = (Movement)Player.GetNode<Movement>("Player");
	}

	private void _on_ToTown_body_entered(object body)
	{
		GetTree().Paused = true;
		level.changeVisible();
		player.setFullHp();
	}

	private void _on_EnemyWraithBoss_dead(Vector2 position)
	{
		portal.Visible = true;
		portal.GlobalPosition = new Vector2(1850, 2700);
	}
}
