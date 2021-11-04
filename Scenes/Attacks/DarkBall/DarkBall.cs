using Godot;
using System;

public class DarkBall : Area2D
{
	[Signal]
	public delegate void darkball_attack();
	private AnimatedSprite darkball;
	private AudioStreamPlayer soundHit;

	[Export] float speed = 700.0f;
	[Export] float lifeTime = 10.0f;
	[Export] float damage = 22.0f;

	Vector2 direction = Vector2.Right;

	public override void _Ready()
	{
		darkball = GetNode<AnimatedSprite>("SpellSprite");
		soundHit = GetNode<AudioStreamPlayer>("SoundHit");
	}

	public override void _PhysicsProcess(float delta)
	{
		lifeTime -= .1f;
		if (lifeTime > 0)
			Position += direction.Normalized() * speed * delta;
		else
			QueueFree();
	}

	public void setup(Vector2 dir)
	{
		direction = dir;
	}

	public void setPosition(Vector2 pos)
	{
		Position = pos;
	}

	private void _on_DarkBall_body_entered(object body)
	{
		if (body is Movement)
		{
			((Movement)body)._Hurt(damage);
			((Movement)body)._Dark();
		}
		if (body is IEnemy == false)
			QueueFree();
	}
}
