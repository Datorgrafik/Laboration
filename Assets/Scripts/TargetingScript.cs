using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetingScript : MonoBehaviour
{
	#region Attributes

	public static Color colorOff;
	public static GameObject selectedTarget = null;
	private EventSystem eventSys;

	#endregion

	#region Methods

	// Update is called once per frame
	void Update()
	{
		//Left mouse click. If object is clicked, target it.
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();
			bool missTarget = true;

			foreach (RaycastHit hit in Physics.RaycastAll(ray))
			{
				if (hit.collider.CompareTag("DataBall"))
				{
					missTarget = false;
					SelectDataBall(hit);
					break;
				}

				else if (hit.collider.CompareTag("DataLine"))
				{
					missTarget = false;
					SelectDataLine(hit);
					break;
				}
			}

            // Deselect target 
			if (missTarget == true && !eventSys.IsPointerOverGameObject() && selectedTarget != null)
			{
				if (selectedTarget.gameObject.CompareTag("DataBall"))
					selectedTarget.GetComponent<Renderer>().material.color = colorOff;
				
				else if (selectedTarget.gameObject.CompareTag("DataLine"))
					selectedTarget.GetComponent<LineRenderer>().material.color = colorOff;

				selectedTarget.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
				selectedTarget = null;
			}
		}
	}

	private void SelectDataBall(RaycastHit hit)
	{
		if (selectedTarget != null)
		{
			selectedTarget.GetComponent<Renderer>().material.color = colorOff;
			selectedTarget.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
		}

		selectedTarget = hit.transform.gameObject;
		colorOff = selectedTarget.GetComponent<Renderer>().material.color;
		selectedTarget.GetComponent<Renderer>().material.color = Color.white;
		selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
	}

	private void SelectDataLine(RaycastHit hit)
	{
		if (selectedTarget != null)
		{
			selectedTarget.GetComponent<LineRenderer>().material.color = colorOff;
			selectedTarget.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
		}

		selectedTarget = hit.transform.gameObject;
		colorOff = selectedTarget.GetComponent<LineRenderer>().material.color;
		selectedTarget.GetComponent<LineRenderer>().material.color = Color.white;
		selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
	}

	#endregion
}
