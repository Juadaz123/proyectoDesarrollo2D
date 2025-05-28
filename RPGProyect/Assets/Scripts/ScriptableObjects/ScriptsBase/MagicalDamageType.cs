// MagicalDamageType.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewMagicalDamageType", menuName = "Damage Types/Magical Damage")]
public class MagicalDamageType : DamageType
{
    [Header("Parámetros de Daño Mágico")]
    [Tooltip("Factor por el cual la defensa es reducida para el daño mágico. Un valor de 2 significa que 100 de defensa actúan como 50.")]
    [SerializeField] private float _defenseEffectivenessFactor = 2f;

    public override float CalculateFinalDamage(float baseDamage, float defensePercentage)
    {
        float effectiveDefense = defensePercentage / _defenseEffectivenessFactor;
        float finalDamage = baseDamage * (1 - (effectiveDefense / 100f));
        return Mathf.Max(0, finalDamage);
    }
}

