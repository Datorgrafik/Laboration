using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    Renderer Ball;
    Color color;

    // Update is called once per frame
    void Start()
    {
        Ball = GetComponent<Renderer>();
        InvokeRepeating("Blinking",0,0.2f);
        color = Ball.material.color;
    }
    void Blinking()
    {
        if (Ball.material.color == color)
            Ball.material.color = Color.white;
        else
            Ball.material.color = color;
    }
}
