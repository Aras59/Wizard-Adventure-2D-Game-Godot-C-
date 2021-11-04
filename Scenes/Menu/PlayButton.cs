using Godot;
using System;

public class PlayButton : Button
{
	private void _on_PlayButton_pressed()
	{
		GetTree().ChangeScene("res://Scenes/Locations/Town/Miasto.tscn");
	}
}



