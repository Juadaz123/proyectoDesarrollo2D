using System;
using Unity.VisualScripting;
using UnityEngine;

public class SoldierAnimationScript : MonoBehaviour
{

    private Animator animatorSoldier;
    private Vector2 axisInput;
    private bool isDamage = false, isDeath = false;



    private GameObject AttackCollider;
    [SerializeField] private string playerAttackCollider;
    [Tooltip("nombre del collider del objeto hijo que maneja el ataque del jugador")]



    private void Awake()
    {
        animatorSoldier = GetComponent<Animator>();

        AttackCollider = GameObject.Find(playerAttackCollider);
    }

    private void Start() {
        if (AttackCollider == null)
        {
            Debug.LogError($"No se a encomtrado ningun objeto con el nombre {playerAttackCollider}");

            AttackCollider.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
        }

    }

    private void Update()
    {
        if (isDeath) return;

        if (isDamage) return;


        RotatePlayer();
    }


    //eventos de ataque
    public void StartAttack()
    {
         AttackCollider.gameObject.GetComponent<CapsuleCollider2D>().enabled = true;
    }


    public void EndAttack()
    {
        animatorSoldier.SetBool("isAttacking", false);
        AttackCollider.gameObject.GetComponent<CapsuleCollider2D>().enabled = false;
    }

    //eventos de daño animaciones

    public void RecibeDamage()
    {
        isDamage = true;
        animatorSoldier.SetBool("isDamage", true);
    }

    public void endOfDamage()
    {
        isDamage = false;
        animatorSoldier.SetBool("isDamage", false);
    }


    private void RotatePlayer()
    {
        if (axisInput.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (axisInput.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    //eventos para llamar animaciones
    public void PlayAttackAnimation(bool isAttacking, int indexAttack)
    {
        animatorSoldier.SetBool("isAttacking", isAttacking);
        if (isAttacking)
        {
            animatorSoldier.SetInteger("Attack", indexAttack);
        }
    }

    public void PlayAttack2Animation(bool isAttacking)
    {
        animatorSoldier.SetBool("isAttacking", isAttacking);
        if (isAttacking)
        {
            animatorSoldier.SetInteger("Attack", 2);
        }
    }


    public void PlayAttack3Animation(bool isAttacking)
    {
        animatorSoldier.SetBool("isAttacking", isAttacking);
        if (isAttacking)
        {
            animatorSoldier.SetInteger("Attack", 3);
        }
    }

    public void PlayHealAnimation(bool isHealing)
    {
        if (isHealing)
        {
            //por si acaso desactivar animacion de ataque
            animatorSoldier.SetBool("isAttacking", false);

            animatorSoldier.SetBool("isWalking", false);
        }
    }

    public void SetWalkingAnimation(bool isWalking)
    {
        animatorSoldier.SetBool("isWalking", isWalking);
    }
    
    public void ResetAllActionAnimations()
    {
        animatorSoldier.SetBool("isAttacking", false);
        animatorSoldier.SetBool("isHealing", false);
        animatorSoldier.SetInteger("Attack", 0); // Resetea el parámetro Attack a un valor "neutro"
    }

    //referencia a la animacion de muerte
    public void DeathAnimationPLay()
    {
        animatorSoldier.SetBool("Death", true);
    }

}
