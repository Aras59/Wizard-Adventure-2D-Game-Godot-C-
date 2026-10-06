using Godot;
using System;

public class Satyr_Mag : EnemyBase
{
	private bool isHealing = false;
	private float healthReg = 350f;
	private AudioStreamPlayer soundHeal;
	[Signal] public delegate void i_am_dead();

	public Satyr_Mag()
	{
		life = 250f;
		damage = 33f;
	}

	protected override string SpriteName { get { return "Satyr_Mag"; } }
	protected override string RunAnimation { get { return "Walk"; } }

	public override void _Ready()
	{
		base._Ready();
		soundHeal = GetNode<AudioStreamPlayer>("SoundHeal");
	}

	protected override bool IsBusy()
	{
		return base.IsBusy() || isHealing;
	}

	protected override void OnBusyTick()
	{
		if (isHitting && timer.GetTimeLeft() == 0)
		{
			OnHitWindow();
		}
		else if (isHealing && timer.GetTimeLeft() == 0)
		{
			GetNode<Area2D>("DetectWarrior").Monitoring = true;
		}
	}

	protected override void OnDead()
	{
		EmitSignal(nameof(i_am_dead));
	}

	protected override void OnOtherAnimationFinished()
	{
		if (isHealing)
			_EndOfHeal();
	}

	public void _Heal()
	{
		DisableShape("DetectWarrior/CollisionShape2D");
		isHealing = true;
		sprite.Stop();
		sprite.Play("Taunt");
		soundHeal.Play();
		timer.Start();
	}

	public void _EndOfHeal()
	{
		isHealing = false;
		GetNode<Area2D>("DetectWarrior").Monitoring = true;
		EnableShape("DetectWarrior/CollisionShape2D");
	}

	private void _on_DetectWarrior_body_entered(object body)
	{
		if (body is Satyr_Warrior)
		{
			if ((body as Satyr_Warrior)._Heal(healthReg))
			{
				_Heal();
			}
		}
		else if (body is Satyr_Boss)
		{
			if ((body as Satyr_Boss)._Heal(healthReg))
			{
				_Heal();
			}
		}
	}
}
