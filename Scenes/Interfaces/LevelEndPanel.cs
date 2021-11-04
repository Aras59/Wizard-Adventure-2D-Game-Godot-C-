using Godot;
using System;

public class LevelEndPanel : CanvasLayer
{
	private Control LevelEnd;
	private String nextLevelName = "Miasto";
	private String biom = "Town";
	private Label totalCoinsLabel;
	private Label coinsLabel;
	private Label timeLabel;
	private Label deathsLabel;
	
	public override void _Ready()
	{
		LevelEnd =  GetNode<Control>("LevelEnd");
		totalCoinsLabel = (Label)(LevelEnd.GetNode("Tcoins"));
		coinsLabel = (Label)(LevelEnd.GetNode("Lcoins"));
		timeLabel = (Label)(LevelEnd.GetNode("TimeLabel"));
		deathsLabel = (Label)(LevelEnd.GetNode("DeathsLabel"));
	}

	private void changeScene(String path){
		Movement.sec_left += Movement.getTime();
		Movement.deaths_level = 0;
		GetTree().ChangeScene(path);
	}
	
	private void _on_ContinueButton_pressed()
	{
		GetTree().Paused = false;
		if(biom.Equals("Town")){
			changeScene("res://Scenes/Locations/"+biom+"/"+nextLevelName+".tscn");
		}else
		changeScene("res://Scenes/Locations/"+biom+"/"+nextLevelName+"/"+nextLevelName+".tscn");
	}
	
	private void _on_MenuButton_pressed()
	{
		GetTree().Paused = false;
		changeScene("res://Scenes/Menu/Menu.tscn");
	}
	
	public void changeVisible(){
		
		LevelEnd.Visible = !LevelEnd.Visible;
	}
	
	private void _on_NextLevelArea_levelEnd(String nextLevelName, String Biom)
	{
		this.nextLevelName = nextLevelName;
		this.biom = Biom;
	}
	
	private void _on_HUD_ending_signal(int totalCoins, int coins)
	{
		long time = Movement.level_time();
		long minutes = time/60000;
		long seconds = time/1000 - minutes*60;
		timeLabel.Text = String.Format("Played time: {0:00} m  ", minutes)+ String.Format("{0:00} s", seconds);
		deathsLabel.Text = String.Format("Deaths at level: {0:00}", Movement.deaths_level);
		totalCoinsLabel.Text = String.Format("Total Coins: {0:00}", totalCoins);
		coinsLabel.Text = String.Format("Coins Level: {0:00}", coins);
	}
	
	
}



