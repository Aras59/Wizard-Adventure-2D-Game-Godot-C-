using Godot;
using System;

public class EnemyOrc : EnemyBase
{
	public EnemyOrc()
	{
		life = 150f;
		damage = 33f;
	}

	protected override string SpriteName { get { return "Orc"; } }
}
