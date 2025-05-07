using UnityEngine;

public class TimeController : MonoBehaviour
{
    public ControlTurnos controlTurnos;
    private float _time;
    
    //ajustar el tiempo
    void Start()
    {
        controlTurnos = new ControlTurnos();
        _time = 1;
    }

    void Update()
    {
       Time.timeScale = _time;

        if (controlTurnos.IsMyTurn == true)
        PlayTime();
        else
        StopTime();

    }

    public void StopTime()
    {
        _time = 0;
        Debug.Log("el timepo a msido ajustado a " + _time);
    }

    public void PlayTime()
    {
        _time = 1;
        // Debug.Log("el timepo a msido ajustado a " + _time);
    }
}
