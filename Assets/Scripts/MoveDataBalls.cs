using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDataBalls : MonoBehaviour
{
    private bool grabItem = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
            grabItem = false;

        if (TargetingScript.selectedTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (grabItem == true)
                    grabItem = false;
                else
                    grabItem = true;
            }

            if (grabItem == true)
                TargetingScript.selectedTarget.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
        }
    }
}
