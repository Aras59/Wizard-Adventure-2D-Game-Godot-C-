using Godot;
using System;

/// <summary>
/// Jeden panel sklepu dla każdej ulepszanej statystyki (HP, mana, obrażenia).
/// Sceny HpShop / ManaShop / DmgShop różnią się tylko wyglądem i wartościami eksportów.
///
/// Wymagane węzły: [layerName] (Control) z dziećmi CoinsLabel, CostLabel i [valueLabelName].
/// </summary>
public class ShopPanel : CanvasLayer
{
	[Export] private string layerName = "HpLayer";
	[Export] private string valueLabelName = "Hp";
	[Export] private int step = 10;
	[Export] private int costPerStep = 15;
	/// <summary>Wartość statystyki musi pozostać poniżej tego limitu (kupić można, dopóki wartość + krok &lt; limit).</summary>
	[Export] private int limit = 210;

	[Signal] public delegate void stat_bought(float value);
	[Signal] public delegate void setcoins(int coins);

	private Control layer;
	private Label coinsLabel;
	private Label costLabel;
	private Label valueLabel;
	private int availableCoins;
	private int pendingCost = 0;
	private float ownedValue = 0;

	public override void _Ready()
	{
		layer = GetNode<Control>(layerName);
		coinsLabel = layer.GetNode<Label>("CoinsLabel");
		costLabel = layer.GetNode<Label>("CostLabel");
		valueLabel = layer.GetNode<Label>(valueLabelName);
		ShowCost();
	}

	public void changeVisible()
	{
		layer.Visible = !layer.Visible;
	}

	private void ShowCost()
	{
		costLabel.Text = String.Format("Cost: {0:00}", pendingCost);
	}

	/// <summary>Miasto podaje aktualną wartość statystyki gracza po wejściu do sklepu.</summary>
	private void _on_Miasto_stat_loaded(float value)
	{
		ownedValue = value;
		valueLabel.Text = ownedValue.ToString();
	}

	private void _on_HUD_ending_signal(int totalCoins, int coins)
	{
		availableCoins = totalCoins;
		coinsLabel.Text = "Coins:  " + totalCoins;
	}

	private void _on_PlusButton_pressed()
	{
		int shown = int.Parse(valueLabel.Text);
		if (shown + step < limit)
		{
			valueLabel.Text = (shown + step).ToString();
			pendingCost += costPerStep;
			ShowCost();
		}
	}

	private void _on_MenuButton_pressed()
	{
		GetTree().Paused = false;
		changeVisible();
		pendingCost = 0;
		ShowCost();
		valueLabel.Text = ownedValue.ToString();
	}

	private void _on_ContinueButton_pressed()
	{
		if (availableCoins - pendingCost >= 0 && pendingCost != 0)
		{
			ownedValue = int.Parse(valueLabel.Text);
			EmitSignal(nameof(stat_bought), ownedValue);
			availableCoins -= pendingCost;
			EmitSignal(nameof(setcoins), availableCoins);
			GetTree().Paused = false;
			changeVisible();
			pendingCost = 0;
			ShowCost();
			coinsLabel.Text = "Coins:  " + availableCoins;
			valueLabel.Text = ownedValue.ToString();
		}
	}
}
