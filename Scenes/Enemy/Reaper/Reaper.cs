using Godot;
using System;

public class Reaper : EnemyBase
{
	private Timer timer3;

	public Reaper()
	{
		life = 200f;
		damage = 33f;
	}

	protected override string SpriteName { get { return "Reaper"; } }
	protected override string DeathSoundName { get { return "SoundDeath"; } }
	protected override string WalkAnimation { get { return "Walking"; } }

	public override void _Ready()
	{
		base._Ready();
		timer3 = SetupTimer("Timer3", 3f);
	}

	// Podczas ataku Reaper staje się widoczny i podatny na trafienia, potem znów znika.
	protected override void OnHit()
	{
		(sprite.Material as ShaderMaterial).SetShaderParam("flash_modifier", 0);
		timer3.Start();
		this.CollisionLayer = 16;
	}

	protected override void OnRotated()
	{
		RayCast2D edgeCheck = GetNode<RayCast2D>("RayCast2D");
		edgeCheck.Scale = new Vector2(-edgeCheck.Scale.x, edgeCheck.Scale.y);
	}

	private void _on_Timer3_timeout()
	{
		(sprite.Material as ShaderMaterial).SetShaderParam("flash_modifier", 0.3);
		this.CollisionLayer = 0;
	}
}
