using System.Collections;
using UnityEngine;

public class BossScript : SlimeAnimation
{
    [Header("Estadisticas del Boss")]
    [SerializeField] private float cooldownDaage = 1.2f;
    private SpriteRenderer sprite;

    public bool YouCanDamgeMe;

    protected override void Awake()
    {
        base.Awake();
        sprite = GetComponent<SpriteRenderer>();
        if (sprite == null)
        Debug.LogError("bossScript: No encuentra el compopnente SpriteRedered");
    }

    public void DamageIndicatorColor()
    {
        if (YouCanDamgeMe == true)
        {
            StartCoroutine(ChangeColorICanDamageMe());
        }

    }
    public void BossTakeDamageAnimation()
    {
        if (YouCanDamgeMe == true)
        {
            Debug.LogWarning(YouCanDamgeMe + "valor bool daño");
            // StartCoroutine(ChangeBossColorHit());
            StartCoroutine(ChangeBossColorHit());
            PlayDamageAnimation();
        }
        else
        {
            Debug.LogWarning(YouCanDamgeMe + "valor bool daño");
            StartCoroutine(ChangeBossColorNoHit());
        }

    }

    //courutina para el daño (solo cambio de color visual)
    private IEnumerator ChangeBossColorHit()
    {
        sprite.color = Color.red;
        yield return new WaitForSeconds(cooldownDaage);
        sprite.color = Color.white;
        StopCoroutine(ChangeBossColorHit());
    }
    //Courrutina para avisar que el jefe NO recibe daño
    private IEnumerator ChangeBossColorNoHit()
    {
        sprite.color = Color.black;
        yield return new WaitForSeconds(cooldownDaage);
        sprite.color = Color.white;
        StopCoroutine(ChangeBossColorNoHit());
    }
    private IEnumerator ChangeColorICanDamageMe()
    {
        sprite.color = Color.yellow;
        yield return new WaitForSeconds(cooldownDaage);
        sprite.color = Color.white;
        StopCoroutine(ChangeColorICanDamageMe());
    }
}
