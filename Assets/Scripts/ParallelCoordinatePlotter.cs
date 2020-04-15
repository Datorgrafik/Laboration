using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ParallelCoordinatePlotter : MonoBehaviour
{
	// List for holding data from CSV reader
	private List<Dictionary<string, object>> pointList;

	public List<string> columnList;
	public List<string> featureList;
	public float plotScale = 10;
	public GameObject PointPrefab;
	public GameObject PointHolder;
	public GameObject LinePrefab;
	public GameObject TargetFeatureList;
	private Color targetColor;
	private List<string> targetFeatures = new List<string>();
	private string columnName;

	// PlotColumns
	public Dropdown column1;
	public Dropdown column2;
	public Dropdown column3;
	public Dropdown column4;

	// Column Text Fields
	public TMP_Text column1Text;
	public TMP_Text column2Text;
	public TMP_Text column3Text;
	public TMP_Text column4Text;

	// Start is called before the first frame update
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		pointList = CSVläsare.Read(MainMenu.fileData);

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);

		// FeatureList without first or last index: Id / TargetColumn
		featureList = new List<string>();
		featureList.AddRange(columnList);
		featureList.RemoveAt(columnList.Count - 1);
		featureList.RemoveAt(0);
		
		// Assign column name from columnList to Name variables
		column1.AddOptions(featureList);
		column1.value = 0;
		column1.onValueChanged.AddListener(delegate { column1Text.text = featureList[column1.value]; });

		column2.AddOptions(featureList);
		column2.value = 1;
		column2.onValueChanged.AddListener(delegate { column2Text.text = featureList[column2.value]; });

		column3.AddOptions(featureList);
		column3.value = 2;
		column3.onValueChanged.AddListener(delegate { column3Text.text = featureList[column3.value]; });

		column4.AddOptions(featureList);
		column4.value = 3;
		column4.onValueChanged.AddListener(delegate { column4Text.text = featureList[column4.value]; });

		// Default values for each columntext at start
		column1Text.text = featureList[0];
		column2Text.text = featureList[1];
		column3Text.text = featureList[2];
		column4Text.text = featureList[3];

		GetDistinctTargetFeatures();

		// Instantiate a list of targetFeatures to the side of the plot
		foreach (var targetFeature in targetFeatures)
		{

		}

		// Run the default startup plot
		for (int i = 1; i <= 4; i++)
		{
			PlotData(i-1, i);
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

			InstantiateAndRenderDatapoints(xPos, i, y);

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
		if (pointList[i][columnList[5]].ToString() == targetFeatures[0])
			targetColor = new Color(0.9921569f, 0.9058824f, 0.1333333f);
		else if (pointList[i][columnList[5]].ToString() == targetFeatures[1])
			targetColor = new Color(0.1333333f, 0.5647059f, 0.5490196f);
		else if (pointList[i][columnList[5]].ToString() == targetFeatures[2])
			targetColor = new Color(0.2627451f, 0.0509804f, 0.3254902f);
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

			if (maxValue < float.Parse(maxValueStringLoop, CultureInfo.InvariantCulture))
				maxValue = float.Parse(maxValueStringLoop, CultureInfo.InvariantCulture);
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

			if (float.Parse(minValueStringLoop, CultureInfo.InvariantCulture) < minValue)
				minValue = float.Parse(minValueStringLoop, CultureInfo.InvariantCulture);
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

		PlotData(column1.value, 1);
		PlotData(column2.value, 2);
		PlotData(column3.value, 3);
		PlotData(column4.value, 4);
	}
}
