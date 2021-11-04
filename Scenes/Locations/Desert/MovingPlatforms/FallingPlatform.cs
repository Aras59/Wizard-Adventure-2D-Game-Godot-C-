using Godot;
using System;

public class FallingPlatform : KinematicBody2D
{
	private AnimationPlayer _animationPlayer;
	private Timer resetTimer;
	private Vector2 velocity = new Vector2();
	[Export] public int gravity = 250;
	[Export] public float resetTime = 3.0f;
	private bool isTriggered = false;
	private Vector2 resetPosition;

	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		resetTimer = GetNode<Timer>("ResetTimer");
		resetPosition = Position;
		SetPhysicsProcess(false);
	}

	public void collide_with(KinematicCollision2D collision, KinematicBody2D collider)
	{
		if (!isTriggered)
		{
			isTriggered = true;
			_animationPlayer.Play("shake");
			velocity = Vector2.Zero;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		velocity.y += gravity * delta * 10;
		Position += velocity * delta;
	}


	private void _on_AnimationPlayer_animation_finished(String anim_name)
	{
		SetPhysicsProcess(true);
		resetTimer.Start(resetTime);
	}

	private void _on_ResetTimer_timeout()
	{
		SetPhysicsProcess(false);
		isTriggered = false;
		Position = resetPosition;
	}
}
