using Godot;
using System;

/// <summary>
/// Wspólna logika zwykłych przeciwników: patrol, obracanie się na krawędzi/ścianie,
/// przyjmowanie obrażeń, śmierć z monetą, szarża po trafieniu lub zajściu od tyłu.
///
/// Klasa pochodna podaje tylko to, co się różni: nazwę węzła sprite'a, nazwy animacji
/// i dźwięków oraz sposób ataku (OnHit / OnHitWindow / OnHitAnimationFinished).
///
/// Wymagane węzły sceny: sprite (AnimatedSprite), RayCast2D, CollisionShape2D,
/// PlayerCollisionDetector, BehindCollisionDetector, Timer, Timer2,
/// SoundHurt, SoundAttack oraz dźwięk śmierci. AttackDetector jest opcjonalny.
///
/// Sygnały w scenach łączą się z: _on_animation_finished,
/// _on_PlayerCollisionDetector_body_entered, _on_AttackDetector_body_entered,
/// _on_BehindCollisionDetector_body_entered.
/// </summary>
public abstract class EnemyBase : KinematicBody2D, IEnemy
{
	protected const int GRAVITY = 30;
	protected const int WALK_SPEED = 150;
	protected const int RUN_SPEED = 300;

	// Nazwy pól zostają bez zmian, bo sceny poziomów nadpisują je z edytora (np. life, damage).
	[Export] protected int SPEED = WALK_SPEED;
	[Export] protected float life = 100f;
	[Export] protected float damage = 33f;
	[Export] protected bool canWalk = true;
	[Export] protected bool canRotate = true;

	public Vector2 velocity = new Vector2();

	protected int direction = 1;
	protected bool isDead = false;
	protected bool isHurt = false;
	protected bool isHitting = false;
	protected bool isRunning = false;

	protected AnimatedSprite sprite;
	protected Timer timer;
	protected Timer timer2;
	protected AudioStreamPlayer soundHurt;
	protected AudioStreamPlayer soundAttack;
	protected AudioStreamPlayer soundDeath;

	protected abstract string SpriteName { get; }
	protected virtual string DeathSoundName { get { return "SoundDie"; } }
	protected virtual string WalkAnimation { get { return "Walk"; } }
	protected virtual string RunAnimation { get { return "Running"; } }
	protected virtual string IdleAnimation { get { return "Idle"; } }

	/// <summary>Po ilu sekundach od rozpoczęcia ataku zadawane jest obrażenie / kończy się atak.</summary>
	protected virtual float AttackDelay { get { return 0.15f; } }

	public override void _Ready()
	{
		sprite = GetNode<AnimatedSprite>(SpriteName);
		soundHurt = GetNode<AudioStreamPlayer>("SoundHurt");
		soundAttack = GetNode<AudioStreamPlayer>("SoundAttack");
		soundDeath = GetNode<AudioStreamPlayer>(DeathSoundName);

		timer = SetupTimer("Timer", AttackDelay);
		timer2 = SetupTimer("Timer2", 3f);
	}

	protected Timer SetupTimer(string name, float waitTime)
	{
		Timer t = GetNode<Timer>(name);
		t.OneShot = true;
		t.WaitTime = waitTime;
		t.Autostart = true;
		return t;
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!IsBusy())
		{
			Patrol();
		}
		else
		{
			OnBusyTick();
		}

		if (isRunning && timer2.GetTimeLeft() == 0)
		{
			SPEED = WALK_SPEED;
			isRunning = false;
		}
	}

	/// <summary>Czy przeciwnik robi coś, co blokuje patrol (atak, ból, śmierć...).</summary>
	protected virtual bool IsBusy()
	{
		return isDead || isHurt || isHitting;
	}

	protected virtual void OnBusyTick()
	{
		if (isHitting && timer.GetTimeLeft() == 0)
		{
			OnHitWindow();
		}
	}

	private void Patrol()
	{
		if (canWalk)
		{
			velocity.x = SPEED * direction;
			sprite.Play(isRunning ? RunAnimation : WalkAnimation);
		}
		else
		{
			sprite.Play(IdleAnimation);
		}

		velocity.y += GRAVITY;
		velocity = MoveAndSlide(velocity, Vector2.Up);

		RayCast2D edgeCheck = GetNode<RayCast2D>("RayCast2D");
		if (IsOnWall() || !edgeCheck.IsColliding() && IsOnFloor() && canRotate)
		{
			_Rotate();
		}
	}

	protected void _Rotate()
	{
		direction = direction * -1;
		sprite.FlipH = direction == -1;

		RayCast2D edgeCheck = GetNode<RayCast2D>("RayCast2D");
		Vector2 vector = edgeCheck.Position;
		vector.x = vector.x * -1;
		edgeCheck.Position = vector;

		FlipArea("PlayerCollisionDetector");
		FlipArea("BehindCollisionDetector");
		if (HasNode("AttackDetector"))
		{
			FlipArea("AttackDetector");
		}

		OnRotated();
	}

	private void FlipArea(string name)
	{
		Area2D area = GetNode<Area2D>(name);
		area.Scale = new Vector2(-area.Scale.x, area.Scale.y);
	}

	/// <summary>Dodatkowe odwracanie elementów po obrocie (domyślnie nic).</summary>
	protected virtual void OnRotated() { }

	public void _Hurt(float damage)
	{
		life -= damage;
		if (life <= 0)
		{
			_Dead();
		}
		else
		{
			isHurt = true;
			sprite.Stop();
			sprite.Play("Hurt");
			soundHurt.Play();
		}
	}

	public void _Dead()
	{
		isDead = true;
		DisableShape("CollisionShape2D");
		DisableShape("PlayerCollisionDetector/CollisionShape2D");
		DisableShape("BehindCollisionDetector/CollisionShape2D");
		if (HasNode("AttackDetector"))
		{
			DisableShape("AttackDetector/CollisionShape2D");
		}
		sprite.Play("Dying");
		soundDeath.Play();
		OnDead();
		DropCoin();
	}

	/// <summary>Wywoływane przy śmierci, po animacji i dźwięku, przed upuszczeniem monety.</summary>
	protected virtual void OnDead() { }

	protected void DisableShape(string path)
	{
		GetNode<CollisionShape2D>(path).SetDeferred("disabled", true);
	}

	protected void EnableShape(string path)
	{
		GetNode<CollisionShape2D>(path).SetDeferred("disabled", false);
	}

	protected void DropCoin()
	{
		PackedScene coinScene = (PackedScene)ResourceLoader.Load("res://Scenes/Coins/Coin.tscn");
		Coin coin = (Coin)coinScene.Instance();
		GetParent().AddChild(coin);

		HUD hud = GetTree().CurrentScene.GetNodeOrNull<HUD>("HUD");
		if (hud != null)
		{
			coin.Connect("coin_collected", hud, nameof(hud._on_Coin_coin_collected));
		}
		coin.Position = this.GetPosition();
	}

	public void _Hit()
	{
		DisableShape("PlayerCollisionDetector/CollisionShape2D");
		isHitting = true;
		sprite.Stop();
		sprite.Play("Attack");
		soundAttack.Play();
		OnHit();
		timer.Start();
	}

	/// <summary>Dodatkowa reakcja na rozpoczęcie ataku (np. rzut pociskiem).</summary>
	protected virtual void OnHit() { }

	/// <summary>Moment w ataku, gdy upłynął AttackDelay. Domyślnie (walka wręcz) włącza strefę obrażeń.</summary>
	protected virtual void OnHitWindow()
	{
		GetNode<Area2D>("AttackDetector").Monitoring = true;
	}

	/// <summary>Co zrobić, gdy skończyła się animacja podczas ataku. Domyślnie kończy atak.</summary>
	protected virtual void OnHitAnimationFinished()
	{
		_EndOfHit();
	}

	public void _EndOfHit()
	{
		isHitting = false;
		if (HasNode("AttackDetector"))
		{
			GetNode<Area2D>("AttackDetector").Monitoring = false;
		}
		EnableShape("PlayerCollisionDetector/CollisionShape2D");
	}

	protected void StartRunning()
	{
		timer2.Start();
		SPEED = RUN_SPEED;
		isRunning = true;
	}

	// ---- Sygnały podpinane w scenach ----

	public void _on_animation_finished()
	{
		if (isDead)
			QueueFree();
		if (isHurt)
		{
			isHurt = false;
			StartRunning();
		}
		if (isHitting)
			OnHitAnimationFinished();
		OnOtherAnimationFinished();
	}

	/// <summary>Hak dla stanów dodatkowych w klasach pochodnych (np. leczenie maga).</summary>
	protected virtual void OnOtherAnimationFinished() { }

	public void _on_PlayerCollisionDetector_body_entered(object body)
	{
		if (body is Movement)
		{
			_Hit();
		}
	}

	public void _on_AttackDetector_body_entered(object body)
	{
		if (body is Movement)
		{
			((Movement)body)._Hurt(damage);
		}
	}

	public void _on_BehindCollisionDetector_body_entered(object body)
	{
		OnPlayerBehind();
	}

	/// <summary>Gracz pojawił się za plecami: przeciwnik zawraca i szarżuje.</summary>
	protected virtual void OnPlayerBehind()
	{
		StartRunning();
		_Rotate();
	}
}
