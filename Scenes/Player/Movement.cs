using Godot;
using System;
using System.Diagnostics;

public class Movement : KinematicBody2D
{
	[Export] public int speed = 300;
	[Export] public int gravity = 30;
	[Export] public PackedScene fire_ball;
	[Export] private int jumpforce = -900;
	[Export] private string biom;
	[Export] private string level;
	private static float health = 100f;
	private static float health_max = 100f;
	private static float dmg = 50f;
	private static float health_regeneration = 1f;
	private static float mana;
	private static float mana_max = 60f;
	private static float mana_regeneration = 5f;
	Timer timer;
	Timer magic_timer;
	Timer dark_timer;
	Timer dead_timer;
	[Export] private float shotDelay = 0.5f;
	[Signal] public delegate void hp_changed(float health);
	[Signal] public delegate void mhp_changed(float health);
	[Signal] public delegate void mmana_changed(float mana);
	[Signal] public delegate void mana_changed(float mana);
	public Sprite weapon;
	private Sprite Wizard;
	public Boolean shot_started = false;
	private AnimationPlayer _animationPlayer;
	private AnimatedSprite _animatedSprite;
	private Vector2 direction = Vector2.Right;
	private Node2D pointer;
	private RayCast2D rayCast;
	private String portal = "not";
	private AudioStreamPlayer soundJump;
	private AudioStreamPlayer soundFireball;
	private AudioStreamPlayer soundHealing;
	private AudioStreamPlayer soundDie;
	private AudioStreamPlayer soundHit;
	private static bool desertBossDead = false;
	private static bool jungleBossDead = false;
	private static bool cemetaryBossDead = false;
	private static int fabular = 0;
	public static int numOfDeaths = 0;
	static Stopwatch stopwatch;
	static Stopwatch level_stopwatch;
	public static long sec_left = 0;
	public static int deaths_level = 0;

	Boolean dead_flag = false;

	public Vector2 velocity = new Vector2();

	public static long getTime(){
		stopwatch.Stop();
		return stopwatch.ElapsedMilliseconds;
	}
	
	public static long level_time(){
		level_stopwatch.Stop();
		return level_stopwatch.ElapsedMilliseconds;
	}
	
	public int getLevelDeatchs(){
		return deaths_level;
	}

	public static void startCounting(){
		stopwatch.Start();
	}

	public float getHp()
	{
		return health_max;
	}

	public float getDmg()
	{
		return dmg;
	}

	public float getMana()
	{
		return mana_max;
	}

	public int getFabular()
	{
		return fabular;
	}

	public void setFullHp()
	{
		health = health_max;
	}

	public void updateFabular(int i)
	{
		fabular = i;
	}

	public void killDesertBoss()
	{
		desertBossDead = true;
	}

	public bool isKilledDesertBoss()
	{
		return desertBossDead;
	}


	public void killJungleBoss()
	{
		jungleBossDead = true;
	}

	public bool isKilledJungleBoss()
	{
		return jungleBossDead;
	}


	public void killCemetaryBoss()
	{
		cemetaryBossDead = true;
	}

	public bool isKilledCemetaryBoss()
	{
		return cemetaryBossDead;
	}

	public override void _Ready()
	{
		health = health_max;
		mana = mana_max;

		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");
		Wizard = GetNode<Sprite>("Wizard");
		_animatedSprite.Visible = false;
		weapon = GetNode<Sprite>("Weapon");
		fire_ball = (PackedScene)ResourceLoader.Load("res://Scenes/Attacks/FireBall/FireBall.tscn");
		pointer = GetNode<Node2D>("Pointer");
		rayCast = pointer.GetNode<RayCast2D>("RayCast2D");
		soundJump = GetNode<AudioStreamPlayer>("SoundJump");
		soundFireball = GetNode<AudioStreamPlayer>("SoundFireball");
		soundHealing = GetNode<AudioStreamPlayer>("SoundHealing");
		soundDie = GetNode<AudioStreamPlayer>("SoundDie");
		soundHit = GetNode<AudioStreamPlayer>("SoundHit");

		timer = GetNode<Timer>("Timer");
		timer.OneShot = true;
		timer.WaitTime = shotDelay;
		timer.Autostart = true;
		magic_timer = GetNode<Timer>("MagicTimer");
		magic_timer.OneShot = true;
		magic_timer.WaitTime = shotDelay * 0.15f;
		magic_timer.Autostart = true;
		dark_timer = GetNode<Timer>("Dark");
		dark_timer.OneShot = true;
		dark_timer.WaitTime = 3f;
		dark_timer.Autostart = true;
		dead_timer = GetNode<Timer>("DeadTimer");
		dead_timer.OneShot = true;
		dead_timer.WaitTime = 0.7f;
		EmitSignal(nameof(mhp_changed), health_max);
		EmitSignal(nameof(mmana_changed), mana_max);
		stopwatch = new Stopwatch();
		level_stopwatch= new Stopwatch();
		stopwatch.Start();
		level_stopwatch.Start();

	}

	public void GetInput()
	{
		if (!portal.Equals("not") && Input.IsActionPressed("interaction"))
		{
			if (portal.Equals("Desertlvl0"))
			{
				changeScene("res://Scenes/Locations/Desert/" + portal + "/" + portal + ".tscn");
			}
			if (portal.Equals("Cemeterylvl1"))
			{
				changeScene("res://Scenes/Locations/Cemetery/" + portal + "/" + portal + ".tscn");
			}
			if (portal.Equals("Junglelvl1"))
			{
				changeScene("res://Scenes/Locations/Jungle/" + portal + "/" + portal + ".tscn");
			}
		}

		if (Input.IsActionPressed("right"))
		{
			velocity.x = speed;
			_animationPlayer.Play("Walk_Right");
			direction = Vector2.Right;
		}
		else if (Input.IsActionPressed("left"))
		{
			velocity.x = -speed;
			_animationPlayer.Play("Walk_Left");
			direction = Vector2.Left;
		}
		else
		{
			velocity.x = 0;
			if (direction == Vector2.Right)
			{
				_animationPlayer.Play("Idle_Right");
			}
			else
			{
				_animationPlayer.Play("Idle_Left");
			}
		}

		rotatePointer();

		if (!IsOnFloor())
		{
			if (velocity.y < 0)
			{
				if (direction == Vector2.Right)
				{
					_animationPlayer.Play("Jump_Right");
				}
				else
				{
					_animationPlayer.Play("Jump_Left");
				}
			}
			else
			{
				if (direction == Vector2.Right)
				{
					_animationPlayer.Play("Falling_Right");
				}
				else
				{
					_animationPlayer.Play("Falling_Left");
				}
			}
		}

		velocity.y += gravity;

		if (Input.IsActionPressed("up") && IsOnFloor())
		{
			if (GetFloorVelocity().y < 0)
			{
				Vector2 pos = new Vector2();
				pos.x = Position.x;
				pos.y = Position.y + GetFloorVelocity().y * GetPhysicsProcessDeltaTime() - gravity * GetPhysicsProcessDeltaTime() - 1;
				Position = pos;
			}
			velocity.y = jumpforce;
			if (direction == Vector2.Right)
			{
				_animationPlayer.Play("Jump_Start_Right");
			}
			else
			{
				_animationPlayer.Play("Jump_Start_Left");
			}
			soundJump.Play();
		}

		if (Input.IsActionPressed("shot") && timer.GetTimeLeft() == 0)
		{
			magic_timer.Start();
			timer.Start();
			if (mana >= 10)
			{
				weapon.Texture = ResourceLoader.Load("res://Assets/Weapons/Wizard/Staves_1/1hand2_magic.png") as Texture;
			}
			else
			{
				weapon.Texture = ResourceLoader.Load("res://Assets/Weapons/Wizard/Staves_1/1hand2_nomana.png") as Texture;
			}

			shot_started = true;
		}
		else if (magic_timer.GetTimeLeft() == 0 && shot_started)
		{
			weapon.Texture = ResourceLoader.Load("res://Assets/Weapons/Wizard/Staves_1/1hand2.png") as Texture;
			shot_started = false;
			shoot();
			soundFireball.Play();
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (dead_flag == false)
		{
			GetInput();
		}
		velocity = MoveAndSlide(velocity, Vector2.Up);
		float new_health = Math.Min(health + health_regeneration, health_max);
		float new_mana = Math.Min(mana + mana_regeneration * delta, mana_max);


		for (int i = 0; i < GetSlideCount(); i++)
		{
			var collision = GetSlideCollision(i);
			if (collision.GetCollider().HasMethod("collide_with"))
			{
				FallingPlatform falling = (FallingPlatform)collision.GetCollider();
				falling.collide_with(collision, this);
			}
		}

		if (new_mana != mana && new_mana > -1)
		{
			mana = new_mana;
			EmitSignal(nameof(mana_changed), mana);
		}

		if (health <= 0)
		{
			_Dead();
		}

	}

	private void changeScene(String path){
		Console.WriteLine(sec_left/1000);
		Console.WriteLine(getTime()/1000);
		sec_left += getTime();
		GetTree().ChangeScene(path);
	}

	private void _Dead()
	{
		if (dead_flag == false)
		{
			numOfDeaths++;
			deaths_level++;
			velocity.x = 0;
			velocity.y = 0;
			_animatedSprite.Visible = true;
			Wizard.Visible = false;
			weapon.Visible = false;
			_animatedSprite.FlipH = !(direction == Vector2.Right);
			_animatedSprite.Play("dying");
			soundDie.Play();
			dead_flag = true;
			
			dead_timer.Start();
		}
		else if (dead_flag && dead_timer.GetTimeLeft() == 0)
		{
			dead_flag = false;
			_animatedSprite.Visible = false;
			Wizard.Visible = true;
			weapon.Visible = true;
			health = health_max;
			mana = mana_max;
			changeScene("res://Scenes/Locations/" + biom + "/" + level + "/" + level + ".tscn");
			
		}
	}

	public void _Heal(float val)
	{
		health = Math.Min(health + val, health_max);
		soundHealing.Play();
		EmitSignal(nameof(hp_changed), health);
	}

	private void shoot()
	{
		if (mana >= 10)
		{
			weapon.Texture = ResourceLoader.Load("res://Assets/Weapons/Wizard/Staves_1/1hand2.png") as Texture;
			FireBall fireBall = (FireBall)fire_ball.Instance();
			fireBall.setDmg(dmg);
			GetParent().AddChild(fireBall);
			float xToMove = 60.0f;
			if (direction == Vector2.Left)
				xToMove *= -1;
			Vector2 where = new Vector2(xToMove, 10.0f);

			fireBall.GlobalPosition = GlobalPosition + where;
			fireBall.setup(direction);
			mana -= 10;
		}
	}

	private void rotatePointer()
	{
		if (direction == Vector2.Right)
		{
			pointer.Rotation = 0;
		}
		else if (direction == Vector2.Left)
		{
			pointer.Rotation = (float)Math.PI;
		}
	}

	private float Lerp(float firstFloat, float secondFloat, float by)
	{
		return firstFloat * (1 - by) + secondFloat * by;
	}

	public void _Hurt(float damage)
	{
		health -= damage;
		if (dead_flag == false && health > 0)
		{
			soundHit.Play();
		}
		EmitSignal(nameof(hp_changed), health);
	}

	public void _Dark()
	{
		if (GetNode<Light2D>("Light2D") != null)
		{
			dark_timer.Start();
			GetNode<Light2D>("Light2D").TextureScale = 2;
		}
	}

	private void _on_FallZone_body_entered(object body)
	{
		dead_flag = true;
		_Dead();
	}

	private void _on_TeleportToDesert_body_entered(object body)
	{
		changeScene("res://Scenes/Locations/Desert/Desertlvl1/Desertlvl1.tscn");
	}

	private void SetLabelForPortal(string NameOfPortal, string text = "")
	{
		var label = (Label)GetNode("/root/Miasto/Label");
		var portal_pos = (Node2D)GetNode("/root/Miasto/" + NameOfPortal);

		label.SetPosition(portal_pos.GetPosition() + new Vector2(-90, -136));

		label.Show();

		if (text.Equals(""))
			label.SetText("Press \"F\"\n to enter");
		else
			label.SetText(text);
	}

	private void _on_Desert_Portal_body_entered(object body)
	{
		portal = "Desertlvl0";
		SetLabelForPortal("DesertPortal");
	}

	private void _on_Desert_Portal_body_exited(object body)
	{
		portal = "not";
		var label = (Label)GetNode("/root/Miasto/Label");
		label.Hide();
	}

	private void _on_Jungle_Portal_body_entered(object body)
	{
		portal = "Junglelvl1";
		SetLabelForPortal("JunglePortal");
	}

	private void _on_Jungle_Portal_body_exited(object body)
	{
		portal = "not";
		var label = (Label)GetNode("/root/Miasto/Label");
		label.Hide();
	}

	private void _on_Cementary_Portal_body_entered(object body)
	{
		portal = "Cemeterylvl1";
		SetLabelForPortal("CementaryPortal");
	}

	private void _on_Cementary_Portal_body_exited(object body)
	{
		portal = "not";
		var label = (Label)GetNode("/root/Miasto/Label");
		label.Hide();
	}

	private void _on_TeleportLVL1_body_entered(object body)
	{
		changeScene("res://Scenes/Locations/Desert/Desertlvl1/Desertlvl1.tscn");
	}

	private void _on_FallZone_bossroom_body_entered(object body)
	{
		changeScene("res://Scenes/Locations/Desert/BossRoom/BossRoom.tscn");
	}

	private void _on_Cemetary1FallZone_body_entered(object body)
	{
		changeScene("res://Scenes/Locations/Cemetery/Cemeterylvl1/Cemeterylvl1.tscn");
	}

	private void _on_Cemetary2FallZone_body_entered(object body)
	{
		changeScene("res://Scenes/Locations/Cemetery/Cemeterylvl2/Cemeterylvl2.tscn");
	}

	private void _on_Dark_timeout()
	{
		if (GetNode<Light2D>("Light2D") != null)
		{
			GetNode<Light2D>("Light2D").TextureScale = 10;
		}
	}

	private void _on_Spike_body_entered(object body)
	{
		if (body == this)
		{
			health = 0;
			EmitSignal(nameof(hp_changed), health);
			_Dead();
		}

	}

	private void _on_HpShop_sethp(float health1)
	{
		health_max = health1;
		health = health1;
		EmitSignal(nameof(mhp_changed), health1);
	}

	private void _on_ManaShop_setmana(float mana1)
	{
		mana_max = mana1;
		mana = mana1;
		EmitSignal(nameof(mmana_changed), mana1);
	}

	private void _on_DmgShop_setdmg(float dmg1)
	{
		dmg = dmg1;
	}

	public void set_active(bool active)
	{
		SetPhysicsProcess(active);
		SetProcess(active);
		SetProcessInput(active);
	}


	private void _on_NPC_body_entered(object body)
	{
		SetLabelForPortal("King", "Press \"F\"\n to talk");
	}

	private void _on_Smith_body_entered(object body)
	{
		SetLabelForPortal("Smith", "Press \"F\"\n to talk");
	}

	private void _on_NPC_body_exited(object body)
	{
		var label = (Label)GetNode("/root/Miasto/Label");
		label.Hide();
	}

}
