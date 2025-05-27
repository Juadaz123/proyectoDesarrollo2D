using UnityEngine;

[CreateAssetMenu(fileName = "NewBaseCharacterStats", menuName = "Character/CharacterStats")]
public class CharacterStats : ScriptableObject
{
    [Header("Estadisticas Base")]
    [Tooltip("La Cantidad maxima de vida Inicial para el personaje")]
    public float MaxLife = 10f;

    [Tooltip("Porcentaje de reduccion de daño del personaje en porcentajes 0-100")]
    public float Defense = 0f;

    [Tooltip("Porcentaje de evadir un ataque de otra entidad")]
    public float Evasion = 0;
    
}
