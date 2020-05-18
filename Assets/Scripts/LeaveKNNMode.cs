using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaveKNNMode : MonoBehaviour
{
	public Button LeaveKNN;
	public GameObject KNNWindow;
	public Button NewData;

	// Start is called before the first frame update
	void Start()
	{
		LeaveKNN.onClick.AddListener(Leave);
	}

	public void Leave()
	{
		KNNWindow.SetActive(false);
		KNN.kPoints.Clear();
		KNN.KNNMode = false;

		if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
		{
            NewData.interactable = true;
            ScatterPlotMatrix.ThisInstans.teleportCamera = false;
            ScatterPlotMatrix.ThisInstans.PlottData();     
		}
	
		else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
		{
            NewData.interactable = true;
            ScatterplotDimensions.ThisInstans.teleportCamera = false;
            ScatterplotDimensions.ThisInstans.PlottData();
		}

		else if (SceneManager.GetActiveScene().name == "ParallelCoordinatePlot")
			ParallelCoordinatePlotter.ThisInstans.ReorderColumns();

		else
		{
			DataPlotter.ThisInstans.PlottData();
            DataPlotter.ThisInstans.teleportCamera = false;
			NewData.interactable = true;
			DataPlotter.ThisInstans.teleportCamera = false;
		}

	}
}
	