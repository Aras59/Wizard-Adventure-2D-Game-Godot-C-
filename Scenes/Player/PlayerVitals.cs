using System;

/// <summary>Aktualne HP i mana gracza w obrębie jednej sceny (maksima i regeneracja siedzą w PlayerData).</summary>
public class PlayerVitals
{
	public float Health;
	public float Mana;

	public bool IsDead { get { return Health <= 0; } }

	public void Refill()
	{
		Health = PlayerData.MaxHealth;
		Mana = PlayerData.MaxMana;
	}

	public void TakeDamage(float amount)
	{
		Health -= amount;
	}

	public void Heal(float amount)
	{
		Health = Math.Min(Health + amount, PlayerData.MaxHealth);
	}

	/// <summary>Zwraca true, jeśli mana się zmieniła.</summary>
	public bool RegenerateMana(float delta)
	{
		float newMana = Math.Min(Mana + PlayerData.ManaRegeneration * delta, PlayerData.MaxMana);
		if (newMana != Mana && newMana > -1)
		{
			Mana = newMana;
			return true;
		}
		return false;
	}

	public bool CanSpend(float cost)
	{
		return Mana >= cost;
	}

	public void Spend(float cost)
	{
		Mana -= cost;
	}
}
