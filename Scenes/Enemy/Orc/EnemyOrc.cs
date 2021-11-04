using Godot;
using System;

public class EnemyOrc : KinematicBody2D, IEnemy
{
	const int GRAVITY = 30;
	int SPEED = 150;
	public Vector2 velocity = new Vector2();
	private int direction = 1;
	bool isDead = false;
	bool isHurt = false;
	bool isHitting = false;
	bool isRunning = false;
	float life = 150f;
	float damage = 33f;
	Timer timer;
	Timer timer2;
	private AudioStreamPlayer soundHurt;
	private AudioStreamPlayer soundAttack;
	private AudioStreamPlayer soundDie;

	float delay = 0.15f;

	public override void _Ready()
	{
		soundHurt = GetNode<AudioStreamPlayer>("SoundHurt");
		soundAttack = GetNode<AudioStreamPlayer>("SoundAttack");
		soundDie = GetNode<AudioStreamPlayer>("SoundDie");

		timer = GetNode<Timer>("Timer");
		timer.OneShot = true;
		timer.WaitTime = delay;
		timer.Autostart = true;

		timer2 = GetNode<Timer>("Timer2");
		timer2.OneShot = true;
		timer2.WaitTime = 3f;
		timer2.Autostart = true;
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!isDead && !isHurt && !isHitting)
		{
			if (direction == -1)
			{
				((AnimatedSprite)GetNode("Orc")).FlipH = true;
			}
			else
			{
				((AnimatedSprite)GetNode("Orc")).FlipH = false;
			}

			velocity.x = SPEED * direction;
			if (!isRunning)
				((AnimatedSprite)GetNode("Orc")).Play("Walk");
			else
				((AnimatedSprite)GetNode("Orc")).Play("Running");
			velocity.y += GRAVITY;
			velocity = MoveAndSlide(velocity, Vector2.Up);

			if (IsOnWall() || ((RayCast2D)GetNode("RayCast2D")).IsColliding() == false && IsOnFloor())
			{
				_Rotate();
			}

		}
		else if (isHitting && timer.GetTimeLeft() == 0)
		{
			((Area2D)GetNode("AttackDetector")).Monitoring = true;
		}
		if (isRunning && timer2.GetTimeLeft() == 0)
		{
			SPEED = 150;
			isRunning = false;
		}
	}

	private void _Rotate()
	{
		direction = direction * -1;
		
		Vector2 vector = ((RayCast2D)GetNode("RayCast2D")).Position;
		vector.x = vector.x * -1;
		((RayCast2D)GetNode("RayCast2D")).Position = vector;
		((Area2D)GetNode("AttackDetector")).Scale = new Vector2(-((Area2D)GetNode("AttackDetector")).Scale.x, ((Area2D)GetNode("AttackDetector")).Scale.y);
		((Area2D)GetNode("PlayerCollisionDetector")).Scale = new Vector2(-((Area2D)GetNode("PlayerCollisionDetector")).Scale.x, ((Area2D)GetNode("PlayerCollisionDetector")).Scale.y);
		((Area2D)GetNode("BehindCollisionDetector")).Scale = new Vector2(-((Area2D)GetNode("BehindCollisionDetector")).Scale.x, ((Area2D)GetNode("BehindCollisionDetector")).Scale.y);
	}

	public void _Dead()
	{
		isDead = true;
		((CollisionShape2D)GetNode("CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("AttackDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("BehindCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((AnimatedSprite)GetNode("Orc")).Play("Dying");
		soundDie.Play();

		PackedScene co = (PackedScene)ResourceLoader.Load("res://Scenes/Coins/Coin.tscn");
		Coin coin = (Coin)co.Instance();
		HUD hud = ((HUD)GetNode("/root/Root/HUD"));
		GetParent().AddChild(coin);
		coin.Connect("coin_collected", hud, nameof(hud._on_Coin_coin_collected));
		coin.Position = this.GetPosition();

	}

	public void _Hurt(float damage)
	{
		life -= damage;
		if (life <= 0) _Dead();
		else
		{
			isHurt = true;
			((AnimatedSprite)GetNode("Orc")).Stop();
			((AnimatedSprite)GetNode("Orc")).Play("Hurt");
			soundHurt.Play();
		}
	}

	public void _Hit()
	{
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		isHitting = true;
		((AnimatedSprite)GetNode("Orc")).Stop();
		((AnimatedSprite)GetNode("Orc")).Play("Attack");
		soundAttack.Play();
		timer.Start();
	}

	public void _EndOfHit()
	{
		isHitting = false;
		((Area2D)GetNode("AttackDetector")).Monitoring = false;
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", false);
	}

	private void _on_Orc_animation_finished()
	{
		if (isDead)
			QueueFree();
		if (isHurt)
		{
			isHurt = false;
			isRunning = true;
			timer2.Start();
			SPEED = 300;
		}

		if (isHitting)
			_EndOfHit();
	}

	private void _on_PlayerCollisionDetector_body_entered(object body)
	{
		if (body is Movement)
		{
			_Hit();
		}
	}

	private void _on_AttackDetector_body_entered(object body)
	{
		if (body is Movement)
		{
			((Movement)body)._Hurt(damage);
		}
	}
	private void _on_BehindCollisionShape2D_body_entered(object body)
	{
		timer2.Start();
		SPEED = 300;
		isRunning = true;
		_Rotate();
	}
}
