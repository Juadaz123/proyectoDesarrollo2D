using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [Tooltip("La cantidad base de daño que este objeto infligirá. Será sobrescrita por el MovementData si se usa SetDamageData.")]
    [SerializeField] private float _damageAmount = 0f; // Inicializado a 0, será llenado por MovementData
    [Tooltip("El tipo específico de daño. Será sobrescrita por el MovementData si se usa SetDamageData.")]
    [SerializeField] private DamageType _damageType; // Inicializado a null, será llenado por MovementData

    public float DamageAmount => _damageAmount;
    public IDamageType DamageType => _damageType;

    [Header("Comportamiento del Objeto")]
    [Tooltip("Si este objeto debe ser destruido después de colisionar y causar daño.")]
    [SerializeField] private bool _destroyOnHit = true;

    // Ya no hay _collider2D ni lógica de Enable/DisableCollider aquí.

    /// <summary>
    /// Establece la cantidad y el tipo de daño basándose en un ScriptableObject MovementData.
    /// Este método será llamado por el script que gestione el ataque (ej. PlayerController).
    /// </summary>
    /// <param name="data">El ScriptableObject MovementData que contiene la información de daño.</param>
    public void SetDamageData(MovementData data)
    {
        if (data != null)
        {
            _damageAmount = data.damageAmount;
            _damageType = data.damageType;
            // Puedes dejar este Debug.Log si te ayuda a ver los valores configurados
            Debug.Log($"DamageDealer en {gameObject.name} configurado con: Daño={_damageAmount}, Tipo={_damageType?.name}");
        }
        else
        {
            Debug.LogWarning("DamageDealer: Intentando establecer datos de daño con MovementData nulo. Daño y tipo de daño no serán configurados.");
            _damageAmount = 0f; // Asegurarse de que el daño sea 0 si los datos son nulos.
            _damageType = null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_destroyOnHit)
        {
            // La lógica original de destrucción se mantiene.
            // Si no quieres que tu collider de ataque de jugador se destruya,
            // asegúrate de que _destroyOnHit esté en false en el Inspector para ese DamageDealer.
            Debug.Log($"DamageDealer: Destruyendo {gameObject.name} después de colisión con {collision.gameObject.name}.");
            Destroy(gameObject);
        }
    }
}