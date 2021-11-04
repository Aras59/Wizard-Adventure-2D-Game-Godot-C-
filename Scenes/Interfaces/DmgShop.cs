using Godot;
using System;

public class DmgShop : CanvasLayer
{
	
	private Control DmgLayer;
	private Label coinsLabel;
	private Label cost;
	private Label  Dmg;
	private int coin;
	private int c = 0;
	private float dmglevel=0f;
	[Signal] public delegate void setdmg(float dmg1);
	[Signal] public delegate void setcoins(int coins1);
	
	
	public override void _Ready()
	{
		DmgLayer = GetNode<Control>("DmgLayer");
		coinsLabel = (Label)(DmgLayer.GetNode("CoinsLabel"));
		cost = (Label)(DmgLayer.GetNode("CostLabel"));
		cost.Text = String.Format("Cost: {0:00}", 0);
		Dmg = (Label)(DmgLayer.GetNode("Dmg"));
	}
	
	public void changeVisible(){
		DmgLayer.Visible = !DmgLayer.Visible;
	}
	
	private void _on_MenuButton_pressed()
	{
		GetTree().Paused = false;
		this.changeVisible();
		c=0;
		cost.Text = String.Format("Cost: {0:00}", c);
		Dmg.Text = dmglevel.ToString();
	}
	
	private void _on_Miasto_getdmg(float dmg)
	{
		dmglevel = dmg;
		Dmg.Text = dmglevel.ToString();
	}
	
	private void _on_HUD_ending_signal(int totalCoins, int coins)
	{
		coin = totalCoins;
		coinsLabel.Text ="Coins:  "+totalCoins;
	}
	
	private void _on_PlusButton_pressed()
	{
		dmglevel = int.Parse(Dmg.Text);
		if(dmglevel+10<130){
			dmglevel = dmglevel+10;
			Dmg.Text = dmglevel.ToString();
			c = c+25;
			cost.Text = String.Format("Cost: {0:00}", c);
			
		}
	}
	
	private void _on_ContinueButton_pressed()
	{
		if(coin-c>=0 && c!=0){
			EmitSignal(nameof(setdmg),dmglevel);
			coin = coin - c;
			coinsLabel.Text ="Coins:  "+c;
			EmitSignal(nameof(setcoins),coin);
			GetTree().Paused = false;
			this.changeVisible();
			c=0;
			cost.Text = String.Format("Cost: {0:00}", c);
			Dmg.Text = dmglevel.ToString();
		}
	}
	

}

