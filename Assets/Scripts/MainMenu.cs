using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
	#region MainMenu ButtonMethods

	public void ScatterPlot()
	{
		SceneManager.LoadScene("ScatterPlot");
	}

	public void ParallelCoordinatePlot()
	{
		SceneManager.LoadScene("ParallelCoordinatePlot");
	}

	public void ScatterPlotMatrix()
	{
		SceneManager.LoadScene("ScatterPlotMatrix");
	}

	public void ValfriTeknik()
	{
		SceneManager.LoadScene("ValfriTeknik");
	}

	public void QuitApplication()
	{
		Debug.Log("QUIT...");
		Application.Quit();
	}

	#endregion
}
