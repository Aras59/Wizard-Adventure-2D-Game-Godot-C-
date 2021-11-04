using Godot;
using System;

public class Smith : KinematicBody2D
{
	
	public override void _Ready()
	{
		
	}
	
	public override void _PhysicsProcess(float delta){
		
		((AnimatedSprite)GetNode("smith")).Play("default");
			
	}
	
}








