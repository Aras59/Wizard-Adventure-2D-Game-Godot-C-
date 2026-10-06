using Godot;

/// <summary>Pocisk, który przeciwnik dystansowy może wystrzelić w danym kierunku.</summary>
public interface IProjectile
{
	void setup(Vector2 dir);
}
