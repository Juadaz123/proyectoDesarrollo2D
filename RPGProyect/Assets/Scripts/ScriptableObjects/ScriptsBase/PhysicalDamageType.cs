// PhysicalDamageType.cs
using UnityEngine;

[CreateAssetMenu(fileName = "NewPhysicalDamageType", menuName = "Damage Types/Physical Damage")]
public class PhysicalDamageType : DamageType
{
    [Header("Parámetros de Daño Físico")]
    [Tooltip("Ignora un porcentaje de la defensa del objetivo (0-100).")]
    [SerializeField] private float _armorPenetration = 0f;

    public override float CalculateFinalDamage(float baseDamage, float defensePercentage)
    {
        float effectiveDefense = Mathf.Max(0, defensePercentage - _armorPenetration);
        
        float finalDamage = baseDamage * (1 - (effectiveDefense / 100f));
        return Mathf.Max(0, finalDamage);
    }
}
