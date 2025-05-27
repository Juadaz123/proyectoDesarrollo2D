using UnityEngine;

public class PlayerLIfeSystem : LifeScript
{
     [Header("Configuración del Jugador")]
    [Tooltip("La etiqueta del GameObject que causará daño al jugador (por ejemplo, 'EnemyBullet', 'Enemy').")]
    [SerializeField] private string _damageDealerTag = "EnemyDamage";

    protected override void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto! Fin del juego.");
        // Aquí puedes añadir la lógica específica de lo que ocurre cuando el jugador muere:
        // Destroy(gameObject);
        // GameManager.Instance.GameOver();
        // SceneManager.LoadScene("GameOverScene");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(_damageDealerTag))
        {
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();

            if (damageDealer != null)
            {
                TakeDamage(damageDealer.DamageAmount, damageDealer.DamageType);
            }
            else
            {
                Debug.LogWarning($"El objeto '{collision.gameObject.name}' con la etiqueta '{_damageDealerTag}' no tiene un componente 'DamageDealer'. No se aplicó daño.");
            }
        }
    }
    
}
