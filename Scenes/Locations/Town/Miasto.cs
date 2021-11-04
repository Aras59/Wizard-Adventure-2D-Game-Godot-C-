using Godot;
using System;

public class Miasto : Node2D
{
	private HpShop hpShop;
	private DmgShop dmgShop;
	private ManaShop manaShop;
	private EndingPanel ending;
	private Label Label2;
	private Area2D HpShopDoors;
	private Area2D ManaShopDoors;
	private Area2D DmgShopDoors;
	private Boolean entered;
	private Movement player;
	private string type = "";
	private static bool portal1 = false;
	private static bool portal2 = false;
	private static bool portal3 = false;
	private static bool stone = true;
	[Signal] public delegate void gethp(float health);
	[Signal] public delegate void getdmg(float dmg);
	[Signal] public delegate void getmana(float mana);
	public override void _Ready()
	{
		if (!portal1)
		{
			offPortal("DesertPortal");
		}
		if (!portal2)
		{
			offPortal("CementaryPortal");
		}
		if (!portal3)
		{
			offPortal("JunglePortal");
		}
		if (!stone)
		{
			offStone();
		}

		entered = false;
		hpShop = GetNode<HpShop>("HpShop");
		dmgShop = GetNode<DmgShop>("DmgShop");
		manaShop = GetNode<ManaShop>("ManaShop");

		Label2 = GetNode<Label>("Label2");
		DmgShopDoors = GetNode<Area2D>("DmgShopDoors");
		ManaShopDoors = GetNode<Area2D>("ManaShopDoors");
		HpShopDoors = GetNode<Area2D>("HpShopDoors");
		ending = GetNode<EndingPanel>("EndingPanel");

		player = GetNode<Movement>("Player");
	}


	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionPressed("buy") && entered == true)
		{
			GetTree().Paused = true;
			if (type.Equals("HP"))
			{
				EmitSignal(nameof(gethp), player.getHp());
				hpShop.changeVisible();
			}
			if (type.Equals("DMG"))
			{
				EmitSignal(nameof(getdmg), player.getDmg());
				dmgShop.changeVisible();
			}
			if (type.Equals("Mana"))
			{
				EmitSignal(nameof(getmana), player.getMana());
				manaShop.changeVisible();
			}
		}
	}

	private void _OffPortals()
	{
		offPortal("DesertPortal");
		offPortal("JunglePortal");
		offPortal("CementaryPortal");
	}

	private void offPortal(string name)
	{
		((AnimatedSprite)GetNode("/root/Miasto/" + name)).Hide();
		((CollisionShape2D)GetNode("/root/Miasto/" + name + "/Area2D/CollisionShape2D")).SetDeferred("disabled", true);
	}

	private void offStone()
	{
		((Sprite)GetNode("/root/Miasto/Rock_03")).Hide();
		((CollisionShape2D)GetNode("/root/Miasto/Rock_03/Area2D/CollisionShape2D")).SetDeferred("disabled", true);
	}

	private void onPortal(string name)
	{
		((AnimatedSprite)GetNode("/root/Miasto/" + name)).Show();
		((CollisionShape2D)GetNode("/root/Miasto/" + name + "/Area2D/CollisionShape2D")).SetDeferred("disabled", false);
		if (name.Equals("DesertPortal"))
			portal1 = true;
		if (name.Equals("CementaryPortal"))
			portal2 = true;
		if (name.Equals("JunglePortal"))
			portal3 = true;
	}

	private void _on_HpShopDoors_body_entered(object body)
	{
		type = "HP";
		Label2.SetPosition(HpShopDoors.GetPosition() + new Vector2(-90, -100));
		Label2.Visible = true;
		entered = true;
		EmitSignal(nameof(gethp), player.getHp());
	}

	private void _on_HpShopDoors_body_exited(object body)
	{
		type = "";
		entered = false;
		Label2.Visible = false;
		GetTree().Paused = false;
	}

	private void _on_ManaShopDoors_body_entered(object body)
	{
		type = "Mana";
		EmitSignal(nameof(getmana), player.getMana());
		Label2.SetPosition(ManaShopDoors.GetPosition() + new Vector2(-90, -100));
		Label2.Visible = true;
		entered = true;
	}

	private void _on_ManaShopDoors_body_exited(object body)
	{
		type = "";
		entered = false;
		Label2.Visible = false;
		GetTree().Paused = false;
	}

	private void _on_DmgShopDoors_body_entered(object body)
	{
		type = "DMG";
		EmitSignal(nameof(getdmg), player.getDmg());
		Label2.SetPosition(DmgShopDoors.GetPosition() + new Vector2(-90, -100));
		Label2.Visible = true;
		entered = true;
	}

	private void _on_DmgShopDoors_body_exited(object body)
	{
		type = "";
		entered = false;
		Label2.Visible = false;
		GetTree().Paused = false;
	}

	private void _on_EndingArea2D_body_entered(object body)
	{
		GetTree().Paused = true;
		ending.changeVisible();
	}
}
