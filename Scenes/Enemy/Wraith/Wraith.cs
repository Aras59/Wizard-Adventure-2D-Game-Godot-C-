using Godot;
using System;

public class Wraith : KinematicBody2D, IEnemy
{

	const int GRAVITY = 30;
	[Export] int SPEED = 150;
	public Vector2 velocity = new Vector2();
	private int direction = 1;
	private Vector2 directionVector = Vector2.Right;

	[Export] bool canWalk = true;
	[Export] bool canRotate = true;

	bool isDead = false;
	bool isHurt = false;
	bool isHitting = false;
	bool isRunning = false;
	[Export] float life = 180f;
	[Export] float damage = 33f;
	Timer timer;
	Timer timer2;
	private AudioStreamPlayer soundHurt;
	private AudioStreamPlayer soundAttack;
	private AudioStreamPlayer soundDeath;


	[Export] public PackedScene throw_darkball;

	float delay = 1f;

	public override void _Ready()
	{
		soundHurt = GetNode<AudioStreamPlayer>("SoundHurt");
		soundAttack = GetNode<AudioStreamPlayer>("SoundAttack");
		soundDeath = GetNode<AudioStreamPlayer>("SoundDeath");

		timer = GetNode<Timer>("Timer");
		timer.OneShot = true;
		timer.WaitTime = delay;
		timer.Autostart = true;

		timer2 = GetNode<Timer>("Timer2");
		timer2.OneShot = true;
		timer2.WaitTime = 3f;
		timer2.Autostart = true;

		throw_darkball = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/DarkBall/DarkBall.tscn");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!isDead && !isHurt && !isHitting)
		{


			if (canWalk)
			{
				velocity.x = SPEED * direction;
				if (!isRunning)
					((AnimatedSprite)GetNode("Wraith")).Play("Walk");

			}
			else
			{
				((AnimatedSprite)GetNode("Wraith")).Play("Idle");
			}

			velocity = MoveAndSlide(velocity, Vector2.Up);
			velocity.y += GRAVITY;


			if (IsOnWall() || ((RayCast2D)GetNode("RayCast2D")).IsColliding() == false && IsOnFloor() && canRotate)
			{
				_Rotate();
			}

		}
		else if (isHitting && timer.GetTimeLeft() == 0)
		{
			if (!isDead)
			{
				_EndOfHit();
			}
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
		((Area2D)GetNode("PlayerCollisionDetector")).Scale = new Vector2(-((Area2D)GetNode("PlayerCollisionDetector")).Scale.x, ((Area2D)GetNode("PlayerCollisionDetector")).Scale.y);
		((Area2D)GetNode("BehindCollisionDetector")).Scale = new Vector2(-((Area2D)GetNode("BehindCollisionDetector")).Scale.x, ((Area2D)GetNode("BehindCollisionDetector")).Scale.y);
		if (direction == -1)
		{
			((AnimatedSprite)GetNode("Wraith")).FlipH = true;
		}
		else
		{
			((AnimatedSprite)GetNode("Wraith")).FlipH = false;
		}
	}

	public void _Dead()
	{
		isDead = true;
		((CollisionShape2D)GetNode("CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("BehindCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((AnimatedSprite)GetNode("Wraith")).Play("Dying");
		soundDeath.Play();

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
			((AnimatedSprite)GetNode("Wraith")).Stop();
			((AnimatedSprite)GetNode("Wraith")).Play("Hurt");
			soundHurt.Play();
		}
	}

	public void _Hit()
	{
		isHitting = true;
		((AnimatedSprite)GetNode("Wraith")).Stop();
		((AnimatedSprite)GetNode("Wraith")).Play("Attack");
		soundAttack.Play();
		Throw();

		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);

		timer.Start();
	}

	private void Throw()
	{
		DarkBall darkBall = (DarkBall)throw_darkball.Instance();
		GetParent().AddChild(darkBall);
		float xToMove = 60.0f;

		if (direction > 0) directionVector = Vector2.Right;
		else directionVector = Vector2.Left;

		if (directionVector == Vector2.Left)
			xToMove *= -1;
		Vector2 where = new Vector2(xToMove, 10.0f);

		darkBall.GlobalPosition = GlobalPosition + where;


		darkBall.setup(directionVector);
	}

	public void _EndOfHit()
	{
		isHitting = false;
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
			((AnimatedSprite)GetNode("Wraith")).Play("Idle");
	}

	private void _on_PlayerCollisionDetector_body_entered(object body)
	{
		if (body is Movement)
		{
			_Hit();
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
