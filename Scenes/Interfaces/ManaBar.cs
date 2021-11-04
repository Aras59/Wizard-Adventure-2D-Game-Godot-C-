using Godot;
using System;

public class ManaBar : HBoxContainer
{
	private TextureProgress manabar;

	public override void _Ready()
	{
		manabar = GetNode<TextureProgress>("Mana");
	}

	public void _mana_changed(float mana)
	{
		manabar.Value = mana;
	}

	public void _mmana_changed(float mana)
	{
		manabar.MaxValue = mana;
		manabar.Value = mana;
	}
}
