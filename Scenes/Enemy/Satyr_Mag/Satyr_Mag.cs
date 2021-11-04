using Godot;
using System;

public class Satyr_Mag : KinematicBody2D, IEnemy
{
	const int GRAVITY = 30;
	int SPEED = 150;
	public Vector2 velocity = new Vector2();
	private int direction = 1;
	bool isDead = false;
	bool isHurt = false;
	bool isHitting = false;
	bool isRunning = false;
	bool isHealing = false;
	float life = 250f;
	float healthReg = 350f;
	float damage = 33f;
	Timer timer;
	Timer timer2;
	private AudioStreamPlayer soundHurt;
	private AudioStreamPlayer soundAttack;
	private AudioStreamPlayer soundDie;
	private AudioStreamPlayer soundHeal;
	[Signal] public delegate void i_am_dead();

	float delay = 0.15f;

	public override void _Ready()
	{
		soundHurt = GetNode<AudioStreamPlayer>("SoundHurt");
		soundAttack = GetNode<AudioStreamPlayer>("SoundAttack");
		soundDie = GetNode<AudioStreamPlayer>("SoundDie");
		soundHeal = GetNode<AudioStreamPlayer>("SoundHeal");

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
		if (!isDead && !isHurt && !isHitting && !isHealing)
		{
			if (direction == -1)
			{
				((AnimatedSprite)GetNode("Satyr_Mag")).FlipH = true;
			}
			else
			{
				((AnimatedSprite)GetNode("Satyr_Mag")).FlipH = false;
			}

			velocity.x = SPEED * direction;
			if (!isRunning)
				((AnimatedSprite)GetNode("Satyr_Mag")).Play("Walk");
			else
				((AnimatedSprite)GetNode("Satyr_Mag")).Play("Walk");
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
		else if (isHealing && timer.GetTimeLeft() == 0)
		{
			((Area2D)GetNode("DetectWarrior")).Monitoring = true;
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
		((AnimatedSprite)GetNode("Satyr_Mag")).Play("Dying");
		soundDie.Play();
		EmitSignal(nameof(i_am_dead));

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
			((AnimatedSprite)GetNode("Satyr_Mag")).Stop();
			((AnimatedSprite)GetNode("Satyr_Mag")).Play("Hurt");
			soundHurt.Play();
		}
	}

	public void _Hit()
	{
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", true);
		isHitting = true;
		((AnimatedSprite)GetNode("Satyr_Mag")).Stop();
		((AnimatedSprite)GetNode("Satyr_Mag")).Play("Attack");
		soundAttack.Play();
		timer.Start();
	}

	public void _Heal()
	{
		((CollisionShape2D)GetNode("DetectWarrior/CollisionShape2D")).SetDeferred("disabled", true);
		isHealing = true;
		((AnimatedSprite)GetNode("Satyr_Mag")).Stop();
		((AnimatedSprite)GetNode("Satyr_Mag")).Play("Taunt");
		soundHeal.Play();
		timer.Start();
	}

	public void _EndOfHit()
	{
		isHitting = false;
		((Area2D)GetNode("AttackDetector")).Monitoring = false;
		((CollisionShape2D)GetNode("PlayerCollisionDetector/CollisionShape2D")).SetDeferred("disabled", false);
	}

	public void _EndOfHeal()
	{
		isHealing = false;
		((Area2D)GetNode("DetectWarrior")).Monitoring = true;
		((CollisionShape2D)GetNode("DetectWarrior/CollisionShape2D")).SetDeferred("disabled", false);
	}

	private void _on_Satyr_Mag_animation_finished()
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
		if (isHealing)
			_EndOfHeal();
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
		timer2.Start();
		SPEED = 300;
		isRunning = true;
		_Rotate();
	}

	private void _on_DetectWarrior_body_entered(object body)
	{
		if (body is Satyr_Warrior)
		{
			if ((body as Satyr_Warrior)._Heal(healthReg))
			{
				_Heal();
			}
		}
		else if (body is Satyr_Boss)
		{
			if ((body as Satyr_Boss)._Heal(healthReg))
			{
				_Heal();
			}
		}
	}

}
