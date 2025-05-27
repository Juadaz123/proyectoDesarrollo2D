using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Scriptable Objects/MovementData")]
public class MovementData : ScriptableObject
{
    [Header("Propiedades de Movimiento/Habilidad")]
    [Tooltip("Fuerza aplicada para el movimiento ")]
    public float shootForce = 1f;
    [Tooltip("Tiempo de recarga (cooldown) de la acción.")]
    public float cooldown = 1f;
    [Tooltip("Fuerza de ataque (si aplica, para cuerpo a cuerpo).")]
    public float attackForce = 0f;

    [Header("Configuración de Daño (si es un ataque)")]
    [Tooltip("Cantidad de daño base que esta acción inflige.")]
    public float damageAmount = 0f; // <-- ¡Añadido!
    [Tooltip("El tipo de daño (Físico, Mágico, etc.) que esta acción inflige.")]
    public DamageType damageType; // <-- ¡Añadido!

    [Tooltip("Icono para representar la acción en la UI.")]
    public Sprite actionicon;

}
