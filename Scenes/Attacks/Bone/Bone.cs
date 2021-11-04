using Godot;
using System;

public class Bone : Area2D
{
	Vector2 THROW_VELOCITY = new Vector2(800, -400);
	int gravity = 200;
	
	[Export] float speed = 700.0f;
	[Export] float lifeTime = 100.0f;
	[Export] float damage = 50.0f;
	
	Vector2 direction = Vector2.Right;
	Boolean flag = true;

	[Export] public PackedScene wraith;
	[Export] public PackedScene reaper;

	
	private Vector2 velocity = Vector2.Zero;

	public override void _Ready() {
		wraith = (PackedScene)ResourceLoader.Load("res://Scenes/Enemy/Wraith/Wraith.tscn");
		reaper = (PackedScene)ResourceLoader.Load("res://Scenes/Enemy/Reaper/Reaper.tscn");
	}

	public override void _PhysicsProcess(float delta) {
		lifeTime -= .1f;
		if (lifeTime>0) {
			Position += direction.Normalized() * speed * delta;
			Rotation += 0.1f;
			Position += new Vector2(0, gravity)*delta;
		} else {
			QueueFree();
		}
	}

	public void setup(Vector2 dir) {
		direction = dir;
	}

	public void setPosition(Vector2 pos) {
		Position = pos;
	}

	private void _on_Bone_body_entered(object body) {
		if (body is Movement)
			((Movement)body)._Hurt(damage);
		else if (body is IEnemy == false) {
			Random rd = new Random();
			int rand = rd.Next(0,2);
			if (rand == 0) {
				Reaper oponent = (Reaper)reaper.Instance();
				GetParent().AddChild(oponent);
				oponent.GlobalPosition = GlobalPosition - new Vector2(0,80);
			} else {
				Wraith oponent = (Wraith)wraith.Instance();
				GetParent().AddChild(oponent);
				oponent.GlobalPosition = GlobalPosition - new Vector2(0,80);
			}
		}
		QueueFree();
	}
}
