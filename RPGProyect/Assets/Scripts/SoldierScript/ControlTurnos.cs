using System.Collections;
using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class ControlTurnos
{
    private bool _ismyTurn;
    private float _cooldownTime;

    public ControlTurnos( float cooldownTime)
    {
        _cooldownTime = cooldownTime;
    }

    //Turnos
    public IEnumerator WaitingTurn()
    {
        _ismyTurn = false;
        yield return new WaitForSeconds(_cooldownTime);
        Debug.Log("Termino tu turno");
    }

    public void ActiveplayerTurn()
    {
        _ismyTurn = true;
    }

    //Comprobador de turno y tiempo
    private void Update() {
        if (_ismyTurn)
        Time.timeScale = 0;
    }
}
