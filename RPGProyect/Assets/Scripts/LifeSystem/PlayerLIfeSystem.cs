using UnityEngine;
using System.Collections; // Necesario para la corrutina si la usamos para el color

public class PlayerLIfeSystem : LifeScript
{
    [Header("Configuración del Jugador")]
    [Tooltip("La etiqueta del GameObject que causará daño al jugador con colisiones (ej. 'EnemyBullet', 'Enemy').")]
    [SerializeField] private string _collisionDamageDealerTag = "EnemyDamage";
    [Tooltip("La etiqueta del GameObject que causará daño al jugador con triggers (ej. 'EnemyAttackArea', 'TrapTrigger').")]
    [SerializeField] private string _triggerDamageDealerTag = "EnemyAttackTrigger"; // Nueva etiqueta para triggers enemigos

    //referencia a las animaciones
    private SoldierAnimationScript soldierAnimation;
    private SpriteRenderer _spriteRenderer; // Referencia al SpriteRenderer

    private void Start()
    {
        soldierAnimation = gameObject.GetComponent<SoldierAnimationScript>();
        if (soldierAnimation == null)
        {
            Debug.LogError($"No se detecta en {gameObject.name} el script SoldierAnimationScript.");
        }

        _spriteRenderer = GetComponent<SpriteRenderer>(); // Obtener la referencia al SpriteRenderer
        if (_spriteRenderer == null)
        {
            Debug.LogWarning($"PlayerLIfeSystem: No se encontró un SpriteRenderer en {gameObject.name}. El cambio de color de evasión no funcionará.");
        }
    }

    protected override void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto! Fin del juego.");

        if (soldierAnimation != null)
        {
            soldierAnimation.DeathAnimationPLay();
        }
        // Aquí puedes añadir la lógica específica de lo que ocurre cuando el jugador muere:
        // Destroy(gameObject);
        // GameManager.Instance.GameOver();
        // SceneManager.LoadScene("GameOverScene");
    }

    // Sobrescribimos TakeDamage para añadir la lógica del SpriteRenderer
    public override void TakeDamage(float damageAmount, IDamageType damageType)
    {
        isEvationFiscal = false;
        if (Random.value < Evasion)
        {
            Debug.Log($"{gameObject.name} evadió el ataque ({damageType.Name} tipo)!");
            isEvationFiscal = true;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.color = Color.gray; // Cambiar a gris si evade
                StartCoroutine(ResetColorAfterDelay(0.5f)); // Restaura el color después de un breve retraso
            }
            return;
        }

        float finalDamage = damageType.CalculateFinalDamage(damageAmount, Defense);

        CurrentLife -= finalDamage;
        CurrentLife = Mathf.Max(0, CurrentLife);

        Debug.Log($"{gameObject.name} recibió {finalDamage:F2} de daño ({damageType.Name} tipo). Vida restante: {CurrentLife:F2}");

        // animacion de daño del jugador (si no evadió el ataque)
        if (soldierAnimation != null)
        {
            soldierAnimation.RecibeDamage();
        }
        UIManager.Instance.ActualizarVidaJugador(CurrentLife, MaxLife); // Modifiacion de los corazones del UI

        if (CurrentLife <= 0)
        {
            Die();
        }

        // Si no evade, asegúrate de que el color sea blanco (normal)
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.white;
        }
    }

    // Corrutina para restaurar el color del sprite
    private IEnumerator ResetColorAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = Color.white; // Vuelve a blanco
        }
    }

    // Para colisiones "físicas" (non-trigger), como balas, enemigos físicos, etc.
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(_collisionDamageDealerTag))
        {
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();

            if (damageDealer != null)
            {
                TakeDamage(damageDealer.DamageAmount, damageDealer.DamageType);
                // La lógica de animación de daño se movió a TakeDamage para ser consistente con la evasión.
            }
            else
            {
                Debug.LogWarning($"El objeto '{collision.gameObject.name}' con la etiqueta '{_collisionDamageDealerTag}' no tiene un componente 'DamageDealer'. No se aplicó daño.");
            }
        }
    }

    // Para colisiones de tipo Trigger, como áreas de ataque de enemigos, trampas de área, etc.
    private void OnTriggerEnter2D(Collider2D other) // Nota: el parámetro es Collider2D, no Collision2D
    {
        // Esta es la detección para que el jugador reciba daño de un Trigger (ej. ataque de enemigo, trampa)
        if (other.gameObject.CompareTag(_triggerDamageDealerTag))
        {
            DamageDealer damageDealer = other.gameObject.GetComponent<DamageDealer>();

            if (damageDealer != null)
            {
                TakeDamage(damageDealer.DamageAmount, damageDealer.DamageType);
                // La lógica de animación de daño se movió a TakeDamage para ser consistente con la evasión.
            }
            else
            {
                Debug.LogWarning($"El objeto '{other.gameObject.name}' con la etiqueta '{_triggerDamageDealerTag}' no tiene un componente 'DamageDealer'. No se aplicó daño.");
            }
        }
    }
}