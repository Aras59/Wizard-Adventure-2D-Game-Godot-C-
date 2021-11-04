using Godot;
using System;

public class FireBall : Area2D
{
	[Signal]
	public delegate void fireball_attack();
	private AnimatedSprite fireball;
	private AudioStreamPlayer soundHit;

	[Export] float speed = 700.0f;
	[Export] float lifeTime = 10.0f;
	[Export] float damage = 50.0f;

	Vector2 direction = Vector2.Right;
	static int bossHitted = 0;

	public void setDmg(float dmg)
	{
		damage = dmg;
	}

	public override void _Ready()
	{
		fireball = GetNode<AnimatedSprite>("SpellSprite");
		soundHit = GetNode<AudioStreamPlayer>("SoundHit");
	}

	public override void _PhysicsProcess(float delta)
	{
		fireball.Play("FireBall");
		lifeTime -= .1f;
		if (lifeTime > 0)
			Position += direction.Normalized() * speed * delta;
		else
			QueueFree();
	}

	public void setup(Vector2 dir)
	{
		direction = dir;
	}

	public void setPosition(Vector2 pos)
	{
		Position = pos;
	}

	private void _on_FireBall_body_entered(object body)
	{
		if (body is IEnemy)
		{
			if (body is EnemyGoblinBoss && bossHitted < 3)
			{
				EmitSignal(nameof(fireball_attack));
				lifeTime += 10;
				direction *= -1;
				bossHitted++;
			}
			else
			{
				QueueFree();
			}

			((IEnemy)body)._Hurt(damage);

			if (bossHitted == 3)
			{
				bossHitted = 0;
				soundHit.Play();
				EmitSignal(nameof(fireball_attack));
				QueueFree();
				return;
			}
			EmitSignal(nameof(fireball_attack));
		}
		else if (body is Movement)
		{
			((Movement)body)._Hurt(damage * 1.5f);
			EmitSignal(nameof(fireball_attack));
			QueueFree();
		}
		else
			QueueFree();
	}

	private void _on_FireBall_area_entered(object area)
	{
		if (area is RollingStone)
		{
			lifeTime += 10;
			direction *= -1;
			speed = 1000.0f;
		}
	}
}
