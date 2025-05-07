using UnityEngine;

public class SlimeAnimation : MonoBehaviour
{
    [SerializeField] Rigidbody2D rbSlime;
    [SerializeField] int speed;
    [SerializeField] KeyCode attack1, attack2, damage;
    private Animator animatorSlime;
    private Vector2 axisInput;
    private bool isDamage = false, isDeath = false;

    [Range(0, 5)] public int life = 5;


    private void Awake()
    {
        rbSlime = GetComponent<Rigidbody2D>();
        animatorSlime = GetComponent<Animator>();
    }

    private void Update()
    {
        if (isDeath) return;

        if (isDamage) return;



        axisInput.x = Input.GetAxisRaw("Horizontal");
        axisInput.y = Input.GetAxisRaw("Vertical");


        //Animación de daño recibido
        if (Input.GetKeyDown(damage))
        {
            RecibeDamage();
        }

        //Animación de muerte
        if (life <= 0)
        {
            animatorSlime.SetBool("isDeath", true);
        }

        //Animacióm de ataques
        if (Input.GetKeyDown(attack1))
        {
            animatorSlime.SetBool("isAttacking", true);
            animatorSlime.SetInteger("Attack", 1);
        }
        if (Input.GetKeyDown(attack2))
        {
            animatorSlime.SetBool("isAttacking", true);
            animatorSlime.SetInteger("Attack", 2);
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
        animatorSlime.SetBool("isAttacking", false);
    }

    //Activa la animación de daño, detiene el movimiento, resta vida
    private void RecibeDamage()
    {
        isDamage = true;
        axisInput = Vector2.zero; //Limpiar inputs
        rbSlime.linearVelocity = Vector2.zero;
        animatorSlime.SetBool("isHurting", true);
        life--;
    }

    //Desactiva la animación de daño
    public void endOfDamage()
    {
        isDamage = false;
        animatorSlime.SetBool("isHurting", false);
    }

    private void Movimiento()
    {
        rbSlime.linearVelocity = new Vector2(axisInput.x * speed, axisInput.y * speed);

        //Animación Movimiento
        if (axisInput.x > 0 || axisInput.x < 0 || axisInput.y < 0 || axisInput.y > 0)
        {
            animatorSlime.SetBool("isWalking", true);
        }
        else { animatorSlime.SetBool("isWalking", false); }

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
