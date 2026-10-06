using Godot;
using System;

/// <summary>
/// Postać gracza: ruch, skok, śmierć i odrodzenie. Reszta jest wydzielona:
/// PlayerVitals (HP/mana), PlayerAnimator (animacje), PlayerWeapon (czary),
/// PlayerData (stan między scenami), SceneManager (zmiana scen).
/// </summary>
public class Movement : KinematicBody2D
{
	[Export] public int speed = 300;
	[Export] public int gravity = 30;
	[Export] public PackedScene fire_ball;
	[Export] private int jumpforce = -900;
	[Export] private string biom;
	[Export] private string level;
	[Export] private float shotDelay = 0.5f;
	[Signal] public delegate void hp_changed(float health);
	[Signal] public delegate void mhp_changed(float health);
	[Signal] public delegate void mmana_changed(float mana);
	[Signal] public delegate void mana_changed(float mana);

	public Vector2 velocity = new Vector2();

	private PlayerVitals vitals = new PlayerVitals();
	private PlayerAnimator animator;
	private PlayerWeapon weapon;
	private Sprite Wizard;
	private AnimatedSprite _animatedSprite;
	private Node2D pointer;
	private Timer dark_timer;
	private Timer dead_timer;
	private AudioStreamPlayer soundJump;
	private AudioStreamPlayer soundHealing;
	private AudioStreamPlayer soundDie;
	private AudioStreamPlayer soundHit;
	private bool facingRight = true;
	private bool dead_flag = false;

	public override void _Ready()
	{
		vitals.Refill();

		animator = new PlayerAnimator(GetNode<AnimationPlayer>("AnimationPlayer"));
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_animatedSprite.Visible = false;
		Wizard = GetNode<Sprite>("Wizard");
		pointer = GetNode<Node2D>("Pointer");
		soundJump = GetNode<AudioStreamPlayer>("SoundJump");
		soundHealing = GetNode<AudioStreamPlayer>("SoundHealing");
		soundDie = GetNode<AudioStreamPlayer>("SoundDie");
		soundHit = GetNode<AudioStreamPlayer>("SoundHit");

		fire_ball = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/FireBall/FireBall.tscn");
		weapon = new PlayerWeapon(this, fire_ball, shotDelay);

		dark_timer = GetNode<Timer>("Dark");
		dark_timer.OneShot = true;
		dark_timer.WaitTime = 3f;
		dark_timer.Autostart = true;
		dead_timer = GetNode<Timer>("DeadTimer");
		dead_timer.OneShot = true;
		dead_timer.WaitTime = 0.7f;

		EmitSignal(nameof(mhp_changed), PlayerData.MaxHealth);
		EmitSignal(nameof(mmana_changed), PlayerData.MaxMana);
		PlayerData.StartSceneTimers();
	}

	// ---- Dostęp do stanu (używany przez UI, króla i sceny lokacji) ----

	public float getHp() { return PlayerData.MaxHealth; }
	public float getDmg() { return PlayerData.Damage; }
	public float getMana() { return PlayerData.MaxMana; }
	public int getFabular() { return PlayerData.StoryStage; }
	public void updateFabular(int i) { PlayerData.StoryStage = i; }
	public void setFullHp() { vitals.Health = PlayerData.MaxHealth; }

	public void killDesertBoss() { PlayerData.DesertBossDead = true; }
	public bool isKilledDesertBoss() { return PlayerData.DesertBossDead; }
	public void killJungleBoss() { PlayerData.JungleBossDead = true; }
	public bool isKilledJungleBoss() { return PlayerData.JungleBossDead; }
	public void killCemetaryBoss() { PlayerData.CemeteryBossDead = true; }
	public bool isKilledCemetaryBoss() { return PlayerData.CemeteryBossDead; }

	/// <summary>Zakup w sklepie: nowe maksimum HP, od razu pełne.</summary>
	public void SetMaxHealth(float value)
	{
		PlayerData.MaxHealth = value;
		vitals.Health = value;
		EmitSignal(nameof(mhp_changed), value);
	}

	/// <summary>Zakup w sklepie: nowe maksimum many, od razu pełne.</summary>
	public void SetMaxMana(float value)
	{
		PlayerData.MaxMana = value;
		vitals.Mana = value;
		EmitSignal(nameof(mmana_changed), value);
	}

	public void SetDamage(float value)
	{
		PlayerData.Damage = value;
	}

	/// <summary>Używane przez DialoguePlayer.gd do zamrożenia gracza na czas rozmowy.</summary>
	public void set_active(bool active)
	{
		SetPhysicsProcess(active);
		SetProcess(active);
		SetProcessInput(active);
	}

	// ---- Sterowanie ----

	public override void _PhysicsProcess(float delta)
	{
		if (dead_flag == false)
		{
			GetInput();
		}
		velocity = MoveAndSlide(velocity, Vector2.Up);

		for (int i = 0; i < GetSlideCount(); i++)
		{
			var collision = GetSlideCollision(i);
			if (collision.GetCollider().HasMethod("collide_with"))
			{
				FallingPlatform falling = (FallingPlatform)collision.GetCollider();
				falling.collide_with(collision, this);
			}
		}

		if (vitals.RegenerateMana(delta))
		{
			EmitSignal(nameof(mana_changed), vitals.Mana);
		}

		if (vitals.IsDead)
		{
			_Dead();
		}
	}

	private void GetInput()
	{
		HandleHorizontalMovement();
		pointer.Rotation = facingRight ? 0 : (float)Math.PI;

		if (!IsOnFloor())
		{
			animator.Airborne(facingRight, velocity.y < 0);
		}

		velocity.y += gravity;

		if (Input.IsActionPressed("up") && IsOnFloor())
		{
			Jump();
		}

		weapon.Update(Input.IsActionPressed("shot"), facingRight, vitals);
	}

	private void HandleHorizontalMovement()
	{
		if (Input.IsActionPressed("right"))
		{
			velocity.x = speed;
			facingRight = true;
			animator.Walk(facingRight);
		}
		else if (Input.IsActionPressed("left"))
		{
			velocity.x = -speed;
			facingRight = false;
			animator.Walk(facingRight);
		}
		else
		{
			velocity.x = 0;
			animator.Idle(facingRight);
		}
	}

	private void Jump()
	{
		// Stoimy na platformie jadącej w górę: wyprzedzamy ją, żeby skok nie "zgubił" podłoża.
		if (GetFloorVelocity().y < 0)
		{
			float dt = GetPhysicsProcessDeltaTime();
			Position = new Vector2(Position.x, Position.y + GetFloorVelocity().y * dt - gravity * dt - 1);
		}
		velocity.y = jumpforce;
		animator.JumpStart(facingRight);
		soundJump.Play();
	}

	// ---- Zdrowie, śmierć, odrodzenie ----

	public void _Hurt(float damage)
	{
		vitals.TakeDamage(damage);
		if (dead_flag == false && vitals.Health > 0)
		{
			soundHit.Play();
		}
		EmitSignal(nameof(hp_changed), vitals.Health);
	}

	public void _Heal(float val)
	{
		vitals.Heal(val);
		soundHealing.Play();
		EmitSignal(nameof(hp_changed), vitals.Health);
	}

	/// <summary>Wołane co klatkę, dopóki HP &lt;= 0: najpierw animacja śmierci, po DeadTimer odrodzenie.</summary>
	private void _Dead()
	{
		if (dead_flag == false)
		{
			PlayerData.TotalDeaths++;
			PlayerData.LevelDeaths++;
			velocity = Vector2.Zero;
			_animatedSprite.Visible = true;
			Wizard.Visible = false;
			weapon.Visible = false;
			_animatedSprite.FlipH = !facingRight;
			_animatedSprite.Play("dying");
			soundDie.Play();
			dead_flag = true;

			dead_timer.Start();
		}
		else if (dead_timer.GetTimeLeft() == 0)
		{
			dead_flag = false;
			_animatedSprite.Visible = false;
			Wizard.Visible = true;
			weapon.Visible = true;
			vitals.Refill();
			SceneManager.GoAndRecordTime(GetTree(), SceneManager.LocationScene(biom, level));
		}
	}

	// ---- Efekty ----

	public void _Dark()
	{
		Light2D light = GetNodeOrNull<Light2D>("Light2D");
		if (light != null)
		{
			dark_timer.Start();
			light.TextureScale = 2;
		}
	}

	private void _on_Dark_timeout()
	{
		Light2D light = GetNodeOrNull<Light2D>("Light2D");
		if (light != null)
		{
			light.TextureScale = 10;
		}
	}

	// ---- Strefy w lokacjach (sygnały podpięte w scenach poziomów) ----

	// Upadek w przepaść: odrodzenie bez animacji śmierci i bez doliczania śmierci
	// (dead_flag ustawione z góry omija pierwszą gałąź _Dead; zachowanie z oryginału).
	private void _on_FallZone_body_entered(object body)
	{
		dead_flag = true;
		_Dead();
	}

	private void _on_Spike_body_entered(object body)
	{
		if (body == this)
		{
			vitals.Health = 0;
			EmitSignal(nameof(hp_changed), vitals.Health);
			_Dead();
		}
	}

	private void _on_FallZone_bossroom_body_entered(object body)
	{
		SceneManager.GoAndRecordTime(GetTree(), "res://Scenes/Locations/Desert/BossRoom/BossRoom.tscn");
	}

	private void _on_TeleportLVL1_body_entered(object body)
	{
		SceneManager.GoAndRecordTime(GetTree(), SceneManager.LocationScene("Desert", "Desertlvl1"));
	}
}
