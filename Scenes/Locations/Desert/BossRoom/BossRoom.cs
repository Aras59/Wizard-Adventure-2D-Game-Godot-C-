using Godot;
using System;

public class BossRoom : Node2D
{
	private Area2D portal;
	static private LevelEndPanel level;
	private Node2D Player;
	private Movement player;
	public override void _Ready()
	{
		portal = GetNode<Area2D>("DesertToTownPortal");
		level = GetNode<LevelEndPanel>("LevelEndPanel");
		Player = GetNode<Node2D>("Player");
		player = (Movement)Player.GetNode<Movement>("Player");
	}

	private void _on_EnemyGoblinBoss_dead(Vector2 position)
	{
		portal.Visible = true;
		portal.GlobalPosition = position;
	}

	private void _on_DesertToTownPortal_body_entered(object body)
	{
		GetTree().Paused = true;
		level.changeVisible();
		player.setFullHp();
	}
}
