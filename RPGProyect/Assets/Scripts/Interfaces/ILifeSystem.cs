using UnityEngine;

public interface ILifeSystem
{
    float CurrentLife { get; }
    float MaxLife { get; }
    float Defense { get; }
    float Evasion { get; }

    void TakeDamage(float damageAmount, IDamageType damageType);
    void Heal(float healAmount);
    bool isAlive();
}
