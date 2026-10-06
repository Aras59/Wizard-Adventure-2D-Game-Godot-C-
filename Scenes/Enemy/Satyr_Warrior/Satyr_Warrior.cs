using Godot;
using System;

public class Satyr_Warrior : EnemyBase
{
	private float maxLife = 350f;
	private Timer timer3;

	public Satyr_Warrior()
	{
		life = 350f;
		damage = 33f;
	}

	protected override string SpriteName { get { return "Satyr_Warrior"; } }
	protected override string RunAnimation { get { return "Walk"; } }

	public override void _Ready()
	{
		base._Ready();
		timer3 = SetupTimer("Timer3", 1f);
	}

	public override void _PhysicsProcess(float delta)
	{
		// Ranny satyr jest podatny na trafienia (warstwa 144), zdrowy tylko na pociski gracza (16).
		this.CollisionLayer = life < maxLife ? 144u : 16u;
		base._PhysicsProcess(delta);
	}

	public bool _Heal(float health)
	{
		if (life < maxLife)
		{
			timer3.Start();
			life = Math.Min(life + health, maxLife);
			(sprite.Material as ShaderMaterial).SetShaderParam("isOn", true);
			this.CollisionLayer = 16;
			return true;
		}
		return false;
	}

	// Oryginalne zachowanie: zajście od tyłu tylko obraca wojownika, bez szarży
	// (SPEED wracał do normy w następnej klatce, bo Timer2 nie był uruchamiany).
	protected override void OnPlayerBehind()
	{
		_Rotate();
	}

	private void _on_Timer3_timeout()
	{
		(sprite.Material as ShaderMaterial).SetShaderParam("isOn", false);
	}
}
