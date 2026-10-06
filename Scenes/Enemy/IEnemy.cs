using Godot;
using System;

/// <summary>
/// Znacznik "to jest przeciwnik, który przyjmuje obrażenia".
/// Pociski (FireBall, Bone, Rock...) sprawdzają `body is IEnemy`, żeby wiedzieć,
/// czy trafiły we wroga (wtedy wołają _Hurt), czy w gracza / ścianę.
/// </summary>
interface IEnemy
{
	void _Dead();
	void _Hurt(float damage);
}
