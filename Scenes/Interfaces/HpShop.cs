using Godot;
using System;

public class HpShop : CanvasLayer
{

	private Control HpLayer;
	private Label coinsLabel;
	private Label cost;
	private Label  Hp;
	private int coin;
	private int c = 0;
	private int hplevel;
	[Signal] public delegate void sethp(float health1);
	[Signal] public delegate void setcoins(int coins1);
	
	public override void _Ready()
	{
		hplevel = 0;
		HpLayer = GetNode<Control>("HpLayer");
		coinsLabel = (Label)(HpLayer.GetNode("CoinsLabel"));
		cost = (Label)(HpLayer.GetNode("CostLabel"));
		cost.Text = String.Format("Cost: {0:00}", 0);
		Hp = (Label)(HpLayer.GetNode("Hp"));
	}
	
	public void changeVisible(){
		HpLayer.Visible = !HpLayer.Visible;
	}
	
	private void _on_MenuButton_pressed()
	{
		GetTree().Paused = false;
		this.changeVisible();
		c=0;
		cost.Text = String.Format("Cost: {0:00}", c);
		Hp.Text = hplevel.ToString();
	}
	
	private void _on_PlusButton_pressed()
	{
		hplevel = int.Parse(Hp.Text);
		
		if(hplevel+10<210){
			hplevel = hplevel+10;
			Hp.Text = hplevel.ToString();
			cost.Text = String.Format("Cost: {0:00}", c+15);
			c = c+15;
		}
		
		
	}
	
	private void _on_HUD_ending_signal(int totalCoins, int coins)
	{
		coin = totalCoins;
		coinsLabel.Text ="Coins:  "+totalCoins;
	}
	
	private void _on_Miasto_gethp(float health)
	{
		
		hplevel = (int)health;
		Hp.Text = hplevel.ToString();
	}
	
	private void _on_ContinueButton_pressed()
	{
		if(coin-c>=0 && c!=0){
			EmitSignal(nameof(sethp),(float)hplevel);
			coin = coin-c;
			coinsLabel.Text ="Coins:  "+c;
			EmitSignal(nameof(setcoins),coin);
			GetTree().Paused = false;
			this.changeVisible();
			c=0;
			cost.Text = String.Format("Cost: {0:00}", c);
			Hp.Text = hplevel.ToString();
			
		}
		
	}
	
	
	
}


















