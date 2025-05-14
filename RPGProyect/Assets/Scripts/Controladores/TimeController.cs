using UnityEngine;

public class TimeController : MonoBehaviour
{
    private GameObject player;
    private Rigidbody2D PlayerRb2D;
    [SerializeField] private float _time;
    
    //ajustar el tiempo
    void Start()
    {
        _time = 1;
        // detectar jugador y su rIgidbody
        player = GameObject.FindGameObjectWithTag("Player");
        if (player == null)
        {
            Debug.LogWarning("TimeController, no detecta el player" );
        }
        PlayerRb2D = player.GetComponent<Rigidbody2D>();   
        if (PlayerRb2D == null)
        {
            Debug.LogWarning("NO DETECTA RB2D en TIme controller");
        }


    }

    void Update()
    {
       Time.timeScale = _time;

       if (PlayerRb2D.linearVelocity == Vector2.zero && !Input.GetMouseButtonDown(0))
       {
        StopTime();
        return;
       }

       PlayTime();
       
        

    }

    public void StopTime()
    {
        //a veces el click del jugador no es detectado correctamente :'v, dicen que debode utilizar el
        // El sistema de eventos pero da flojera aprederlo XD
        _time = 0.2f;
        // Debug.Log("el timepo a msido ajustado a " + _time);
    }

    public void PlayTime()
    {
        _time = 1;
        //Debug.Log("el timepo a msido ajustado a " + _time);
    }
}
