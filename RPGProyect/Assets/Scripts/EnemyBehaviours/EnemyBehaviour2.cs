// EnemyBehaviour2.cs - CORREGIDO
using UnityEngine;

public class EnemyBehaviour2 : EnemyBehaviourBase
{
    private float _yOffset;

    public EnemyBehaviour2(Transform playerTransform, EnemyController enemyController) : base(playerTransform, enemyController)
    {
        // Decide aleatoriamente entre +10 o -10 al ser instanciado
        _yOffset = Random.Range(0, 2) == 0 ? 7f : -7f;
        // Debug.Log($"EnemyBehaviour2 instanciado con Y-Offset: {_yOffset}");
    }

    public override void Turno(Vector2 direction, Rigidbody2D rb2D, MovementData data, MonoBehaviour context)
    {
        if (_playerTransform == null)
        {
            Debug.LogWarning("Player Transform no asignado en EnemyBehaviour2.");
            return;
        }

        Vector2 targetPosition = new Vector2(_playerTransform.position.x, _playerTransform.position.y + _yOffset);
        Vector2 directionToTarget = (targetPosition - (Vector2)rb2D.transform.position).normalized;

        rb2D.AddForce(directionToTarget * data.shootForce, ForceMode2D.Impulse);
        
        // SOLUCION 4: Eliminar corrutina duplicada - solo llamar a la clase base
        if (_enemyController != null)
        {
            _enemyController.StartCoroutine(CooldownTimer(data.cooldown, rb2D));
        }
    }
}