using UnityEngine;

public class LifeScript : MonoBehaviour, ILifeSystem
{
    [Header("Estadísticas del Personaje")]
    [Tooltip("Referencia al ScriptableObject que define las estadísticas base de este personaje.")]
    [SerializeField] protected CharacterStats _baseStats;

    // Propiedades de la interfaz ILifeSystem
    // CurrentLife ahora tiene un setter protegido para que las clases derivadas puedan modificarlo internamente
    public float CurrentLife { get; protected set; }
    public float MaxLife => _baseStats != null ? _baseStats.MaxLife : 0f;
    public float Defense => _baseStats != null ? _baseStats.Defense : 0f;
    public float Evasion => _baseStats != null ? _baseStats.Evasion : 0f;

    public bool isEvationFiscal = false;

    protected virtual void Awake()
    {
        if (_baseStats != null)
        {
            CurrentLife = _baseStats.MaxLife;
        }
        else
        {
            Debug.LogWarning($"{gameObject.name}: No se asignó CharacterStats al LifeScript. Inicializando vida a 0.");
            CurrentLife = 0;
        }
    }

    public virtual void TakeDamage(float damageAmount, IDamageType damageType)
    {
        isEvationFiscal = false;
        if (Random.value < Evasion)
        {
            Debug.Log($"{gameObject.name} evadió el ataque ({damageType.Name} tipo)!");
            isEvationFiscal =true;
            return;
        }

        float finalDamage = damageType.CalculateFinalDamage(damageAmount, Defense);

        CurrentLife -= finalDamage;
        CurrentLife = Mathf.Max(0, CurrentLife);

        Debug.Log($"{gameObject.name} recibió {finalDamage:F2} de daño ({damageType.Name} tipo). Vida restante: {CurrentLife:F2}");

        if (CurrentLife <= 0)
        {
            Die();
        }
    }

    public virtual void Heal(float healAmount)
    {
        CurrentLife += healAmount;
        CurrentLife = Mathf.Min(MaxLife, CurrentLife);
        Debug.Log($"{gameObject.name} se curó {healAmount:F2}. Vida actual: {CurrentLife:F2}");
    }

    public bool isAlive()
    {
        return CurrentLife > 0;
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} ha muerto.");
    }
}
