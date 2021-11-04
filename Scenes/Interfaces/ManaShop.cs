using Godot;
using System;

public class ManaShop : CanvasLayer
{
	
	private Control ManaLayer;
	private Label coinsLabel;
	private Label cost;
	private Label  Mana;
	private int coin;
	private int c = 0;
	private float manalevel;
	[Signal] public delegate void setmana(float mana1);
	[Signal] public delegate void setcoins(int coins1);
	
	
	public override void _Ready()
	{
		manalevel = 0;
		ManaLayer = GetNode<Control>("ManaLayer");
		coinsLabel = (Label)(ManaLayer.GetNode("CoinsLabel"));
		cost = (Label)(ManaLayer.GetNode("CostLabel"));
		cost.Text = String.Format("Cost: {0:00}", 0);
		Mana = (Label)(ManaLayer.GetNode("Mana"));
	}
	
	public void changeVisible(){
		ManaLayer.Visible = !ManaLayer.Visible;
	}
	
	private void _on_MenuButton_pressed()
	{
		GetTree().Paused = false;
		this.changeVisible();
		c=0;
		cost.Text = String.Format("Cost: {0:00}", c);
		Mana.Text = manalevel.ToString();
	}
	
	private void _on_HUD_ending_signal(int totalCoins, int coins)
	{
		coin = totalCoins;
		coinsLabel.Text ="Coins:  "+totalCoins;
	}
	
	private void _on_Miasto_getmana(float mana)
	{
		manalevel = mana;
		Mana.Text = manalevel.ToString();
	}
	
	private void _on_ContinueButton_pressed()
	{
		if(coin-c>=0 && c!=0){
			manalevel = int.Parse(Mana.Text);
			EmitSignal(nameof(setmana),manalevel);
			coin = coin-c;
			EmitSignal(nameof(setcoins),coin);
			GetTree().Paused = false;
			this.changeVisible();
			c=0;
			cost.Text = String.Format("Cost: {0:00}", c);
			coinsLabel.Text ="Coins:  "+coin.ToString();
			Mana.Text = manalevel.ToString();
		}
	}
	
	private void _on_PlusButton_pressed()
	{
		int temp = int.Parse(Mana.Text);
		if(temp+10<130){
			temp = temp+10;
			Mana.Text = temp.ToString();
			c = c+20;
			cost.Text = String.Format("Cost: {0:00}", c);
		}
	}
	
}


