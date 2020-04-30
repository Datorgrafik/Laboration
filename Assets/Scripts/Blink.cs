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
    }
    void Update()
    {
        Ball.enabled = false;
        Ball.enabled = true;
    }
}
