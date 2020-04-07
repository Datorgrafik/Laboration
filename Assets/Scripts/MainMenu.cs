using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
	public Dropdown renderModeDropdown;
	private int renderMode = 0;

	private void Start()
	{
		renderModeDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(renderModeDropdown); });
	}

	public void DropdownValueChanged(Dropdown value)
	{
		renderMode = value.value;
	}

	public void ScatterPlot()
	{
		if (renderMode == 0)
		{
			SceneManager.LoadScene("ScatterPlot2D");
		}
		else if (renderMode == 1)
		{
			SceneManager.LoadScene("ScatterPlot");
		}
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

	public void BackToMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void QuitApplication()
	{
		Debug.Log("QUIT...");
		Application.Quit();
	}
}
