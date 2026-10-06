using Godot;
using System;

/// <summary>
/// Przeciwnik, który zamiast uderzać wręcz, rzuca pociskiem (Ogre, Wraith).
/// Atak kończy się po AttackDelay, a nie po animacji.
/// </summary>
public abstract class RangedEnemy : EnemyBase
{
	private PackedScene projectileScene;

	/// <summary>Ścieżka do sceny pocisku. Scena musi mieć skrypt implementujący IProjectile.</summary>
	protected abstract string ProjectilePath { get; }

	protected override float AttackDelay { get { return 1f; } }

	public override void _Ready()
	{
		base._Ready();
		projectileScene = (PackedScene)ResourceLoader.Load(ProjectilePath);
	}

	protected override void OnHit()
	{
		Throw();
	}

	protected override void OnHitWindow()
	{
		if (!isDead)
		{
			_EndOfHit();
		}
	}

	protected override void OnHitAnimationFinished()
	{
		sprite.Play(IdleAnimation);
	}

	private void Throw()
	{
		Vector2 directionVector = direction > 0 ? Vector2.Right : Vector2.Left;
		float xToMove = direction > 0 ? 60.0f : -60.0f;

		Node2D projectile = (Node2D)projectileScene.Instance();
		GetParent().AddChild(projectile);
		projectile.GlobalPosition = GlobalPosition + new Vector2(xToMove, 10.0f);
		((IProjectile)projectile).setup(directionVector);
	}
}
