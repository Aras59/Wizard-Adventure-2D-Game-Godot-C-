using Godot;
using System;

public class EndingPanel : CanvasLayer
{
	private Control Ending;
	private Label timeLabel;
	private Label deathsLabel;
	public override void _Ready()
	{
		Ending = GetNode<Control>("EndingPanel");
		timeLabel = (Label)(Ending.GetNode("TimeLabel"));
		deathsLabel = (Label)(Ending.GetNode("DeathsLabel"));
	}
	
	private void _on_MenuButton_pressed()
	{
	   GetTree().Quit();
	}
	
	public void changeVisible(){
		long time = Movement.getTime() + Movement.sec_left;
		long minutes = time/60000;
		long seconds = time/1000 - minutes*60;
		timeLabel.Text = String.Format("Played time: {0:00} m  ", minutes)+ String.Format("{0:00} s", seconds);
		deathsLabel.Text = String.Format("Deaths: {0:00}", Movement.numOfDeaths);
		Ending.Visible = !Ending.Visible;
	}
	
}



