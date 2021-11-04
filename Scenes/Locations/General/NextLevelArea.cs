using Godot;
using System;

public class NextLevelArea : Area2D
{
	[Export] public String nextLevelName = "";
	[Export] public String biom = "";
	[Signal] public delegate void levelEnd(String nextLevelName,String Biom);
	public override void _Ready()
	{
	}
	
	private void _on_NextLevelArea_body_entered(object body)
	{
		EmitSignal(nameof(levelEnd),this.nextLevelName,this.biom);
	}
}



