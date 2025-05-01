using UnityEngine;

public class MovePlayer : MonoBehaviour
{
    public float cooldown = 1.5f;
    public float shootForce = 10f;

    private Rigidbody2D _rb;
    private Camera _mainCamera;

    private ControlTurnos _controlTurnos;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _mainCamera = Camera.main;
        _rb = GetComponent<Rigidbody2D>();

        _controlTurnos = new ControlTurnos(cooldown); // se instancia aquí
    }

    private void Update()
    {
        _controlTurnos.UpdateTurn(Time.unscaledDeltaTime); // actualiza el turno con tiempo real

        // Pausar tiempo si no es turno del jugador
        Time.timeScale = _controlTurnos.IsMyTurn ? 1f : 0f;

        if (_controlTurnos.IsMyTurn && Input.GetMouseButtonDown(0))
        {
            MoveTowardMouse();
            _controlTurnos.EndTurn();
        }
    }

    private void MoveTowardMouse()
    {
        Vector3 mouseWorldPos = _mainCamera.ScreenToWorldPoint(Input.mousePosition);
        Vector2 direction = (mouseWorldPos - transform.position).normalized;
        _rb.AddForce(direction * shootForce, ForceMode2D.Impulse);
    }
}
