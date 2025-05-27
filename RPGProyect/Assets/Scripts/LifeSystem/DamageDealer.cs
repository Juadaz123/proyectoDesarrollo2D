using UnityEngine;

public class DamageDealer : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [Tooltip("La cantidad base de daño que este objeto infligirá.")]
    [SerializeField] private float _damageAmount = 10f;

    [Tooltip("El tipo específico de daño (físico, mágico, etc.) que este objeto inflige. Debe ser un ScriptableObject de tipo DamageType.")]
    [SerializeField] private DamageType _damageType;

    public float DamageAmount => _damageAmount;
    public IDamageType DamageType => _damageType;

    [Header("Comportamiento del Objeto")]
    [Tooltip("Si este objeto debe ser destruido después de colisionar y causar daño.")]
    [SerializeField] private bool _destroyOnHit = true;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (_destroyOnHit)
        {
            Destroy(gameObject);
        }
    }
}
