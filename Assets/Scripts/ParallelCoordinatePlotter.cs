using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParallelCoordinatePlotter : MonoBehaviour
{
	#region Attributes

	// List for holding data from CSV reader
	private List<Dictionary<string, object>> pointList;

	// Lists
	public List<string> columnList;
	public List<string> featureList;
	private List<string> targetFeatures = new List<string>();

	// GameObjects
	public GameObject PointPrefab;
	public GameObject LinePrefab;
	public GameObject TargetFeaturePrefab;
	public GameObject PointHolder;

	// Misc
	public float plotScale = 10;
	public TMP_Text valuePrefab;
	private Color targetColor;
	private string columnName;
	private int nFeatures;

	// PlotColumns
	public Dropdown column1;
	public Dropdown column2;
	public Dropdown column3;
	public Dropdown column4;
	List<Dropdown> columnDropdownList;

	// Column Text Fields
	public TMP_Text column1Text;
	public TMP_Text column2Text;
	public TMP_Text column3Text;
	public TMP_Text column4Text;
	List<TMP_Text> columnTextList;

	#endregion

	#region Methods

	// Start is called before the first frame update
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		DataClass dataClass = CSVläsare.Read(MainMenu.fileData);
		pointList = dataClass.CSV;

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);

		// FeatureList without first and last index: Id / TargetColumn
		featureList = new List<string>();
		featureList.AddRange(columnList);
		featureList.RemoveAt(columnList.Count - 1);
		featureList.RemoveAt(0);
		nFeatures = featureList.Count;

		columnTextList = new List<TMP_Text>()
		{
			column1Text,
			column2Text,
			column3Text,
			column4Text
		};

		columnDropdownList = new List<Dropdown>()
		{
			column1,
			column2,
			column3,
			column4
		};

		AddDropdownListeners();

		// Default values for each columntext at start, depending on nFeatures (max 4)
		for (int i = 0; i < nFeatures; i++)
		{
			columnTextList[i].text = featureList[i];

			if (i+1 == 4)
				break;
		}

		GetDistinctTargetFeatures();

		InstantiateTargetFeatureList();

		DrawBackgroundGrid();

		// Run the default startup plot
		for (int i = 1; i <= 4; i++)
		{
			PlotData(i - 1, i);
		}
	}

	private void AddDropdownListeners()
	{
		// Assign column name from columnList to Name variables
		for (int i = 0; i < nFeatures; i++)
		{
			columnDropdownList[i].AddOptions(featureList);
			columnDropdownList[i].value = i;
			columnDropdownList[i].onValueChanged.AddListener(delegate { columnTextList[i].text = featureList[columnDropdownList[i].value]; });

			if (i + 1 == 4)
				break;
		}
	}

	private void DrawBackgroundGrid()
	{
		// Draw vertical lines
		for (int i = 1; i <= 4; i++)
		{
			float xPos = SetColumnPosition(i);

			GameObject xLine = Instantiate(LinePrefab, new Vector3(xPos, 0f, -0.001f) * plotScale, Quaternion.identity);
			xLine.transform.parent = PointHolder.transform;
			xLine.transform.name = $"Column{i}Line";

			LineRenderer xLineRenderer = xLine.GetComponent<LineRenderer>();
			xLineRenderer.positionCount = 2;
			xLineRenderer.startWidth = 0.025f;
			xLineRenderer.endWidth = 0.025f;
			xLineRenderer.SetPosition(0, new Vector3(xPos, -0.05f, -0.001f) * plotScale);
			xLineRenderer.SetPosition(1, new Vector3(xPos, 1.05f, -0.001f) * plotScale);
			xLineRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		}

		float yMax = 0f;
		float yMin = float.Parse(pointList[0][featureList[0]].ToString(), CultureInfo.InvariantCulture);

		// Find and render Max- & Min-values on Y-Axis
		for (int i = 0; i < featureList.Count; i++)
		{
			Debug.Log(featureList[i]);
			float yMaxTempValue = FindMaxValue(featureList[i]);
			if (yMaxTempValue > yMax)
				yMax = yMaxTempValue;

			float yMinTempValue = FindMinValue(featureList[i]);
			if (yMinTempValue < yMin)
				yMin = yMinTempValue;
		}

		// Draw horizontal lines
		for (int i = 0; i <= 10; i++)
		{
			// Instantiate lines, set parent, set transform name
			GameObject yLine = Instantiate(LinePrefab, new Vector3(0, 0f, -0.001f) * plotScale, Quaternion.identity);
			yLine.transform.parent = PointHolder.transform;
			yLine.transform.name = $"Value{i}Line";

			LineRenderer yLineRenderer = yLine.GetComponent<LineRenderer>();
			yLineRenderer.positionCount = 2;
			yLineRenderer.startWidth = 0.025f;
			yLineRenderer.endWidth = 0.025f;
			yLineRenderer.SetPosition(0, new Vector3(0.1f, (float)i / 10, -0.001f) * plotScale);
			yLineRenderer.SetPosition(1, new Vector3(1.5f, (float)i / 10, -0.001f) * plotScale);
			yLineRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);

			TMP_Text valuePointY = Instantiate(valuePrefab, new Vector3(1.25f, 0 + i, 0), Quaternion.identity);
			valuePointY.transform.name = $"Value{i}LineText";

			float yValue = ((yMax - yMin) / 10) * i + yMin;

			valuePointY.text = Convert.ToString(yValue);
		}
	}

	private void GetDistinctTargetFeatures()
	{
		// Add targetFeatures to a seperate list
		for (int i = 0; i < pointList.Count; i++)
		{
			targetFeatures.Add(pointList[i][columnList[columnList.Count - 1]].ToString());
		}

		// Only keep distinct targetFeatures
		targetFeatures = targetFeatures.Distinct().ToList();
	}

	private void InstantiateTargetFeatureList()
	{
		float targetXpos = 17f;
		float targetYpos = 8f;
		float targetZpos = 0f;
		Color targetColor;

		// Instantiate a list of targetFeatures to the side of the plot
		foreach (var targetFeature in targetFeatures)
		{
			// Instantiate targetFeaturePoint and name it
			GameObject targetFeaturePoint = Instantiate(TargetFeaturePrefab, new Vector3(targetXpos, targetYpos, targetZpos), Quaternion.identity);
			targetFeaturePoint.name = targetFeature;

			float index = targetFeatures.IndexOf(targetFeature.ToString());
			float colorValue = 1 / (index + 1);

			if (index % 3 == 0)
				targetColor = new Color(0, colorValue, 0);
			else if (index % 3 == 1)
				targetColor = new Color(0, 0, colorValue);
			else if (index % 3 == 2)
				targetColor = new Color(colorValue, 0, 0);
			else
				targetColor = Color.black;

			// Set color and text
			targetFeaturePoint.GetComponentInChildren<Renderer>().material.color = targetColor;
			TextMeshPro text = targetFeaturePoint.GetComponentInChildren<TextMeshPro>();
			text.text = targetFeature;
			text.color = targetColor;

			// Change Y-Position for next targetFeature in the loop
			targetYpos -= 1f;
		}
	}

	private void PlotData(int column, int columnPos)
	{
		columnName = featureList[column];

		// Sets the correct X-Axis position for each column
		float xPos = SetColumnPosition(columnPos);

		// Get MaxValue
		float columnMax = FindMaxValue(columnName);
		// Get MinValue
		float columnMin = FindMinValue(columnName);

		//Loop through Pointlist & Render dataset
		for (var i = 0; i < pointList.Count; i++)
		{
			// Set correct color
			targetColor = SetColors(targetFeatures, i);

			// Get original value
			string valueString = pointList[i][columnName].ToString();
			// Set normalized Y-value
			float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - columnMin) / (columnMax - columnMin);

			//InstantiateAndRenderDatapoints(xPos, i, y);

			InstantiateAndRenderLines(columnPos, xPos, i, y);
		}
	}

	private void InstantiateAndRenderDatapoints(float xPos, int i, float y)
	{
		// Create clone
		GameObject dataPoint = Instantiate(PointPrefab, new Vector3(xPos, y, 0) * plotScale, Quaternion.identity);
		// Set color
		dataPoint.GetComponent<Renderer>().material.color = targetColor;
		// Set new scale
		dataPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		// Set parent
		dataPoint.transform.parent = PointHolder.transform;
		// Set name
		dataPoint.transform.name = Convert.ToString(pointList[i][columnName]);
	}

	private void InstantiateAndRenderLines(int columnPos, float xPos, int i, float y)
	{
		GameObject dataLine;
		LineRenderer lineRenderer;

		// Instantiate LineRenderer at first column for every point
		if (columnPos == 1 && GameObject.Find("Line " + (i + 1)) == null)
		{
			// Instantiate the line
			dataLine = Instantiate(LinePrefab, new Vector3(xPos, y, -0.001f) * plotScale, Quaternion.identity);
			// Set parent
			dataLine.transform.parent = PointHolder.transform;
			// Set name
			dataLine.transform.name = Convert.ToString("Line " + (i + 1));
			// Get the LineRenderer
			lineRenderer = dataLine.GetComponent<LineRenderer>();
			// Set line color
			lineRenderer.material.color = targetColor;
		}
		else
		{
			// Find the correct dataLine
			dataLine = GameObject.Find("Line " + (i + 1));
			// Get the LineRenderer
			lineRenderer = dataLine.GetComponent<LineRenderer>();
		}

		// Set line position
		lineRenderer.SetPosition((columnPos - 1), new Vector3(xPos, y, -0.001f) * plotScale);
	}

	private float SetColumnPosition(int columnPos)
	{
		float xPos = float.MinValue;

		if (columnPos == 1)
			xPos = 0.2f;
		else if (columnPos == 2)
			xPos = 0.6f;
		else if (columnPos == 3)
			xPos = 1.0f;
		else if (columnPos == 4)
			xPos = 1.4f;

		return xPos;
	}

	private Color SetColors(List<string> targetFeatures, int i)
	{
		Color targetColor;
		float index = targetFeatures.IndexOf(pointList[i][columnList[columnList.Count - 1]].ToString());
		float colorValue = 1 / (index + 1);

		if (index % 3 == 0)
			targetColor = new Color(0, colorValue, 0);
		else if (index % 3 == 1)
			targetColor = new Color(0, 0, colorValue);
		else if (index % 3 == 2)
			targetColor = new Color(colorValue, 0, 0);
		else
			targetColor = Color.black;

		return targetColor;
	}

	private float FindMaxValue(string columnName)
	{
		string maxValueString = pointList[0][columnName].ToString();
		float maxValue = float.Parse(maxValueString, CultureInfo.InvariantCulture);

		for (var i = 0; i < pointList.Count; i++)
		{
			string maxValueStringLoop = pointList[i][columnName].ToString();

			try
			{
				if (maxValue < float.Parse(maxValueStringLoop, CultureInfo.InvariantCulture))
					maxValue = float.Parse(maxValueStringLoop, CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				// Catches '?' as missing values that cannot be converted to floats in the dataset.
			}
		}

		return maxValue;
	}

	private float FindMinValue(string columnName)
	{
		string minValueString = pointList[0][columnName].ToString();
		float minValue = float.Parse(minValueString, CultureInfo.InvariantCulture);

		for (var i = 0; i < pointList.Count; i++)
		{
			string minValueStringLoop = pointList[i][columnName].ToString();

			try
			{
				if (float.Parse(minValueStringLoop, CultureInfo.InvariantCulture) < minValue)
					minValue = float.Parse(minValueStringLoop, CultureInfo.InvariantCulture);
			}
			catch (Exception)
			{
				// Catches '?' as missing values that cannot be converted to floats in the dataset.
			}
		}

		return minValue;
	}

	public void ReorderColumns()
	{
		// Destroy Datapoints
		GameObject[] databalls = GameObject.FindGameObjectsWithTag("DataBall");

		foreach (var databall in databalls)
		{
			Destroy(databall);
		}

		// Plot data for the four selected columns
		PlotData(column1.value, 1);
		PlotData(column2.value, 2);
		PlotData(column3.value, 3);
		PlotData(column4.value, 4);
	}

	#endregion
}
