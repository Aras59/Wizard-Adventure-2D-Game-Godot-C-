using Godot;
using System;

public class RollingStone : Area2D
{
	Vector2 THROW_VELOCITY = new Vector2(800, -400);
	
	[Export] float speed = 700.0f;
	[Export] float lifeTime = 10.0f;
	[Export] float damage = 50.0f;
	
	Vector2 direction = Vector2.Right;
	Boolean flag = true;

	public float rotating = 0.1f;
	private Vector2 velocity = Vector2.Zero;

	public override void _Ready() {}

	public override void _PhysicsProcess(float delta) {
		lifeTime -= .1f;
		if (lifeTime>0) {
			Position += direction.Normalized() * speed * delta;
			Rotation += rotating;
			
			if (lifeTime>0 && lifeTime <1 && flag && direction != Vector2.Left && direction != Vector2.Right) {
				flag = false;
				direction.y = 1;
				lifeTime+=20;
				speed*=0.75f;
			}
		} else
			QueueFree();
	}

	public void setup(Vector2 dir) {
		direction = dir;
	}

	public void setPosition(Vector2 pos) {
		Position = pos;
	}

	private void _on_RollingStone_body_entered(object body) {
		if (body is Movement)
			((Movement)body)._Hurt(damage);
		if (body is IEnemy == false)
			QueueFree();
	}
}
