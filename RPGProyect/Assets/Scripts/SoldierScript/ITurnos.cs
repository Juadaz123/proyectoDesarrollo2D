using NUnit.Framework;
using Unity.VisualScripting;
using UnityEngine;

public interface ITurnos
{
        void Turno(Vector2 direction, Rigidbody2D rb2D, MovementData data, MonoBehaviour context);

}
