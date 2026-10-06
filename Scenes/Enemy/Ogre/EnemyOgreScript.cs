using Godot;
using System;

public class EnemyOgreScript : RangedEnemy
{
	public EnemyOgreScript()
	{
		life = 150f;
		damage = 33f;
	}

	protected override string SpriteName { get { return "Ogre"; } }
	protected override string ProjectilePath { get { return "res://Scenes/Attacks/Rock/Rock.tscn"; } }
}
