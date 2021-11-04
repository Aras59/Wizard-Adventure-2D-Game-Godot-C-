using Godot;
using System;

public class EnemyGoblinBoss : KinematicBody2D, IEnemy
{
	const int GRAVITY = 30;
	int SPEED = 150;
	public Vector2 velocity = new Vector2();
	private int direction = 1;
	bool isDead = false;
	bool isHurt = false;
	bool isHitting = false;
	bool isRunning = false;
	bool isThrowing = false;
	float life = 300f;
	float damage = 66f;
	Timer timer;
	Timer timer2;
	Timer shot_timer;
	private AudioStreamPlayer soundHurt;
	private AudioStreamPlayer soundAttack;
	private AudioStreamPlayer soundDie;
	int hitted_times = 0;
	[Export] public PackedScene town_portal;
	[Export] public PackedScene throw_rock;
	[Signal] public delegate void dead(Vector2 position);

	float throw_delay = 1f;
	float delay = 0.15f;
	private Vector2 directionVector = Vector2.Right;

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

		shot_timer = GetNode<Timer>("Timer3");
		shot_timer.OneShot = true;
		shot_timer.WaitTime = 4f;
		shot_timer.Autostart = true;

		throw_rock = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/Rock/Rock.tscn");
		town_portal = (PackedScene)ResourceLoader.Load("res://Scenes/Locations/Desert/BossRoom/DesertToTownPortal.tscn");
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
		else if (shot_timer.GetTimeLeft() == 0)
		{
			((Area2D)GetNode("ThrowCollision")).Monitoring = true;
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
		((Area2D)GetNode("ThrowCollision")).Scale = new Vector2(-((Area2D)GetNode("ThrowCollision")).Scale.x, ((Area2D)GetNode("ThrowCollision")).Scale.y);
	}

	public void _Dead()
	{
		isDead = true;
		((Movement)GetNode("/root/Root/Player/Player")).killDesertBoss();
		((CollisionShape2D)GetNode("CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("AttackDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("BehindCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((AnimatedSprite)GetNode("Orc")).Play("Dying");
		soundDie.Play();
		EmitSignal(nameof(dead), GlobalPosition + new Vector2(0, 80));
	}

	public void _Hurt(float damage)
	{
		life -= damage / 2;
		if (life <= 0)
			_Dead();
		else
		{
			hitted_times++;

			isHurt = true;
			if (hitted_times == 3)
			{
				((AnimatedSprite)GetNode("Orc")).Stop();
				((AnimatedSprite)GetNode("Orc")).Play("Hurt");
				hitted_times = 0;
				life -= damage / 2;
			}
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

	public void _Hit_Throw()
	{
		isHitting = true;
		((AnimatedSprite)GetNode("Orc")).Stop();
		((AnimatedSprite)GetNode("Orc")).Play("Throwing in the air");
		soundAttack.Play();
		Throw();
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		shot_timer.Start();
	}

	private void Throw()
	{
		Rock rock = (Rock)throw_rock.Instance();
		Rock rock2 = (Rock)throw_rock.Instance();
		Rock rock3 = (Rock)throw_rock.Instance();
		GetParent().AddChild(rock);
		GetParent().AddChild(rock2);
		GetParent().AddChild(rock3);
		float xToMove = 60.0f;
		Vector2 rock2_vect;

		if (direction > 0)
		{
			directionVector = Vector2.Right;
			rock2_vect = new Vector2(1, -1);
		}
		else
		{
			directionVector = Vector2.Left;
			rock2_vect = new Vector2(-1, -1);
		}

		if (directionVector == Vector2.Left)
			xToMove *= -1;
		Vector2 where = new Vector2(xToMove, 10.0f);

		rock2.GlobalPosition = GlobalPosition + where;
		rock3.GlobalPosition = GlobalPosition + where;
		rock.GlobalPosition = GlobalPosition + where + new Vector2(0, 75);
		rock3.Scale = 2 * rock3.Scale;
		rock.Scale = 2 * rock.Scale;
		rock2.Scale = 2 * rock2.Scale;

		rock.setup(directionVector);
		rock2.setup(rock2_vect);
		rock3.setup(Vector2.Up);
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

	private void _on_ThrowCollision_body_entered(object body)
	{
		if (body is Movement)
		{
			_Hit_Throw();
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
