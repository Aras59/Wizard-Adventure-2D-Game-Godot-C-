using Godot;
using System;

public class Pause : CanvasLayer
{
	private Control pause;
	private bool isPressed;
	private Label play_time;
	private Label deaths;
	Timer timer;
	public override void _Ready()
	{
		pause = GetNode<Control>("Pause");
		play_time = ((Label)GetNode("Pause/TimeLabel"));
		deaths = ((Label)GetNode("Pause/Deaths"));
		timer = GetNode<Timer>("Timer");
		timer.OneShot = true;
		timer.WaitTime = 1f;
		timer.Autostart = true;
	}
	
	
	public void GetInput(){
		
		if(Input.IsActionPressed("pause")){
			GetTree().Paused = true;
			pause.Visible = true;
			long time = Movement.getTime() + Movement.sec_left;
			long minutes = time/60000;
			long seconds = time/1000 - minutes*60;
			play_time.Text = String.Format("Played time: {0:00} m  ", minutes)+ String.Format("{0:00} s", seconds);
			deaths.Text = String.Format("Deaths: {0:00}", Movement.numOfDeaths);
		}
		
	}
		
	public override void _PhysicsProcess(float delta){
		GetInput();
	}
	
	private void _on_MenuButton_pressed()
	{
		GetTree().Paused = false;
		GetTree().ChangeScene("res://Scenes/Menu/Menu.tscn");
		Movement.startCounting();
	}
	
	private void _on_ContinueButton_pressed()
	{
			GetTree().Paused = false;
			pause.Visible = false;
			Movement.startCounting();
	}
	
}









