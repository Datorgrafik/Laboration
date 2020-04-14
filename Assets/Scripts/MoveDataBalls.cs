using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDataBalls : MonoBehaviour
{
	private bool grabItem = false;
	private Vector3 pointOnScreen;
	private Vector3 ObjectPoint;

	// Update is called once per frame
	void Update()
	{
		if (Input.GetMouseButtonDown(0))
			grabItem = false;

		if (TargetingScript.selectedTarget != null && MainMenu.renderMode == 0)
		{
			OnMouseDown();
			OnMouseDrag();
		}
		else if (TargetingScript.selectedTarget != null)
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

	private void OnMouseDrag()
	{
		Vector3 cursorScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, TargetingScript.selectedTarget.transform.position.z);

		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreen) + ObjectPoint;
		TargetingScript.selectedTarget.transform.position = cursorPosition;
	}

	private void OnMouseDown()
	{
		pointOnScreen = Camera.main.WorldToScreenPoint(TargetingScript.selectedTarget.transform.position);
		ObjectPoint = TargetingScript.selectedTarget.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, TargetingScript.selectedTarget.transform.position.z));
	}
}
