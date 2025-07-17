using UnityEngine;

public class BossLifeScript : LifeScript
{
    [Header("Configuración del Enemigo")]
    [Tooltip("La etiqueta del GameObject que causará daño al enemigo con triggers (ej. 'PlayerAttackCollider').")]
    [SerializeField] private string _damageDealerTag = "PlayerAttack"; // Para el ataque de cuerpo a cuerpo del jugador

    private BossScript bossScript;
    private BossController bossController;

    private void Start()
    {
        bossScript = gameObject.GetComponent<BossScript>();
        if (bossScript == null)
        {
            Debug.LogError($"EnemyLifeSystem: No se detecta en {gameObject.name} el script BossScript.");
        }

        bossController = gameObject.GetComponent<BossController>();
        if (bossController == null)
        {
            Debug.LogError($"EnemyLifeSystem: No se detecta en {gameObject.name} el script BossController.");
        }
    }

    //Muerte basica boss
    protected override void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto!");

        if (bossScript != null)
        {
            bossScript.PlayDeathAnimation();
            //destruir y desaparecer el slime en 4 segundos
            Destroy(gameObject, 4f);
        }

        if (bossController != null)
        {
            bossController.enabled = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag(_damageDealerTag))
        {
            //obtner referencia del damgeDealer
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();

            if (damageDealer != null)
            {
                // Comprobación del booleano YouCanDamgeMe y si el jefe no está muerto
                if (bossScript.YouCanDamgeMe && !IsDeathActive())
                {
                    TakeDamage(damageDealer.DamageAmount, damageDealer.DamageType);
                    Debug.Log("Booleano de daño: " + bossScript.YouCanDamgeMe + ". El jefe recibió daño.");
                    bossScript.BossTakeDamageAnimation(); // El jefe puede recibir daño y cambia a rojo
                }
                else
                {
                    Debug.Log("Booleano de daño: " + bossScript.YouCanDamgeMe + ". El jefe NO recibió daño.");
                    bossScript.BossTakeDamageAnimation(); // El jefe NO puede recibir daño y cambia a negro
                }
            }
            else  
            {
                Debug.LogWarning($"EnemyLifeSystem: El objeto '{collision.gameObject.name}' con la etiqueta '{_damageDealerTag}' no tiene un componente 'DamageDealer'. No se aplicó daño.");
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.gameObject.CompareTag(_damageDealerTag))
        {
            //obtner referencia del damgeDealer
            DamageDealer damageDealer = collision.gameObject.GetComponent<DamageDealer>();

            if (damageDealer != null)
            {
                // Comprobación del booleano YouCanDamgeMe y si el jefe no está muerto
                if (bossScript.YouCanDamgeMe && !IsDeathActive())
                {
                    TakeDamage(damageDealer.DamageAmount, damageDealer.DamageType);
                    Debug.Log("Booleano de daño: " + bossScript.YouCanDamgeMe + ". El jefe recibió daño.");
                    bossScript.BossTakeDamageAnimation(); // El jefe puede recibir daño y cambia a rojo
                }
                else
                {
                    Debug.Log("Booleano de daño: " + bossScript.YouCanDamgeMe + ". El jefe NO recibió daño.");
                    bossScript.BossTakeDamageAnimation(); // El jefe NO puede recibir daño y cambia a negro
                }
            }
            else  
            {
                Debug.LogWarning($"EnemyLifeSystem: El objeto '{collision.gameObject.name}' con la etiqueta '{_damageDealerTag}' no tiene un componente 'DamageDealer'. No se aplicó daño.");
            }
        }
    }

    //boleanos
    public bool IsDeathActive()
    {
        if (bossScript != null)
        {
            return bossScript.IsDeathActive();
        }
        return false;
    }
}