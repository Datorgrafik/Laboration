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
		if (Input.GetMouseButtonDown(0) && !DataPlotter.KNNMode && !ScatterPlotMatrix.KNNMode)
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
			}

            // Deselect target 
			if (missTarget == true && !eventSys.IsPointerOverGameObject() && selectedTarget != null)
			{
				selectedTarget.GetComponent<Renderer>().material.color = colorOff;
				selectedTarget.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
				selectedTarget = null;
			}
		}

	}

	private void SelectDataBall(RaycastHit hit)
	{
        //Demark the earlier target
		if (selectedTarget != null)
		{
			selectedTarget.GetComponent<Renderer>().material.color = colorOff;
			selectedTarget.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
		}

        //Mark the target
		selectedTarget = hit.transform.gameObject;
		colorOff = selectedTarget.GetComponent<Renderer>().material.color;
		selectedTarget.GetComponent<Renderer>().material.color = Color.white;
		selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
	}

	#endregion
}
