using Godot;

/// <summary>
/// Jedno miejsce na ścieżki scen i zmianę scen, zamiast stringów rozsianych po skryptach.
/// </summary>
public static class SceneManager
{
	public const string MenuScene = "res://Scenes/Menu/Menu.tscn";
	public const string TownScene = "res://Scenes/Locations/Town/Miasto.tscn";

	/// <summary>Np. LocationScene("Desert", "Desertlvl1") -> .../Desert/Desertlvl1/Desertlvl1.tscn</summary>
	public static string LocationScene(string biom, string level)
	{
		return "res://Scenes/Locations/" + biom + "/" + level + "/" + level + ".tscn";
	}

	/// <summary>Zmiana sceny bez dotykania liczników (menu, początek gry).</summary>
	public static void Go(SceneTree tree, string path)
	{
		tree.ChangeScene(path);
	}

	/// <summary>
	/// Zmiana sceny w trakcie rozgrywki: czas spędzony na bieżącej scenie jest dodawany
	/// do całkowitego czasu gry, a opcjonalnie zerowane są śmierci na levelu.
	/// </summary>
	public static void GoAndRecordTime(SceneTree tree, string path, bool resetLevelDeaths = false)
	{
		PlayerData.AccumulatedMs += PlayerData.StopSceneTimer();
		if (resetLevelDeaths)
		{
			PlayerData.LevelDeaths = 0;
		}
		tree.ChangeScene(path);
	}
}
