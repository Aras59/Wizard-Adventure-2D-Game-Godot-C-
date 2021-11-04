using Godot;
using System;

public class Coin : Area2D
{
	[Signal]
	public delegate void coin_collected();
	
	public override void _Ready() {}

	private void _on_Coin_body_entered(object body) {
		EmitSignal(nameof(coin_collected));
		QueueFree();
	}
}
