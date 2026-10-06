using Godot;
using System;

public class DesertToTownPortal : Node2D
{
	private void _on_TownPortalFromDesert_body_entered(object body)
	{
		if(body is Movement){
			SceneManager.Go(GetTree(), SceneManager.TownScene);
		}
	}
}
