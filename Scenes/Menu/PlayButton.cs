using Godot;
using System;

public class PlayButton : Button
{
	private void _on_PlayButton_pressed()
	{
		PlayerData.Reset();
		SceneManager.Go(GetTree(), SceneManager.TownScene);
	}
}



