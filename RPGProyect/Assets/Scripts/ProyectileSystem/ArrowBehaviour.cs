using UnityEngine;
using UnityEngine.Windows;
using UnityEngine.InputSystem;
using System.Collections;

public class ArrowBehaviour : MonoBehaviour
{
    [Header("Parametros Proyectil")]
    [SerializeField] private string targettag;
    private LaucherBehaviour laucher;
    [SerializeField] private float shootforce, waitedTime, lifetime;
    private Rigidbody2D rb2d;
    private CapsuleCollider2D Arrowcollider2D;
    private PlayerController player;
    private Vector2 target;
    private bool DisableArrow;

    private void Awake()
    {
        //comprobadores
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        if (player == null)
        { Debug.LogError($" {gameObject.name} no encuentra al player ni el componente playerCOntroller."); }
        rb2d = GetComponent<Rigidbody2D>();
        if (rb2d == null)
        {
            Debug.LogWarning($" {gameObject.name} no tiene Rigidbpdy2D");
        }
        Arrowcollider2D = GetComponent<CapsuleCollider2D>();
        if (Arrowcollider2D == null)
            Debug.LogWarning($"No se encuentra collider en el objeto {gameObject.name}");
        laucher = GetComponent<LaucherBehaviour>();
    }

    //metodo de prueba
    private void OnEnable()
    {
        StartCoroutine(LaucherCourrutine());
    }
    private void OnDisable()
    {
        StopCoroutine(LaucherCourrutine());
        target = Vector2.zero;
    }

    void LauncherArrow()
    {
        target = player.GetDirectionToMouse();
        rb2d.AddForce(target * shootforce, ForceMode2D.Impulse);
        Debug.Log("coordenadas mouse: " + player.GetDirectionToMouse());
    }

    private IEnumerator LaucherCourrutine()
    {
        yield return new WaitForSeconds(waitedTime);
        LauncherArrow();
    }
    

}
