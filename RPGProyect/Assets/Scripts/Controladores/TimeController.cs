using UnityEngine;

public class TimeController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D PlayerRb2D;

    private ControlTurnos turnController;
    [SerializeField] private float _time;

    private GameObject menu;

    //ajustar el tiempo
    void Start()
    {
        _time = 0;
        // detectar jugador y su rIgidbody
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("TimeController, no detecta el player");
        }
        PlayerRb2D = player.GetComponent<Rigidbody2D>();
        if (PlayerRb2D == null)
        {
            Debug.LogWarning("NO DETECTA RB2D en TIme controller");
        }

        // Busca la instancia de ControlTurnos en la escena
        turnController = FindFirstObjectByType<ControlTurnos>();
        if (turnController == null)
        {
            Debug.LogError("TimeController: No se encontró el ControlTurnos en la escena. Asegúrate de que esté en un GameObject.");
        }

        menu = GameObject.FindGameObjectWithTag("PanelMenu");
        if (menu == null)
        {
            Debug.LogError("No asignaste la etiqueta PanelMenu a el panel."); 
        }


    }

    void Update()
    {
       Time.timeScale = _time;
        // Controla la escala de tiempo basándose en el turno del jugador
        if (turnController != null && turnController.IsMyTurn)
        {
            PlayTime(); // Es el turno del jugador, tiempo normal
        }
        else
        {
            StopTime(); // No es el turno del jugador, tiempo lento
        }
        if (turnController.enabled == false)
        {
            PlayTime();
        }
        

    }

    public void StopTime()
    {
        //a veces el click del jugador no es detectado correctamente :'v, dicen que debode utilizar el
        // El sistema de eventos pero da flojera aprederlo XD
        _time = 0f;
        menu.SetActive(true);
        // Debug.Log("el timepo a msido ajustado a " + _time);
    }

    public void PlayTime()
    {
        _time = 1f;
        menu.SetActive(false);
        //Debug.Log("el timepo a msido ajustado a " + _time);
    }
}
