using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.IO;
using System.Linq;

public class MainMenu : MonoBehaviour
{
	#region Attributes

	public Dropdown renderModeDropdown;
	public static int renderMode = 0;
	public Dropdown DatasetDropdown;
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

		// Get filepath info from Dataset directory if it exists
		if (Directory.Exists(Path.Combine(Application.dataPath, "Datasets")))
		{
			string filePath = Path.Combine(Application.dataPath, "Datasets");
			List<string> fileList = Directory.GetFiles(filePath, "*.csv").ToList();

			DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
			List<string> fileNames = new List<string>();

			// Get all filenames with extentions.
			foreach (var file in directoryInfo.GetFiles("*.csv"))
			{
				fileNames.Add(file.Name.ToString());
			}

			// Add filepath info to Dropdown options in DatasetDropdown
			DatasetDropdown.AddOptions(fileNames);
			DatasetDropdown.onValueChanged.AddListener(delegate
			{
				fileData = File.ReadAllText(fileList[DatasetDropdown.value-1]);
				file.text = fileNames[DatasetDropdown.value-1];
			});
		}
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
