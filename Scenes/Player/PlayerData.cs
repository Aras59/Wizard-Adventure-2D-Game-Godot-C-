using System.Diagnostics;

/// <summary>
/// Stan gry, który przeżywa zmiany scen (statystyki, postęp, monety, czas, śmierci).
/// Wszystko, co wcześniej leżało w polach static w Movement / HUD / Miasto.
/// </summary>
public static class PlayerData
{
	public const float DefaultMaxHealth = 100f;
	public const float DefaultMaxMana = 60f;
	public const float DefaultDamage = 50f;

	// Statystyki (ulepszane w sklepach)
	public static float MaxHealth = DefaultMaxHealth;
	public static float MaxMana = DefaultMaxMana;
	public static float Damage = DefaultDamage;
	public static float HealthRegeneration = 1f;
	public static float ManaRegeneration = 5f;

	// Postęp fabularny
	public static int StoryStage = 0;
	public static bool DesertBossDead = false;
	public static bool JungleBossDead = false;
	public static bool CemeteryBossDead = false;
	public static bool DesertPortalUnlocked = false;
	public static bool CemeteryPortalUnlocked = false;
	public static bool JunglePortalUnlocked = false;
	public static bool TownStoneBlocking = true;

	// Monety zebrane w całej grze (bez monet z bieżącego levelu)
	public static int TotalCoins = 0;

	// Statystyki przejścia gry
	public static int TotalDeaths = 0;
	public static int LevelDeaths = 0;
	public static long AccumulatedMs = 0;

	private static Stopwatch sceneStopwatch = new Stopwatch();
	private static Stopwatch levelStopwatch = new Stopwatch();

	/// <summary>Przywraca stan do początkowego, używane przy "Nowa gra".</summary>
	public static void Reset()
	{
		MaxHealth = DefaultMaxHealth;
		MaxMana = DefaultMaxMana;
		Damage = DefaultDamage;
		HealthRegeneration = 1f;
		ManaRegeneration = 5f;

		StoryStage = 0;
		DesertBossDead = false;
		JungleBossDead = false;
		CemeteryBossDead = false;
		DesertPortalUnlocked = false;
		CemeteryPortalUnlocked = false;
		JunglePortalUnlocked = false;
		TownStoneBlocking = true;

		TotalCoins = 0;
		TotalDeaths = 0;
		LevelDeaths = 0;
		AccumulatedMs = 0;
		sceneStopwatch = new Stopwatch();
		levelStopwatch = new Stopwatch();
	}

	/// <summary>Wołane przy wejściu na scenę z graczem: mierzy czas od zera dla tej sceny.</summary>
	public static void StartSceneTimers()
	{
		sceneStopwatch = Stopwatch.StartNew();
		levelStopwatch = Stopwatch.StartNew();
	}

	public static void ResumeTimer()
	{
		sceneStopwatch.Start();
	}

	/// <summary>Czas bieżącej sceny. Zatrzymuje stoper (wznawia go ResumeTimer).</summary>
	public static long StopSceneTimer()
	{
		sceneStopwatch.Stop();
		return sceneStopwatch.ElapsedMilliseconds;
	}

	/// <summary>Czas bieżącego levelu. Zatrzymuje stoper levelu.</summary>
	public static long StopLevelTimer()
	{
		levelStopwatch.Stop();
		return levelStopwatch.ElapsedMilliseconds;
	}

	/// <summary>Czas przejścia od początku gry (zakończone sceny + bieżąca).</summary>
	public static long TotalPlayTimeMs()
	{
		return StopSceneTimer() + AccumulatedMs;
	}
}
