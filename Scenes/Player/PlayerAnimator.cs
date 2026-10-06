using Godot;

/// <summary>Wybiera animację gracza (AnimationPlayer) na podstawie ruchu i kierunku.</summary>
public class PlayerAnimator
{
	private readonly AnimationPlayer player;

	public PlayerAnimator(AnimationPlayer player)
	{
		this.player = player;
	}

	private static string Side(bool facingRight)
	{
		return facingRight ? "Right" : "Left";
	}

	public void Walk(bool facingRight)
	{
		player.Play("Walk_" + Side(facingRight));
	}

	public void Idle(bool facingRight)
	{
		player.Play("Idle_" + Side(facingRight));
	}

	public void Airborne(bool facingRight, bool rising)
	{
		player.Play((rising ? "Jump_" : "Falling_") + Side(facingRight));
	}

	public void JumpStart(bool facingRight)
	{
		player.Play("Jump_Start_" + Side(facingRight));
	}
}
