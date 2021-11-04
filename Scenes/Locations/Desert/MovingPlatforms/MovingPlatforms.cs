using Godot;
using System;

public class MovingPlatforms : Node2D
{
	private AnimationPlayer _animationPlayer;
	[Export] String type = "";
	public override void _Ready()
	{
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		if(type.Equals("Circle")){
			_animationPlayer.Play("Circle");
		}else{
			_animationPlayer.Play("CircleMove");
		}
		
	}

}
