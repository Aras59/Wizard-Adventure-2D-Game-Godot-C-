using Godot;
using System;

public class Miasto : Node2D
{
	private enum ShopType { None, Hp, Dmg, Mana }

	private ShopPanel hpShop;
	private ShopPanel dmgShop;
	private ShopPanel manaShop;
	private EndingPanel ending;
	private Label Label2;
	private Area2D HpShopDoors;
	private Area2D ManaShopDoors;
	private Area2D DmgShopDoors;
	private ShopType currentShop = ShopType.None;
	private Movement player;
	private Label interactionLabel;
	private string currentPortalScene = null;
	[Signal] public delegate void gethp(float health);
	[Signal] public delegate void getdmg(float dmg);
	[Signal] public delegate void getmana(float mana);

	public override void _Ready()
	{
		if (!PlayerData.DesertPortalUnlocked)
		{
			offPortal("DesertPortal");
		}
		if (!PlayerData.CemeteryPortalUnlocked)
		{
			offPortal("CementaryPortal");
		}
		if (!PlayerData.JunglePortalUnlocked)
		{
			offPortal("JunglePortal");
		}
		if (!PlayerData.TownStoneBlocking)
		{
			offStone();
		}

		hpShop = GetNode<ShopPanel>("HpShop");
		dmgShop = GetNode<ShopPanel>("DmgShop");
		manaShop = GetNode<ShopPanel>("ManaShop");

		Label2 = GetNode<Label>("Label2");
		DmgShopDoors = GetNode<Area2D>("DmgShopDoors");
		ManaShopDoors = GetNode<Area2D>("ManaShopDoors");
		HpShopDoors = GetNode<Area2D>("HpShopDoors");
		ending = GetNode<EndingPanel>("EndingPanel");

		player = GetNode<Movement>("Player");
		interactionLabel = GetNode<Label>("Label");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (currentPortalScene != null && Input.IsActionPressed("interaction"))
		{
			SceneManager.GoAndRecordTime(GetTree(), currentPortalScene);
		}

		if (Input.IsActionPressed("buy") && currentShop != ShopType.None)
		{
			GetTree().Paused = true;
			switch (currentShop)
			{
				case ShopType.Hp:
					EmitSignal(nameof(gethp), player.getHp());
					hpShop.changeVisible();
					break;
				case ShopType.Dmg:
					EmitSignal(nameof(getdmg), player.getDmg());
					dmgShop.changeVisible();
					break;
				case ShopType.Mana:
					EmitSignal(nameof(getmana), player.getMana());
					manaShop.changeVisible();
					break;
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
		GetNode<AnimatedSprite>(name).Hide();
		GetNode<CollisionShape2D>(name + "/Area2D/CollisionShape2D").SetDeferred("disabled", true);
	}

	private void offStone()
	{
		GetNode<Sprite>("Rock_03").Hide();
		GetNode<CollisionShape2D>("Rock_03/Area2D/CollisionShape2D").SetDeferred("disabled", true);
	}

	/// <summary>Wywoływane z King.gd po rozmowie z królem.</summary>
	private void onPortal(string name)
	{
		GetNode<AnimatedSprite>(name).Show();
		GetNode<CollisionShape2D>(name + "/Area2D/CollisionShape2D").SetDeferred("disabled", false);
		if (name.Equals("DesertPortal"))
			PlayerData.DesertPortalUnlocked = true;
		if (name.Equals("CementaryPortal"))
			PlayerData.CemeteryPortalUnlocked = true;
		if (name.Equals("JunglePortal"))
			PlayerData.JunglePortalUnlocked = true;
	}

	private void EnterShop(ShopType shop, Area2D doors)
	{
		currentShop = shop;
		Label2.SetPosition(doors.GetPosition() + new Vector2(-90, -100));
		Label2.Visible = true;
		switch (shop)
		{
			case ShopType.Hp:
				EmitSignal(nameof(gethp), player.getHp());
				break;
			case ShopType.Dmg:
				EmitSignal(nameof(getdmg), player.getDmg());
				break;
			case ShopType.Mana:
				EmitSignal(nameof(getmana), player.getMana());
				break;
		}
	}

	private void LeaveShop()
	{
		currentShop = ShopType.None;
		Label2.Visible = false;
		GetTree().Paused = false;
	}

	private void _on_HpShopDoors_body_entered(object body)
	{
		EnterShop(ShopType.Hp, HpShopDoors);
	}

	private void _on_HpShopDoors_body_exited(object body)
	{
		LeaveShop();
	}

	private void _on_ManaShopDoors_body_entered(object body)
	{
		EnterShop(ShopType.Mana, ManaShopDoors);
	}

	private void _on_ManaShopDoors_body_exited(object body)
	{
		LeaveShop();
	}

	private void _on_DmgShopDoors_body_entered(object body)
	{
		EnterShop(ShopType.Dmg, DmgShopDoors);
	}

	private void _on_DmgShopDoors_body_exited(object body)
	{
		LeaveShop();
	}

	private void _on_EndingArea2D_body_entered(object body)
	{
		GetTree().Paused = true;
		ending.changeVisible();
	}

	// ---- Portale i NPC: podpowiedź "Press F" ----

	private void ShowInteractionLabel(string nodeName, string text)
	{
		interactionLabel.SetPosition(GetNode<Node2D>(nodeName).Position + new Vector2(-90, -136));
		interactionLabel.Text = text;
		interactionLabel.Show();
	}

	private string PortalDestination(string portalName)
	{
		switch (portalName)
		{
			case "DesertPortal": return SceneManager.LocationScene("Desert", "Desertlvl0");
			case "CementaryPortal": return SceneManager.LocationScene("Cemetery", "Cemeterylvl1");
			case "JunglePortal": return SceneManager.LocationScene("Jungle", "Junglelvl1");
			default: return null;
		}
	}

	// Podpięte w Miasto.tscn z bindem: nazwą węzła portalu.
	private void _on_Portal_body_entered(object body, string portalName)
	{
		currentPortalScene = PortalDestination(portalName);
		ShowInteractionLabel(portalName, "Press \"F\"\n to enter");
	}

	private void _on_Portal_body_exited(object body, string portalName)
	{
		currentPortalScene = null;
		interactionLabel.Hide();
	}

	// Podpięte w Miasto.tscn z bindem: nazwą węzła NPC (King, Smith).
	private void _on_Npc_body_entered(object body, string npcName)
	{
		ShowInteractionLabel(npcName, "Press \"F\"\n to talk");
	}

	private void _on_Npc_body_exited(object body)
	{
		interactionLabel.Hide();
	}

	// ---- Wyniki zakupów w sklepach ----

	private void _on_HpShop_stat_bought(float value)
	{
		player.SetMaxHealth(value);
	}

	private void _on_ManaShop_stat_bought(float value)
	{
		player.SetMaxMana(value);
	}

	private void _on_DmgShop_stat_bought(float value)
	{
		player.SetDamage(value);
	}
}
