using UnityEngine;

public interface IDamageType
{
    string Name { get; }
    float CalculateFinalDamage(float baseDamage, float defensePorcentage);
    
}
