using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TargetingScript : MonoBehaviour {
	private Color colorOff;
	public static GameObject selectedTarget = null;
	private EventSystem eventSys;

	// Update is called once per frame
	void Update () {
		//Left mouse click. If object is clicked, target it.
		if (Input.GetMouseButtonDown(0))
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit[] hits = Physics.RaycastAll(ray);
			eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();
			bool missTarget = true;

			foreach (RaycastHit hit in hits)
			{
				if(hit.collider.CompareTag("DataBall"))
				{
					missTarget = false;
					SelectTarget(hit);
					break;
				}
			}
			if (missTarget == true && !eventSys.IsPointerOverGameObject() && selectedTarget != null)
			{
				selectedTarget.GetComponent<Renderer>().material.color = colorOff;
				selectedTarget.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
				selectedTarget = null;
			}
		}
	}

	private void SelectTarget(RaycastHit hit)
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
}
