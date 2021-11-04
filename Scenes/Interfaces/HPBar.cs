using Godot;
using System;

public class HPBar : HBoxContainer
{
	private TextureProgress lifebar;

	public override void _Ready()
	{
		lifebar = GetNode<TextureProgress>("LifeBar");
	}

	public void _hp_changed(float health)
	{
		lifebar.Value = health;
	}

	public void _mhp_changed(float health)
	{
		lifebar.MaxValue = health;
		lifebar.Value = health;
	}
}
