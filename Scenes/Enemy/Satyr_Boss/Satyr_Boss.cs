using Godot;
using System;

public class Satyr_Boss : KinematicBody2D, IEnemy
{
	const int GRAVITY = 30;
	int SPEED = 150;
	public Vector2 velocity = new Vector2();
	private int direction = 1;
	bool isDead = false;
	bool isHurt = false;
	bool isHitting = false;
	bool isThrowing = false;
	AnimatedSprite sprite;
	bool isRunning = false;
	float life = 450f;
	float maxLife = 450f;
	float damage = 50f;
	float prevHealth;
	Timer timer;
	Timer timer2;
	Timer timer3;
	Timer respawn_timer;
	Timer throw_timer;
	private Vector2 directionVector = Vector2.Right;
	private AudioStreamPlayer soundHurt;
	private AudioStreamPlayer soundAttack;
	private AudioStreamPlayer soundDie;
	[Export] public PackedScene throw_stone;
	[Export] public PackedScene healer;
	[Signal] public delegate void dead(Vector2 position);
	Boolean isStart = true;

	float delay = 0.15f;

	public override void _Ready()
	{
		prevHealth = maxLife;
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

		timer3 = GetNode<Timer>("Timer3");
		timer3.OneShot = true;
		timer3.WaitTime = 1f;
		timer3.Autostart = true;

		respawn_timer = GetNode<Timer>("RespawnTimer");
		respawn_timer.OneShot = true;
		respawn_timer.WaitTime = 8f;
		respawn_timer.Autostart = false;

		throw_timer = GetNode<Timer>("ThrowTimer");
		throw_timer.OneShot = true;
		throw_timer.WaitTime = 4f;
		throw_timer.Autostart = true;

		sprite = GetNode<AnimatedSprite>("Satyr_Boss");
		throw_stone = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/RollingStone/RollingStone.tscn");
		healer = (PackedScene)ResourceLoader.Load("res://Scenes/Enemy/Satyr_Mag/Satyr_Mag.tscn");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (life < maxLife)
		{
			this.CollisionLayer = 144;
		}
		else this.CollisionLayer = 16;

		if (!isDead && !isHurt && !isHitting)
		{
			if (direction == -1)
			{
				((AnimatedSprite)GetNode("Satyr_Boss")).FlipH = true;
			}
			else
			{
				((AnimatedSprite)GetNode("Satyr_Boss")).FlipH = false;
			}

			velocity.x = SPEED * direction;
			if (!isRunning)
				((AnimatedSprite)GetNode("Satyr_Boss")).Play("Walk");
			else
				((AnimatedSprite)GetNode("Satyr_Boss")).Play("Walk");
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


		if (throw_timer.GetTimeLeft() == 0)
		{
			((Area2D)GetNode("ThrowCollision")).Monitoring = true;
		}
		if (isRunning && timer2.GetTimeLeft() == 0)
		{
			SPEED = 150;
			isRunning = false;
		}

		if (respawn_timer.GetTimeLeft() == 0 && !isStart)
		{
			((AnimatedSprite)GetNode("Satyr_Boss")).Stop();
			((AnimatedSprite)GetNode("Satyr_Boss")).Play("Taunt");
			CreateSatyrMag();
			isStart = true;
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
		((Movement)GetNode("/root/Root/Player/Player")).killJungleBoss();
		((CollisionShape2D)GetNode("CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("AttackDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((CollisionShape2D)GetNode("BehindCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((AnimatedSprite)GetNode("Satyr_Boss")).Play("Dying");
		soundDie.Play();
		EmitSignal(nameof(dead), GlobalPosition + new Vector2(0, 80));
	}

	public void _Hurt(float damage)
	{
		life -= damage;
		if (life <= 0) _Dead();
		else if (life > 0.8f * maxLife)
		{
			isHurt = false;
		}
		else
		{
			isHurt = true;
			((AnimatedSprite)GetNode("Satyr_Boss")).Stop();
			((AnimatedSprite)GetNode("Satyr_Boss")).Play("Hurt");
			soundHurt.Play();
		}
	}

	public bool _Heal(float health)
	{
		if (life < maxLife)
		{
			timer3.Start();
			life = Math.Min(life + health, prevHealth);
			(sprite.Material as ShaderMaterial).SetShaderParam("isOn", true);
			this.CollisionLayer = 16;
			return true;
		}
		return false;
	}

	public void _Hit()
	{
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		isHitting = true;
		((AnimatedSprite)GetNode("Satyr_Boss")).Stop();
		((AnimatedSprite)GetNode("Satyr_Boss")).Play("Attack");
		soundAttack.Play();
		timer.Start();
	}

	public void _EndOfHit()
	{
		isHitting = false;
		((Area2D)GetNode("AttackDetector")).Monitoring = false;
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", false);
	}

	public void _EndOfThrow()
	{
		isThrowing = false;
		((Area2D)GetNode("ThrowCollision")).Monitoring = false;
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", false);
	}

	public void _Hit_Throw()
	{
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		((AnimatedSprite)GetNode("Satyr_Boss")).Stop();
		((AnimatedSprite)GetNode("Satyr_Boss")).Play("Taunt");
		Throw();
		soundAttack.Play();
		isThrowing = true;
		throw_timer.Start();
	}

	private void Throw()
	{
		RollingStone stone = (RollingStone)throw_stone.Instance();
		GetParent().AddChild(stone);
		float xToMove = 60.0f;

		if (direction > 0)
		{
			directionVector = Vector2.Right;
			stone.rotating = 0.1f;
		}
		else
		{
			directionVector = Vector2.Left;
			stone.rotating = -0.1f;
		}

		if (directionVector == Vector2.Left)
			xToMove *= -1;
		Vector2 where = new Vector2(xToMove, 50.0f);

		stone.GlobalPosition = GlobalPosition + where;

		stone.setup(directionVector);
	}

	private void CreateSatyrMag()
	{
		Satyr_Mag mag = (Satyr_Mag)healer.Instance();
		mag.Connect("i_am_dead", this, nameof(_on_EnemySatyr_Mag3_i_am_dead));
		GetParent().AddChild(mag);
		mag.GlobalPosition = new Vector2(3779, -89);
		prevHealth = life;
	}

	private void _on_Satyr_Boss_animation_finished()
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
		{
			_EndOfHit();
		}
		if (isThrowing)
		{
			_EndOfThrow();
		}

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

	private void _on_BehindCollisionDetector_body_entered(object body)
	{
		SPEED = 300;
		isRunning = true;
		_Rotate();
	}

	private void _on_Timer3_timeout()
	{
		(sprite.Material as ShaderMaterial).SetShaderParam("isOn", false);
	}

	private void _on_ThrowCollision_body_entered(object body)
	{
		if (body is Movement)
		{
			_Hit_Throw();
		}
	}

	public void _on_EnemySatyr_Mag3_i_am_dead()
	{
		if (respawn_timer.GetTimeLeft() == 0)
		{
			respawn_timer.Start();
			isStart = false;
		}
	}
}
