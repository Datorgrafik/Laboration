using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

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
		if (Input.GetMouseButtonDown(0) && !KNN.KNNMode)
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


		#region Temporary Fix for PCP
		// Temporary try to fix for PCP (will not look like this forever)
		if (KNN.KNNMode && Input.GetMouseButtonDown(0) && SceneManager.GetActiveScene().name == "ParallelCoordinatePlot")
		{
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();
			bool missTarget = true;

			foreach (RaycastHit hit in Physics.RaycastAll(ray))
			{
				if (hit.collider.CompareTag("DataBall"))
				{
						// The latest added datapoint should always be the last one in the list, so this should work
						if (hit.collider.gameObject.GetComponent<StoreIndexInDataBall>().Index.ToString()
							.Equals((ParallelCoordinatePlotter.ThisInstans.pointList.Count - 1).ToString()))
						{
							missTarget = false;
							SelectDataBall(hit);
							break;
						}
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

        //Allowing all the kNN targets to be moved in ScatterPlotMatrix
        if(KNN.KNNMode && Input.GetMouseButtonDown(0) && SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();

            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                if (hit.collider.CompareTag("DataBall") && hit.transform.gameObject.GetComponent<StoreIndexInDataBall>().Index == selectedTarget.GetComponent<StoreIndexInDataBall>().Index)
                {
                    SelectDataBall(hit);
                    break;
                }
            }
        }
		#endregion

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
