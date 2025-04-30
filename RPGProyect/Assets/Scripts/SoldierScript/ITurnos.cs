using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class ITurnos: ControlTurnos
{
    private Vector2 _direction;
    private Rigidbody2D _rb2D;
    private float _shootForce;

    public ControlTurnos controlTurnos;

    //necesita que le pases a la funcion tiempo de espera, direcction
    public ITurnos( float Cooldown, Vector2 direction, float ShootForce, Rigidbody2D rb2D )
     : base( Cooldown)
    {
        _direction = direction;
        _rb2D = rb2D;
        _shootForce = ShootForce;
    }

    public virtual void BasicAction()
    {
        controlTurnos.WaitingTurn(); //utilia el tiempo de espera que se le pasa a Iturnos
        _rb2D.AddForce(_shootForce * _direction * Time.deltaTime, ForceMode2D.Impulse);
        controlTurnos.ActiveplayerTurn();
    }


}
