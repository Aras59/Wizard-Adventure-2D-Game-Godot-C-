using Godot;
using System;

public class HPChange : Node
{
	// Declare member variables here. Examples:
	// private int a = 2;
	// private string b = "text";
	[Signal] public delegate void player_stats_changed();
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		EmitSignal(nameof(player_stats_changed));
	}

//  // Called every frame. 'delta' is the elapsed time since the previous frame.
//  public override void _Process(float delta)
//  {
//      
//  }
}
