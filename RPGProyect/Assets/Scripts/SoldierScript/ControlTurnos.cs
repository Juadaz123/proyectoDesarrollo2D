using UnityEditor;
using UnityEngine;


public class ControlTurnos
{
    private bool _isMyTurn;
    private float _cooldownTime;
    private float _timer;

    public bool IsMyTurn => _isMyTurn;

    public ControlTurnos(float cooldownTime)
    {
        _cooldownTime = cooldownTime;
        _isMyTurn = true;
    }

    // Llamar esto cada frame desde MovePlayer
    public void UpdateTurn(float deltaTime)
    {
        if (!_isMyTurn)
        {
            _timer -= deltaTime;
            if (_timer <= 0f)
            {
                _isMyTurn = true;
                _timer = 0;
                UnityEngine.Debug.Log("¡Turno reactivado!");
            }
        }
    }

    public void EndTurn()
    {
        _isMyTurn = false;
        _timer = _cooldownTime;
        UnityEngine.Debug.Log("Turno terminado, esperando...");
    }
}
