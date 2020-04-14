﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDataBalls : MonoBehaviour
{
	private bool grabItem = false;
	private Vector3 pointOnScreen;
	private Vector3 ObjectPoint;
    private Vector3 mousePosition;

    // Update is called once per frame
    void Update()
	{
		if (Input.GetMouseButtonDown(0))
			grabItem = false;

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (grabItem == true)
                grabItem = false;
            else
                grabItem = true;
        }


        //Följer muspekaren i 2D nu, men behöver koda om så att man inte behöver trycka på E för att plocka upp le databall.
        if (grabItem == true)
        {
            if (TargetingScript.selectedTarget != null && MainMenu.renderMode == 0)
            {
                mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, TargetingScript.selectedTarget.transform.position.z) * -1);
                TargetingScript.selectedTarget.transform.position = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
            }
            else if (TargetingScript.selectedTarget != null)
            {
                TargetingScript.selectedTarget.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
            }
        }
	}
}
