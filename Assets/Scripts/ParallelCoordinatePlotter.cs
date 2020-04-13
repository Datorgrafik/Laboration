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
	public float plotScale = 10;
	public GameObject PointPrefab;
	public GameObject PointHolder;
	public GameObject LinePrefab;
	private Color targetColor;
	private List<string> targetFeatures = new List<string>();
	private string columnName;

	// PlotColumns
	[SerializeField]
	private Dropdown column1;
	[SerializeField]
	private Dropdown column2;
	[SerializeField]
	private Dropdown column3;
	[SerializeField]
	private Dropdown column4;

	// Column Text Fields
	[SerializeField]
	private TMP_Text column1Text;
	[SerializeField]
	private TMP_Text column2Text;
	[SerializeField]
	private TMP_Text column3Text;
	[SerializeField]
	private TMP_Text column4Text;

	// Start is called before the first frame update
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		pointList = CSVläsare.Read(MainMenu.fileData);

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);

		// Assign column name from columnList to Name variables
		column1.AddOptions(columnList);
		column1.value = 1;
		column1.onValueChanged.AddListener(delegate { DropdownValueChanged(1); });

		column2.AddOptions(columnList);
		column2.value = 2;
		column2.onValueChanged.AddListener(delegate { DropdownValueChanged(2); });

		column3.AddOptions(columnList);
		column3.value = 3;
		column3.onValueChanged.AddListener(delegate { DropdownValueChanged(3); });

		column4.AddOptions(columnList);
		column4.value = 4;
		column4.onValueChanged.AddListener(delegate { DropdownValueChanged(4); });

		// Default values for each columntext at start
		column1Text.text = columnList[1];
		column2Text.text = columnList[2];
		column3Text.text = columnList[3];
		column4Text.text = columnList[4];

		GetDistinctTargetFeatures();

		// Run the default startup plot
		for (int i = 1; i <= 4; i++)
		{
			PlotData(i, i);
		}
	}

	private void GetDistinctTargetFeatures()
	{
		// Add targetFeatures to a seperate list
		for (int i = 0; i < pointList.Count; i++)
		{
			targetFeatures.Add(pointList[i][columnList[5]].ToString());
		}

		// Only keep distinct targetFeatures
		targetFeatures = targetFeatures.Distinct().ToList();
	}

	private void PlotData(int column, int columnPos)
	{
		columnName = columnList[column];

		// Sets the correct X-Axis position for each column
		float xPos = SetColumnPosition(columnPos);

		// Get MaxValue
		float columnMax = FindMaxValue(columnName);
		// Get MinValue
		float columnMin = FindMinValue(columnName);

		string valueString;
		//Loop through Pointlist
		for (var i = 0; i < pointList.Count; i++)
		{
			// Set correct color
			targetColor = SetColors(targetFeatures, i);

			// Get original value
			valueString = pointList[i][columnName].ToString();
			// Set normalized Y-value
			float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - columnMin) / (columnMax - columnMin);
			// Create clone
			GameObject dataPoint = Instantiate(PointPrefab, new Vector3(xPos, y, 0) * plotScale, Quaternion.identity);
			// Set color
			dataPoint.GetComponent<Renderer>().material.color = targetColor;
			// Set parent
			dataPoint.transform.parent = PointHolder.transform;
			// Set name
			dataPoint.transform.name = Convert.ToString(pointList[i][columnName]);

			GameObject dataLine;
			// Instantiate LineRenderer at first column for every point
			if (columnPos == 1)
			{
				// Instantiate the line
				dataLine = Instantiate(LinePrefab, new Vector3(xPos, y, -0.001f) * plotScale, Quaternion.identity);
				// Set parent
				dataLine.transform.parent = PointHolder.transform;
				// Set name
				dataLine.transform.name = Convert.ToString("Line " + (i + 1));
			}

			// Find the correct dataLine
			dataLine = GameObject.Find("Line " + (i + 1));

			// Get the LineRenderer
			LineRenderer lineRenderer = dataLine.GetComponent<LineRenderer>();
			// Set line color
			lineRenderer.material.color = targetColor;
			// Set line position
			lineRenderer.SetPosition((columnPos - 1), new Vector3(xPos, y, -0.001f) * plotScale);
		}
	}

	private static float SetColumnPosition(int columnPos)
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
		//set initial value to first value
		string maxValueString = pointList[0][columnName].ToString();
		float maxValue = float.Parse(maxValueString, CultureInfo.InvariantCulture);

		//float maxValue = Convert.ToSingle(pointList[0][columnName]);

		//Loop through Dictionary, overwrite existing maxValue if new value is larger
		for (var i = 0; i < pointList.Count; i++)
		{
			string maxValueStringLoop = pointList[i][columnName].ToString();

			if (maxValue < float.Parse(maxValueStringLoop, CultureInfo.InvariantCulture))
				maxValue = float.Parse(maxValueStringLoop, CultureInfo.InvariantCulture);
		}

		//Spit out the max value
		return maxValue;
	}

	private float FindMinValue(string columnName)
	{
		//float minValue = Convert.ToSingle(pointList[0][columnName]);

		string minValueString = pointList[0][columnName].ToString();
		float minValue = float.Parse(minValueString, CultureInfo.InvariantCulture);

		//Loop through Dictionary, overwrite existing minValue if new value is smaller
		for (var i = 0; i < pointList.Count; i++)
		{
			string minValueStringLoop = pointList[i][columnName].ToString();

			if (float.Parse(minValueStringLoop, CultureInfo.InvariantCulture) < minValue)
				minValue = float.Parse(minValueStringLoop, CultureInfo.InvariantCulture);
		}

		return minValue;
	}

	private void DropdownValueChanged(int columnPos)
	{
		// Find the changed column and destroy it.
		GameObject parallelCoordinatePlot = GameObject.Find("ParallelCoordinatePlot");

		foreach (Transform child in parallelCoordinatePlot.transform)
		{
			Destroy(child.gameObject);
		}

		if (columnPos == 1)
			column1Text.text = columnList[column1.value];
		else if (columnPos == 2)
			column2Text.text = columnList[column2.value];
		else if (columnPos == 3)
			column3Text.text = columnList[column3.value];
		else if (columnPos == 4)
			column4Text.text = columnList[column4.value];

		// Run PlotData() with the selected columns.
		PlotData(column1.value, 1);
		PlotData(column2.value, 2);
		PlotData(column3.value, 3);
		PlotData(column4.value, 4);
	}
	
}
