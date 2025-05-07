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

        rb2D.AddForce(direction.normalized * data.shootForce, ForceMode2D.Impulse);
        controlTurnos.UpdateTurn();

        context.StartCoroutine(CooldownTimer(data.cooldown));
    }

    private IEnumerator CooldownTimer(float cooldown)
    {
        yield return new WaitForSeconds(cooldown);
        controlTurnos.EndTurn();
    }
}
