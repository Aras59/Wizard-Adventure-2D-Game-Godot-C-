using Godot;
using System;

public class WraithBoss : KinematicBody2D, IEnemy
{

	int GRAVITY = 0;
	[Export] int SPEED = 300;
	public Vector2 velocity = new Vector2();
	private int direction = 1;
	private Vector2 directionVector = Vector2.Right;

	[Export] bool canWalk = true;
	[Export] bool canRotate = true;
	[Signal] public delegate void dead(Vector2 position);
	bool isDead = false;
	bool isHurt = false;
	bool isHitting = false;
	bool isRunning = false;
	[Export] float life = 150f;
	[Export] float damage = 33f;
	Timer timer;
	Timer timer2;
	Timer throw_timer;
	private AudioStreamPlayer soundHurt;
	private AudioStreamPlayer soundAttack;
	private AudioStreamPlayer soundDeath;


	[Export] public PackedScene throw_bone;
	Random rand;
	float delay = 1f;
	Boolean rotate_flag = true;

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

		throw_timer = GetNode<Timer>("ThrowTimer");
		throw_timer.OneShot = true;
		throw_timer.WaitTime = 5f;
		throw_timer.Autostart = true;

		rand = new Random();

		throw_bone = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/Bone/Bone.tscn");

		set_active(false);
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


			if (IsOnWall())
			{
				_Rotate();
			}

			if (throw_timer.GetTimeLeft() == 0)
			{
				throw_timer.Start();
				_Hit();
			}

			int r = rand.Next(0, 1000);
			if (r >= 0 && r <= 3 && rotate_flag)
			{
				_Rotate();
			}
			if (r >= 18 && r <= 21)
			{
				rotate_flag = true;
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
		rotate_flag = false;
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
		GRAVITY = 50;
		((Movement)GetNode("/root/Root/Player/Player")).killCemetaryBoss();
		((CollisionShape2D)GetNode("CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("BehindCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((AnimatedSprite)GetNode("Wraith")).Play("Dying");
		soundDeath.Play();
		EmitSignal(nameof(dead), GlobalPosition + new Vector2(0, 80));
	}

	public void _Hurt(float damage)
	{
		Random rnd = new Random();
		;
		this.SetPosition(new Vector2(rnd.Next(150, 3650), 2300));

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
		Bone bone = (Bone)throw_bone.Instance();
		GetParent().AddChild(bone);
		float xToMove = 60.0f;
		bone.Scale = 0.5f * bone.Scale;

		if (direction > 0) directionVector = Vector2.Right;
		else directionVector = Vector2.Left;

		if (directionVector == Vector2.Left)
			xToMove *= -1;
		Vector2 where = new Vector2(xToMove, 10.0f);

		bone.GlobalPosition = GlobalPosition + where;


		bone.setup(directionVector);
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


	private void set_active(bool active)
	{
		SetPhysicsProcess(active);
		SetProcess(active);
		SetProcessInput(active);
	}


	private void _on_BossActivate_body_entered(object body)
	{
		set_active(true);
	}
}
