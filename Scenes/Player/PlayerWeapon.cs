using Godot;

/// <summary>
/// Laska gracza: obsługa cooldownu, ładowania czaru (zmiana tekstury) i wystrzelenia FireBall.
/// Węzły w scenie gracza: Weapon (Sprite), Timer, MagicTimer, SoundFireball.
/// </summary>
public class PlayerWeapon
{
	private const float ManaCost = 10f;
	private const string StaffPath = "res://Assets/Weapons/Wizard/Staves_1/";

	private readonly Node2D owner;
	private readonly PackedScene fireBallScene;
	private readonly Sprite sprite;
	private readonly Timer cooldown;
	private readonly Timer castTimer;
	private readonly AudioStreamPlayer castSound;
	private readonly Texture idleTexture;
	private readonly Texture chargedTexture;
	private readonly Texture noManaTexture;
	private bool casting = false;

	public PlayerWeapon(Node2D owner, PackedScene fireBallScene, float shotDelay)
	{
		this.owner = owner;
		this.fireBallScene = fireBallScene;

		sprite = owner.GetNode<Sprite>("Weapon");
		castSound = owner.GetNode<AudioStreamPlayer>("SoundFireball");
		cooldown = SetupTimer("Timer", shotDelay);
		castTimer = SetupTimer("MagicTimer", shotDelay * 0.15f);

		idleTexture = (Texture)ResourceLoader.Load(StaffPath + "1hand2.png");
		chargedTexture = (Texture)ResourceLoader.Load(StaffPath + "1hand2_magic.png");
		noManaTexture = (Texture)ResourceLoader.Load(StaffPath + "1hand2_nomana.png");
	}

	public bool Visible
	{
		set { sprite.Visible = value; }
	}

	private Timer SetupTimer(string name, float waitTime)
	{
		Timer t = owner.GetNode<Timer>(name);
		t.OneShot = true;
		t.WaitTime = waitTime;
		t.Autostart = true;
		return t;
	}

	public void Update(bool shotPressed, bool facingRight, PlayerVitals vitals)
	{
		if (shotPressed && cooldown.GetTimeLeft() == 0)
		{
			castTimer.Start();
			cooldown.Start();
			sprite.Texture = vitals.CanSpend(ManaCost) ? chargedTexture : noManaTexture;
			casting = true;
		}
		else if (casting && castTimer.GetTimeLeft() == 0)
		{
			sprite.Texture = idleTexture;
			casting = false;
			Fire(facingRight, vitals);
			castSound.Play();
		}
	}

	private void Fire(bool facingRight, PlayerVitals vitals)
	{
		if (!vitals.CanSpend(ManaCost))
			return;

		Vector2 direction = facingRight ? Vector2.Right : Vector2.Left;
		FireBall fireBall = (FireBall)fireBallScene.Instance();
		fireBall.setDmg(PlayerData.Damage);
		owner.GetParent().AddChild(fireBall);
		fireBall.GlobalPosition = owner.GlobalPosition + new Vector2(facingRight ? 60.0f : -60.0f, 10.0f);
		fireBall.setup(direction);
		vitals.Spend(ManaCost);
	}
}
