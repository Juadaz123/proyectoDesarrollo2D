using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementData data;
    private Rigidbody2D rb2D;
    private bool CanMove = true;

    private ITurnos turnos;
    private ControlTurnos controlTurnos;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        controlTurnos = new ControlTurnos();
        turnos = new MovePlayer(controlTurnos);

        //prueba logica para no duplicar turnos
        CanMove = true;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && CanMove == true)
        {
            StartCoroutine(Activadorinterno());
            
            SearchCamera();
        }
    }

    private void SearchCamera()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorld - rb2D.position;

        turnos.Turno(direction, rb2D, data, this);
    }

    private IEnumerator Activadorinterno()
    {
        CanMove = false;
        yield return new WaitForSeconds(data.cooldown);
        CanMove = true;
    }
}
