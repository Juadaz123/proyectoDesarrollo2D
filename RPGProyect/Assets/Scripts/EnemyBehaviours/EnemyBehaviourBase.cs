// EnemyBehaviourBase.cs - CORREGIDO
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
        // Debug.Log("Direccion del jugador: " + directionToPlayer + " velocidad: " + data.shootForce);
        
        if (rb2D == null)
        {
            Debug.LogError("Error desconocido movimiento");
            return;
        }

        // SOLUCION 2: Solo iniciar la corrutina una vez desde el EnemyController
        if (_enemyController != null)
        {
            _enemyController.StartCoroutine(CooldownTimer(data.cooldown, rb2D));
        }
        else
        {
            Debug.LogError("EnemyController es null. No se puede iniciar CooldownTimer.");
        }
    }

    protected IEnumerator CooldownTimer(float cooldown, Rigidbody2D rb2D)
    {
        yield return new WaitForSeconds(cooldown);
        
        if (rb2D != null)
        {
            // SOLUCION 3: Reducir gradualmente la velocidad en lugar de resetearla bruscamente
            rb2D.linearVelocity = Vector2.zero;// Reduce a 10% de la velocidad actual
            // rb2D.angularVelocity = Vector2.zero;
            // Debug.Log("Velocidad del Rigidbody reducida.");
        }
        
        if (_enemyController != null)
        {
            _enemyController.OnBehaviorCompleted();
        }
        else
        {
            Debug.LogError("EnemyController es null en CooldownTimer. No se pudo llamar OnBehaviorCompleted.");
        }
    }
}