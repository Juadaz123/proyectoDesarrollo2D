using UnityEngine;

[CreateAssetMenu(fileName = "MovementData", menuName = "Scriptable Objects/MovementData")]
public class MovementData : ScriptableObject
{
    public float shootForce = 1f;
    public float cooldown = 1f;
    public float attackForce = 0f;

    public float Damage = 0f;

    public Sprite actionicon;

}
