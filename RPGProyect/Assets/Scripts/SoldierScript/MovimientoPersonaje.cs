using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MovimientoPersonaje : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb2D;

    private void Start() {
        rb2D = GetComponent<Rigidbody2D>();
        
    }

}