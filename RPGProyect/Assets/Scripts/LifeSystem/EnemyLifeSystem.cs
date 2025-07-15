using System.Collections;
using UnityEngine;
using UnityEngine.UI;
public class EnemyLifeSystem : LifeScript
{
    [Header("Configuración del Enemigo")]
    [Tooltip("La etiqueta del GameObject que causará daño al enemigo con colisiones (ej. 'PlayerBullet').")]
    [SerializeField] private string _collisionDamageDealerTag = "PlayerDamage"; // Para balas, etc.
    [Tooltip("La etiqueta del GameObject que causará daño al enemigo con triggers (ej. 'PlayerAttackCollider').")]
    [SerializeField] private string _triggerDamageDealerTag = "PlayerAttackCollider"; // Para el ataque de cuerpo a cuerpo del jugador

    private SlimeAnimation slimeAnimation;
    private EnemyController enemyController;

    private void Start()
    {
        slimeAnimation = gameObject.GetComponent<SlimeAnimation>();
        if (slimeAnimation == null)
        {
            Debug.LogError($"EnemyLifeSystem: No se detecta en {gameObject.name} el script SlimeAnimation.");
        }

        enemyController = gameObject.GetComponent<EnemyController>();
        if (enemyController == null)
        {
            Debug.LogError($"EnemyLifeSystem: No se detecta en {gameObject.name} el script EnemyController.");
        }
    }

    protected override void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto!");

        if (slimeAnimation != null)
        {
            slimeAnimation.PlayDeathAnimation();
            //destruir y desaparecer el slime en 4 segundos
            //Destroy(gameObject, 4f);
        }

        if (enemyController != null)
        {
            enemyController.enabled = false;
        }

        StartCoroutine(FadeAndDestroy());
    }

    // Para colisiones "físicas" (non-trigger), como balas del jugador
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(_collisionDamageDealerTag))
        {
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();

            if (damageDealer != null)
            {
                TakeDamage(damageDealer.DamageAmount, damageDealer.DamageType);

                if (isEvationFiscal == false && !IsDeathActive())
                {
                    if (slimeAnimation != null)
                    {
                        slimeAnimation.PlayDamageAnimation();
                    }
                }
            }
            else
            {
                Debug.LogWarning($"EnemyLifeSystem: El objeto '{collision.gameObject.name}' con la etiqueta '{_collisionDamageDealerTag}' no tiene un componente 'DamageDealer'. No se aplicó daño.");
            }
        }
    }

    // Para colisiones de tipo Trigger, como el ataque de cuerpo a cuerpo del jugador o trampas de área.
    private void OnTriggerEnter2D(Collider2D other) // Nota: el parámetro es Collider2D, no Collision2D
    {
        if (other.gameObject.CompareTag(_triggerDamageDealerTag))
        {
            DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();

            if (damageDealer != null)
            {
                TakeDamage(damageDealer.DamageAmount, damageDealer.DamageType);

                // Aquí podrías añadir la lógica para el knockback si quieres que el enemigo retroceda
                // Ejemplo de knockback:
                Rigidbody2D enemyRb = GetComponent<Rigidbody2D>();
                 if (enemyRb != null && damageDealer.DamageAmount > 0) // Solo si hay daño para aplicar knockback
                {
                   Vector2 knockbackDirection = (transform.position - other.transform.position).normalized;
                    float knockbackForce = 3f; // Ajusta esta fuerza según sea necesario
                    enemyRb.AddForce(knockbackDirection * knockbackForce, ForceMode2D.Impulse);
                }

                if (isEvationFiscal == false && !IsDeathActive())
                {
                    if (slimeAnimation != null)
                    {
                        slimeAnimation.PlayDamageAnimation();
                    }
                }
            }
            else
            {
                Debug.LogWarning($"EnemyLifeSystem: El objeto '{other.gameObject.name}' con la etiqueta '{_triggerDamageDealerTag}' no tiene un componente 'DamageDealer'. No se aplicó daño.");
            }
        }
    }

    public bool IsDead()
    {
        return CurrentLife <= 0;
    }
    
    public bool IsDeathActive()
    {
        if (slimeAnimation != null)
        {
            return slimeAnimation.IsDeathActive();
        }
        return false;
    }


    private IEnumerator FadeAndDestroy()
    {
        float duration = 2f;
        float elapsed = 0f;

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        Color originalColor = sprite.color;

        // Obtenemos el CanvasGroup del hijo (por ejemplo, para la barra de vida del enemigo)
        CanvasGroup canvasGroup = GetComponentInChildren<CanvasGroup>();

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);

            if (sprite != null)
            {
                sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, alpha);
            }

            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        if (sprite != null)
        {
            sprite.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0f);
        }

        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
        }

        Destroy(gameObject);
    }
}