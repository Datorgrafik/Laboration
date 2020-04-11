﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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

	// Start is called before the first frame update
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		pointList = CSVläsare.Read(MainMenu.fileData);

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);

		// Assign column name from columnList to Name variables
		column1.AddOptions(columnList);
		column1.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		column2.AddOptions(columnList);
		column2.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		column3.AddOptions(columnList);
		column3.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		column4.AddOptions(columnList);
		column4.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		// Default values for each column at start
		column1.value = 1;
		column2.value = 2;
		column3.value = 3;
		column4.value = 4;

		// Run the default startup plot
		for (int i = 1; i <= 4; i++)
		{
			PlotData(i, i);
		}

	}

	private void PlotData(int column, int columnPos)
	{
		columnName = columnList[column];
		float xPos = (float)columnPos/10;

		// Get MaxValue
		float columnMax = FindMaxValue(columnName);

		// Get MinValue
		float columnMin = FindMinValue(columnName);

		string valueString;
		//Loop through Pointlist
		for (var i = 0; i < pointList.Count; i++)
		{
			// Get value in poinList at ith "row", in "column" Name, normalize
			valueString = pointList[i][columnName].ToString();
			float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - columnMin) / (columnMax - columnMin);

			GameObject dataPoint;

			dataPoint = Instantiate(PointPrefab, new Vector3(xPos, y, 0) * plotScale, Quaternion.identity);

			// Gets material color and sets it to a new RGBA color we define
			//dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f, y, 1.0f, 1.0f);
			dataPoint.transform.parent = PointHolder.transform;
			string dataPointName = Convert.ToString(columnName + ": "  + pointList[i][columnName]);
			dataPoint.transform.name = dataPointName;
		}
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

	private void DropdownValueChanged()
	{
	   // Find the changed column and destroy it.

		// Run PlotData() with the newly selected column.

		//PlotData(string.Empty);
	}

}