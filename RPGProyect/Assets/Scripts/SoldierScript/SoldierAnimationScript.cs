using System;
using UnityEngine;

public class SoldierAnimationScript : MonoBehaviour
{
    [SerializeField] Rigidbody2D rbSoldier;
    [SerializeField] int speed;
    [SerializeField] KeyCode attack1,attack2,attack3,damage;
    private Animator animatorSoldier;
    private Vector2 axisInput;
    private bool isDamage = false, isDeath = false;

    [Range(0,5)]public int life = 5;
    

    private void Awake()
    {
        rbSoldier = GetComponent<Rigidbody2D>();
        animatorSoldier = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDeath) return;

        if (isDamage) return;
        


        axisInput.x = Input.GetAxisRaw("Horizontal");
        axisInput.y = Input.GetAxisRaw("Vertical");


        //Animaci�n de da�o recibido
        if (Input.GetKeyDown(damage))
        {
            RecibeDamage();
        }

        //Animaci�n de muerte
        if (life <= 0)
        {
            animatorSoldier.SetBool("Death", true);
        }

        //Animaci�m de ataques
        if (Input.GetKeyDown(attack1))
        {
            animatorSoldier.SetBool("isAttacking", true);
            animatorSoldier.SetInteger("Attack", 1);
        }
        if (Input.GetKeyDown(attack2))
        {
            animatorSoldier.SetBool("isAttacking", true);
            animatorSoldier.SetInteger("Attack", 2);
        }
        if (Input.GetKeyDown(attack3))
        {
            animatorSoldier.SetBool("isAttacking", true);
            animatorSoldier.SetInteger("Attack", 3);
        }
    }

    private void FixedUpdate()
    {
        //Movimiento axis
        if (!isDeath)
        {
            if (!isDamage)
            {
                Movimiento();
            }
        }
    }

    public void EndAttack()
    {
        animatorSoldier.SetBool("isAttacking", false);
    }

    //Activa la animación de daño, detiene el movimiento, resta vida
    private void RecibeDamage()
    {
        isDamage = true;
        axisInput = Vector2.zero; //Limpiar inputs
        rbSoldier.linearVelocity = Vector2.zero;
        animatorSoldier.SetBool("isDamage", true);
        life--;
    }

    //Desactiva la animación de daño
    public void endOfDamage()
    {
        isDamage = false;
        animatorSoldier.SetBool("isDamage", false);
    }

    private void Movimiento()
    {
        rbSoldier.linearVelocity = new Vector2(axisInput.x * speed, axisInput.y * speed);

        //Animaci�n Movimiento
        if (axisInput.x > 0 || axisInput.x < 0 || axisInput.y < 0 || axisInput.y > 0)
        {
            animatorSoldier.SetBool("isWalking", true);
        }
        else { animatorSoldier.SetBool("isWalking", false); }

        //Giro de sentido
        if (axisInput.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (axisInput.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

}
