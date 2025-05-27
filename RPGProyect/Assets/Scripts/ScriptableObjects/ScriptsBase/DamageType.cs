using UnityEngine;

public abstract class DamageType : ScriptableObject, IDamageType
{
    [SerializeField] private string _name;

    public string Name => _name;

    public abstract float CalculateFinalDamage(float damage, float defensePorcentage);
}
