// EnemyBehaviourBase.cs
using System.Collections;
using UnityEngine;

public class EnemyBehaviourBase : ITurnos
{
    protected EnemyController _enemyController;
    protected Transform _playerTransform;

    public EnemyBehaviourBase(Transform playerTransform, EnemyController enemyController)
    {
        _playerTransform = playerTransform;
        _enemyController = enemyController;
    }

    public virtual void Turno(Vector2 direction, Rigidbody2D rb2D, MovementData data, MonoBehaviour context)
    {
        if (_playerTransform == null)
        {
            Debug.LogWarning("Player Transform no asignado en EnemyBehaviourBase.");
            return;
        }

        Vector2 directionToPlayer = (_playerTransform.position - rb2D.transform.position).normalized;

        rb2D.AddForce(directionToPlayer * data.shootForce, ForceMode2D.Impulse);
        Debug.Log("Direccion del jugador" + directionToPlayer + "veolidad " + data.shootForce);
        context.StartCoroutine(CooldownTimer(data.cooldown, rb2D));
        if (rb2D == null)
        {
            Debug.LogError("Error desconocido movimeinto");
        }
    }

    protected IEnumerator CooldownTimer(float cooldown, Rigidbody2D rb2D)
    {
        yield return new WaitForSeconds(cooldown);
        if (rb2D != null)
        {
            rb2D.linearVelocity = Vector2.zero;
            rb2D.angularVelocity = 0f; // También resetea la velocidad angular si es relevante
            Debug.Log("Velocidad del Rigidbody reseteada a cero.");


        }
        if (_enemyController != null)
        {
            _enemyController.OnBehaviorCompleted();
        }
        else
        {
            Debug.LogError("EnemyController es null en CooldownTimer. No se pudo llamar OnBehaviorCompleted.");
        }    }
}