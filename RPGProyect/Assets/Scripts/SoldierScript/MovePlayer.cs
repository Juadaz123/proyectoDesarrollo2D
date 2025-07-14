using System.Collections;
using UnityEngine;

public class MovePlayer : ITurnos
{
    private ControlTurnos controlTurnos;

    public MovePlayer(ControlTurnos turnos)
    {
        controlTurnos = turnos;
    }

    public void Turno(Vector2 direction, Rigidbody2D rb2D, MovementData data, MonoBehaviour context)
    {
        if (!controlTurnos.IsMyTurn) return;

        // Cast a PlayerController
        PlayerController playerController = context as PlayerController;
        if (playerController == null)
        {
            Debug.LogError("MovePlayer: El contexto no es un PlayerController.");
            return;
        }

        rb2D.AddForce(direction.normalized * data.shootForce, ForceMode2D.Impulse);
        playerController.StartCoroutine(CooldownTimer(data.cooldown, rb2D, playerController));
    }

    private IEnumerator CooldownTimer(float cooldown, Rigidbody2D rb2D, MonoBehaviour context)
    {
        yield return new WaitForSeconds(cooldown);

        rb2D.linearVelocity = Vector2.zero;

        if (context is PlayerController player)
        {
            player.SetWalkingAnimation(false);
        }

        controlTurnos.EndTurn();
    }
}
