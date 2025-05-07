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

        context.StartCoroutine(CooldownTimer(data.cooldown, rb2D));
    }

    private IEnumerator CooldownTimer(float cooldown, Rigidbody2D rb2D)
    {
        yield return new WaitForSeconds(cooldown);
        rb2D.linearVelocity = Vector2.zero;
        controlTurnos.EndTurn();
    }
}
