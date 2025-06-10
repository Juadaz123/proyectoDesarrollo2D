// EnemyBehaviour2.cs
using UnityEngine;

public class EnemyBehaviour2 : EnemyBehaviourBase
{
    private float _yOffset; // El offset se decide internamente

    public EnemyBehaviour2(Transform playerTransform, EnemyController enemyController) : base(playerTransform, enemyController)
    {
        // Decide aleatoriamente entre +10 o -10 al ser instanciado
        _yOffset = Random.Range(0, 2) == 0 ? 10f : -10f;
        Debug.Log($"EnemyBehaviour2 instanciado con Y-Offset: {_yOffset}");
    }

    private void OnEnable() {   
        _yOffset = Random.Range(0, 2) == 0 ? 10f : -10f;

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

        rb2D.AddForce(directionToTarget * data.shootForce,ForceMode2D.Impulse );
        context.StartCoroutine(CooldownTimer(data.cooldown, rb2D));
          if (_enemyController != null) // Comprobación adicional
        {
            _enemyController.StartCoroutine(CooldownTimer(data.cooldown, rb2D));
        }
    }
}