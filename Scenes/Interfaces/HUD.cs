using Godot;
using System;


public class HUD : CanvasLayer
{
	static int totalCoins = 0;
	static int startMillis = 0;
	int coins = 0;
	private Label label;
	private HPBar hpbar;
	private ManaBar manabar;
	private AudioStreamPlayer _soundCollectingCoin;
	
	[Signal] public delegate void ending_signal(int totalCoins, int coins);

	public override void _Ready()
	{
		coins = 0;
		label = ((Label)GetNode("Coins_counter"));
		label.Text = String.Format("{0:00}", totalCoins + coins);
		hpbar = GetNode<HPBar>("HPBar");
		manabar = GetNode<ManaBar>("ManaBar");
		_soundCollectingCoin = GetNode<AudioStreamPlayer>("SoundCollectingCoin");
	}

	public void _on_Coin_coin_collected()
	{
		_soundCollectingCoin.Play();
		coins = coins + 1;
		label.Text = String.Format("{0:00}", totalCoins + coins);

	}

	private void _on_Player_hp_changed(float health)
	{
		hpbar._hp_changed(health);
	}

	private void _on_Player_mana_changed(float mana)
	{
		manabar._mana_changed(mana);
	}

	private void _on_NextLevelArea_body_entered(object body)
	{
		totalCoins = totalCoins + coins;
		EmitSignal(nameof(ending_signal), totalCoins, coins);
	}

	private void _on_DesertToTownPortal_body_entered(object body)
	{
		totalCoins = totalCoins + coins;
		EmitSignal(nameof(ending_signal), totalCoins, coins);
	}

	private void _on_HpShopDoors_body_entered(object body)
	{
		totalCoins = totalCoins + coins;
		EmitSignal(nameof(ending_signal), totalCoins, coins);
	}

	private void _on_ManaShopDoors_body_entered(object body)
	{
		totalCoins = totalCoins + coins;
		EmitSignal(nameof(ending_signal), totalCoins, coins);
	}

	private void _on_DmgShopDoors_body_entered(object body)
	{
		totalCoins = totalCoins + coins;
		EmitSignal(nameof(ending_signal), totalCoins, coins);
	}

	private void _on_HpShop_setcoins(int coins1)
	{
		totalCoins = coins1;
		label.Text = String.Format("{0:00}", totalCoins);
	}

	private void _on_DmgShop_setcoins(int coins1)
	{
		totalCoins = coins1;
		label.Text = String.Format("{0:00}", totalCoins);
	}

	private void _on_ManaShop_setcoins(int coins1)
	{
		totalCoins = coins1;
		label.Text = String.Format("{0:00}", totalCoins);
	}

	private void _on_Player_mhp_changed(float health)
	{
		hpbar._mhp_changed(health);
	}

	private void _on_Player_mmana_changed(float mana)
	{
		manabar._mmana_changed(mana);
	}

	private void _on_ToTown_body_entered(object body)
	{
		totalCoins = totalCoins + coins;
		EmitSignal(nameof(ending_signal), totalCoins, coins);
	}
}
