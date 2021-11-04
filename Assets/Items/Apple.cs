using Godot;
using System;

public class Apple : Sprite
{
	[Signal]
	public delegate void apple_collected();
	public float heal_value = 20f;

	private void _on_Apple_body_entered(object body)
	{
		if (body is Movement)
		{
			((Movement)body)._Heal(heal_value);
			EmitSignal(nameof(apple_collected));
			QueueFree();
		}
	}
}
