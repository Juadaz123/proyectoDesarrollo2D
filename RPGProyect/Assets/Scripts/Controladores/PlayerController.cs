using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private MovementData data;
    private Rigidbody2D rb2D;

    private ITurnos turnos;
    private ControlTurnos controlTurnos;

    private void Start()
    {
        rb2D = GetComponent<Rigidbody2D>();
        controlTurnos = new ControlTurnos();
        turnos = new MovePlayer(controlTurnos);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.timeScale > 0f)
        {
            SearchCamera();
        }
    }

    private void SearchCamera()
    {
        Vector2 mouseWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = mouseWorld - rb2D.position;

        turnos.Turno(direction, rb2D, data, this);
    }
}
