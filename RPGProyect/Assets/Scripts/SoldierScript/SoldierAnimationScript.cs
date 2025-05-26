using System;
using UnityEngine;

public class SoldierAnimationScript : MonoBehaviour
{

    private Animator animatorSoldier;
    private Vector2 axisInput;
    private bool isDamage = false, isDeath = false;

    [Range(0, 5)] public int life = 5;


    private void Awake()
    {
        animatorSoldier = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDeath) return;

        if (isDamage) return;





        // //usar el uso de recibe damage con un oncollison enter2d
        // if (Input.GetKeyDown(damage))
        // {
        //     RecibeDamage();
        // }

        //asignar un evento mediante el collider, para el daño.

        //Animaci�n de muerte
        if (life <= 0)
        {
            animatorSoldier.SetBool("Death", true);
        }

        RotatePlayer();
    }



    public void EndAttack()
    {
        animatorSoldier.SetBool("isAttacking", false);
    }

    private void RecibeDamage()
    {
        isDamage = true;
        animatorSoldier.SetBool("isDamage", true);
        life--; // a revisar si manejar la vida aqui
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

}
