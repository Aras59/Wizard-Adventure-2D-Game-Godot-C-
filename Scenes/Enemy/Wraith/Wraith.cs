using Godot;
using System;

public class Wraith : RangedEnemy
{
	public Wraith()
	{
		life = 180f;
		damage = 33f;
	}

	protected override string SpriteName { get { return "Wraith"; } }
	protected override string DeathSoundName { get { return "SoundDeath"; } }
	protected override string ProjectilePath { get { return "res://Scenes/Attacks/DarkBall/DarkBall.tscn"; } }

	// Wraith nie ma osobnej animacji biegu.
	protected override string RunAnimation { get { return "Walk"; } }
}
