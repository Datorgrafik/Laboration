using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenu : MonoBehaviour
{
	#region Attributes

	public Dropdown renderModeDropdown;
	public static int renderMode = 0;
	public string filePath;
	public TMP_Text file;
	private static string fileText = "";
	public static string fileData;
	private readonly string errorMsg = "Please select a .csv file to plot the data...";
	public Sprite TwoDImage;
	public Sprite ThreeDImage;

	#endregion

	#region Methods

	private void Start()
	{
		SetCorrectScatterPlotImage();

		renderModeDropdown.value = renderMode;
		renderModeDropdown.onValueChanged.AddListener(delegate { DropdownValueChanged(renderModeDropdown); });
		file.text = fileText;
	}

	public void DropdownValueChanged(Dropdown value)
	{
		renderMode = value.value;

		SetCorrectScatterPlotImage();
	}

	private void SetCorrectScatterPlotImage()
	{
		if (renderMode == 0)
			GameObject.FindGameObjectWithTag("ScatterPlotButton").GetComponent<Image>().sprite = TwoDImage;
		else if (renderMode == 1)
			GameObject.FindGameObjectWithTag("ScatterPlotButton").GetComponent<Image>().sprite = ThreeDImage;
	}

	public void OpenFileExplorer()
	{
		filePath = EditorUtility.OpenFilePanel("Overwrite with dataset", "", "csv");
		fileText = filePath.Substring(filePath.LastIndexOf('/') + 1);
		file.text = fileText;
		fileData = System.IO.File.ReadAllText(filePath);
	}

	public void ScatterPlot()
	{
		if (fileData == null)
		{
			file.text = errorMsg;
			return;
		}

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
		if (fileData == null)
		{
			file.text = errorMsg;
			return;
		}
		renderMode = 0;
		SceneManager.LoadScene("ParallelCoordinatePlot");
	}

	public void ScatterPlotMatrix()
	{
		if (fileData == null)
		{
			file.text = errorMsg;
			return;
		}
		renderMode = 0;
		SceneManager.LoadScene("ScatterPlotMatrix");
	}

	public void ValfriTeknik()
	{
		if (fileData == null)
		{
			file.text = errorMsg;
			return;
		}

		renderMode = 1;
		SceneManager.LoadScene("ValfriTeknik");
	}

	public void BackToMenu()
	{
		SceneManager.LoadScene("MainMenu");
	}

	public void QuitApplication()
	{
		Application.Quit();
	}

	#endregion

}
