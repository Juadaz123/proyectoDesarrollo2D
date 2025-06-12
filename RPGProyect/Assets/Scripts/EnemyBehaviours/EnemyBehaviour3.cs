// EnemyBehaviourSineFollow.cs
using System.Collections;
using UnityEngine;

//en proceso ns si implemetarlo aun
public class EnemyBehaviour3 : EnemyBehaviourBase
{
    private float _timeElapsed = 0f;
    private float _sineAmplitude;
    private float _sineFrequency; // Frecuencia se decide internamente

    public EnemyBehaviour3(Transform playerTransform, EnemyController enemyController) : base(playerTransform, enemyController)
    {
        // Valores fijos o aleatorios internos para amplitud y frecuencia del seno
        _sineAmplitude = 5f; // Ejemplo de valor fijo
        _sineFrequency = 2f; // Ejemplo de valor fijo
        // Si quisieras que fueran aleatorios, podrías usar:
        // _sineAmplitude = Random.Range(3f, 7f);
        // _sineFrequency = Random.Range(1f, 3f);
        Debug.Log($"EnemyBehaviourSineFollow instanciado con Amplitud: {_sineAmplitude}, Frecuencia: {_sineFrequency}");
    }

    public override void Turno(Vector2 direction, Rigidbody2D rb2D, MovementData data, MonoBehaviour context)
    {
        if (_playerTransform == null)
        {
            Debug.LogWarning("Player Transform no asignado en EnemyBehaviourSineFollow.");
            return;
        }

        _timeElapsed += Time.fixedDeltaTime;

        Vector2 baseDirectionToPlayer = (_playerTransform.position - rb2D.transform.position);

        Vector2 perpendicularDirection = new Vector2(-baseDirectionToPlayer.y, baseDirectionToPlayer.x).normalized;
        float sineOffset = Mathf.Sin(_timeElapsed * _sineFrequency) * _sineAmplitude;
        Vector2 finalDirection = (baseDirectionToPlayer + perpendicularDirection * sineOffset).normalized;

        rb2D.AddForce(finalDirection * data.shootForce, ForceMode2D.Impulse);
        if (_enemyController != null) // Comprobación adicional
        {
            _enemyController.StartCoroutine(CooldownTimer(data.cooldown, rb2D));
        }
    }
}