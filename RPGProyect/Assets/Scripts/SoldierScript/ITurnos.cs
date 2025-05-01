using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public class ITurnos
{
    private Vector2 _direction;
    private Rigidbody2D _rb2D;
    private float _shootForce;

    public ITurnos(Vector2 direction, float shootForce, Rigidbody2D rb2D)
    {
        _direction = direction.normalized;
        _rb2D = rb2D;
        _shootForce = shootForce;
    }

    public virtual void BasicAction()
    {
        if (_rb2D != null)
        {
            _rb2D.AddForce(_direction * _shootForce, ForceMode2D.Impulse);
            Debug.Log("Acción de turno ejecutada (impulso aplicado).");
        }
        else
        {
            Debug.LogWarning("Rigidbody2D no asignado en ITurnos.");
        }
    }
}