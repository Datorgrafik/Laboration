using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blink : MonoBehaviour
{
    Renderer Ball;

    // Update is called once per frame
    void Start()
    {
        Ball = GetComponent<Renderer>();
        InvokeRepeating("Blinking",0,0.2f);
    }
    void Blinking()
    {
        if (Ball.enabled == true)
        {
            Ball.enabled = false;
        }
        else
            Ball.enabled = true;
    }
}
