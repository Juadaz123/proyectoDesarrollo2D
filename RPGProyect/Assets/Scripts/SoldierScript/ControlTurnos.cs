using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class ControlTurnos
{
    private bool _isMyTurn = true;

    public bool IsMyTurn => _isMyTurn;



    // Llamar esto cada frame desde MovePlayer
    public virtual void UpdateTurn()
    {
        _isMyTurn = true;
        UnityEngine.Debug.Log("Iniciando Turno..." + IsMyTurn + "Valor del turno");

    }

    public void EndTurn()
    {
        _isMyTurn = false;
        UnityEngine.Debug.Log("Turno terminado, esperando... " + IsMyTurn + "Valor del turno");
    }
}
