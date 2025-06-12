using UnityEngine;

public class SlimeAnimation : MonoBehaviour
{
    [SerializeField] private GameObject attackColliderObject; 

    
    private Animator animatorSlime;
    
    private bool isDamageActive = false; 
    private bool isDeathActive = false; 

    private void Awake()
    {
        animatorSlime = GetComponent<Animator>();

        if (animatorSlime == null)
        {
            Debug.LogError("SlimeAnimation: No se encontró el componente Animator en este GameObject.");
        }
        
        if (attackColliderObject != null)
        {
            Collider2D collider = attackColliderObject.GetComponent<Collider2D>();
            if (collider != null)
            {
                collider.enabled = false;
            }
            else
            {
                Debug.LogWarning($"SlimeAnimation: El objeto '{attackColliderObject.name}' no tiene un componente Collider2D.");
            }
        }
        else
        {
            Debug.LogWarning("SlimeAnimation: No se ha asignado un objeto de collider de ataque en el Inspector.");
        }
    }

    private void Update()
    {
      if (isDeathActive) return;

        if (isDamageActive) return;
    }

    public void SetWalkingAnimation(bool isWalking)
    {
        if (isDeathActive || isDamageActive) return;
        animatorSlime.SetBool("isWalking", isWalking);
    }

    public void PlayAttackAnimation(int attackIndex)
    {
        if (isDeathActive || isDamageActive) return;
        animatorSlime.SetBool("isAttacking", true);
        animatorSlime.SetInteger("Attack", attackIndex);
    }

    public void ResetAllActionAnimations()
    {
        animatorSlime.SetBool("isAttacking", false);
        animatorSlime.SetInteger("Attack", 0);
        SetWalkingAnimation(false);
    }

    public void EndAttackAnimation()
    {
        animatorSlime.SetBool("isAttacking", false);
        animatorSlime.SetInteger("Attack", 0);
        EndAttackCollider();
    }

    public void PlayDamageAnimation()
    {
        if (isDeathActive) return; 
        isDamageActive = true;
        animatorSlime.SetBool("isHurting", true);
    }

    public void EndDamageAnimation()
    {
        isDamageActive = false;
        animatorSlime.SetBool("isHurting", false);
    }

    public void PlayDeathAnimation()
    {
        isDeathActive = true;
        animatorSlime.SetBool("isDeath", true);
    }

    public void StartAttackCollider()
    {

        // Debug.Log("Mensaje puerda StartAtattackCOllider");
        if (attackColliderObject != null)
        {
            CapsuleCollider2D collider = attackColliderObject.GetComponent<CapsuleCollider2D>();
            if (collider != null)
            {
                collider.enabled = true;
                Debug.Log("Slime Attack Collider Enabled!");
            }
        }
    }

    public void EndAttackCollider()
    {
        if (attackColliderObject != null)
        {
            CapsuleCollider2D collider = attackColliderObject.GetComponent<CapsuleCollider2D>();
            if (collider != null)
            {
                collider.enabled = false;
                Debug.Log("Slime Attack Collider Disabled!");
            }
        }
    }

    public void RotateSlime(Vector2 direction)
    {
        if (direction.x < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }
    }

    public bool IsDamageActive()
    {
        return isDamageActive;
    }

    public bool IsDeathActive()
    {
        return isDeathActive;
    }
}