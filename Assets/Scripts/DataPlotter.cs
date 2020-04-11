using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;
using TMPro;

public class DataPlotter : MonoBehaviour
{
	// List for holding data from CSV reader
	public static List<Dictionary<string, object>> pointList;

	// Indices for columns to be assigned
	public int columnX = 1;
	public int columnY = 2;
	public int columnZ = 3;

	// Full column names
	public static string xName;
	public static string yName;
	public static string zName;

	[SerializeField]
	private TMP_Text xAxisText;
	[SerializeField]
	private TMP_Text yAxisText;
	[SerializeField]
	private TMP_Text zAxisText;

	public float plotScale = 10;
	public GameObject PointPrefab;
	public GameObject LineSeparatorPrefab;

	[SerializeField]
	public TMP_Text valuePrefab;

	public Dropdown xList;
	public Dropdown yList;
	public Dropdown zList;

	public GameObject PointHolder;
	public List<string> columnList;

	// Use this for initialization
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		Debug.Log(MainMenu.fileData);
		pointList = CSVläsare.Read(MainMenu.fileData);

		//Log to console
		Debug.Log(pointList);

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);

		// Print number of keys (using .count)
		Debug.Log("There are " + columnList.Count + " columns in CSV");

		foreach (string key in columnList)
			Debug.Log("Column name is " + key);

		// Assign column name from columnList to Name variables
		xList.AddOptions(columnList);
		xList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		yList.AddOptions(columnList);
		yList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		if (MainMenu.renderMode == 1)
		{
			zList.AddOptions(columnList);
			zList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });
		}
		
		xList.value = 1;
		yList.value = 2;

		if (MainMenu.renderMode == 1)
		{
			zList.value = 3;
		}

		PlottData();
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

	private void PlottData()
	{
		xName = columnList[xList.value];
		yName = columnList[yList.value];

		xAxisText.text = xName;
		yAxisText.text = yName;

		// Get maxes of each axis
		float xMax = FindMaxValue(xName);
		float yMax = FindMaxValue(yName);
		float zMax = float.MinValue;

		// Get minimums of each axis
		float xMin = FindMinValue(xName);
		float yMin = FindMinValue(yName);
		float zMin = float.MinValue;

		if (MainMenu.renderMode == 1)
		{
			zName = columnList[zList.value];
			zAxisText.text = zName;
			zMax = FindMaxValue(zName);
			zMin = FindMinValue(zName);
		}

		string valueString;

		if (MainMenu.renderMode == 0)
		{
			GameObject[] allGameObjects = GameObject.FindGameObjectsWithTag("dataValues");
			foreach (GameObject dataValues in allGameObjects)
			{
				GameObject.Destroy(dataValues);
			}

			for (int i = 0; i < 11; i++)
			{
				GameObject lineSeparatorX = Instantiate(LineSeparatorPrefab, new Vector3(i, 5.4F, -0.001F), Quaternion.identity);
				GameObject lineSeparatorY = Instantiate(LineSeparatorPrefab, new Vector3(5, i, -0.001F), Quaternion.identity);
				lineSeparatorX.transform.rotation = Quaternion.Euler(0, 0, 0);
				lineSeparatorY.transform.rotation = Quaternion.Euler(0, 0, 90);


				TMP_Text valuePointX = Instantiate(valuePrefab, new Vector3(i + 0.7F, -1, 0), Quaternion.identity);
				TMP_Text valuePointY = Instantiate(valuePrefab, new Vector3(-1, 0 + i, 0), Quaternion.identity);

				float xValue = ((xMax - xMin) / 10) * i + xMin;
				float yValue = ((yMax - yMin) / 10) * i + yMin;

				valuePointX.text = Convert.ToString(xValue);
				valuePointY.text = Convert.ToString(yValue);
			}
		}


		//Loop through Pointlist
		for (var i = 0; i < pointList.Count; i++)
		{
			// Get value in poinList at ith "row", in "column" Name, normalize
			valueString = pointList[i][xName].ToString();
			float x = (float.Parse(valueString, CultureInfo.InvariantCulture) - xMin) / (xMax - xMin);

			valueString = pointList[i][yName].ToString();
			float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - yMin) / (yMax - yMin);

			valueString = pointList[i][zName].ToString();
			float z = (float.Parse(valueString, CultureInfo.InvariantCulture) - zMin) / (zMax - zMin);

			GameObject dataPoint;

			if (MainMenu.renderMode == 0)
			{
				dataPoint = Instantiate(PointPrefab, new Vector3(x, y, 0) * plotScale, Quaternion.identity);
			}
			else
			{
				dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
			}

			// Gets material color and sets it to a new RGBA color we define
			dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);
			dataPoint.transform.parent = PointHolder.transform;
			string dataPointName = pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][zName];
			dataPoint.transform.name = dataPointName;
		}
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

	public void DropdownValueChanged()
	{
		GameObject ScatterPlotter = GameObject.Find("Scatterplot");

		foreach (Transform child in ScatterPlotter.transform)
		{
			Destroy(child.gameObject);
		}

		PlottData();
	}
}
