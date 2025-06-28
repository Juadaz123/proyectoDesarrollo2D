using System; // Importar System para Action
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class ControlTurnos : MonoBehaviour
{
    private bool _isMyTurn = true;

    public bool IsMyTurn {get; private set;}

    // --- NUEVO: Evento para notificar el fin de un turno ---
    public event Action OnTurnEnded; //

    void Start()
    {
        IsMyTurn = false;
    }

    // Llamar esto cada turno desde MovePlayer
    public virtual void UpdateTurn()
    {
        _isMyTurn = true;
        IsMyTurn = _isMyTurn;
        UnityEngine.Debug.Log("Iniciando Turno..." + IsMyTurn + "Valor del turno");

    }

    public void EndTurn()
    {
        _isMyTurn = false;
        IsMyTurn = _isMyTurn;
        UnityEngine.Debug.Log("Turno terminado, esperando... " + IsMyTurn + "Valor del turno");
        OnTurnEnded?.Invoke(); // Disparar el evento cuando el turno termina
    }
}