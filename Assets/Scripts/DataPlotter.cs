﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class DataPlotter : MonoBehaviour
{
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
	public List<string> targetFeatures;


    public float xMax;
    public float yMax;
    public float zMax;
    public float xMin;
    public float yMin;
    public float zMin;


    public static DataPlotter ThisInstans;
    public static DataClass dataClass;
    private int selectedIndex = -1;

	// Use this for initialization
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		//Debug.Log(MainMenu.fileData);
		dataClass = CSVläsare.Read(MainMenu.fileData);
        pointList = dataClass.CSV;
        ThisInstans = this;
        //Log to console
        //Debug.Log(pointList);

        // Declare list of strings, fill with keys (column names)
        columnList = new List<string>(pointList[1].Keys);
		// Print number of keys (using .count)
		//Debug.Log("There are " + columnList.Count + " columns in CSV");

		//foreach (string key in columnList)
		//	Debug.Log("Column name is " + key);

		// Assign column name from columnList to Name variables
		xList.AddOptions(columnList);
        xList.value = 1;
        xList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });


		yList.AddOptions(columnList);
        yList.value = 2;
        yList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });


		if (MainMenu.renderMode == 1)
		{
			zList.AddOptions(columnList);
            zList.value = 3;
            zList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		}

		PlottData();
	}

	private float FindMaxValue(string columnName)
	{
		//set initial value to first value
		string maxValueString = pointList[0][columnName].ToString();
		float maxValue = float.Parse(maxValueString, CultureInfo.InvariantCulture);

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

	public void PlottData()
	{
        if(TargetingScript.selectedTarget != null)
        {
            selectedIndex = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
        }

        GameObject[] allDataBalls = GameObject.FindGameObjectsWithTag("DataBall");
        foreach (GameObject dataValues in allDataBalls)
        {
            Destroy(dataValues);
        }

		xName = columnList[xList.value];
		yName = columnList[yList.value];

		xAxisText.text = xName;
		yAxisText.text = yName;

		// Get maxes of each axis
		xMax = FindMaxValue(xName);
		yMax = FindMaxValue(yName);
		zMax = 0f;

		// Get minimums of each axis
		xMin = FindMinValue(xName);
		yMin = FindMinValue(yName);
		zMin = 0f;

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
				Destroy(dataValues);
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

                valuePointX.text = xValue.ToString("0.00"); 
                valuePointY.text = yValue.ToString("0.00"); 

            }
		}


		//Loop through Pointlist
		for (var i = 0; i < pointList.Count; i++)
		{
			GameObject dataPoint;

			// Get value in poinList at ith "row", in "column" Name, normalize
			valueString = pointList[i][xName].ToString();
			float x = (float.Parse(valueString, CultureInfo.InvariantCulture) - xMin) / (xMax - xMin);

			valueString = pointList[i][yName].ToString();
			float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - yMin) / (yMax - yMin);

			float z;

			//Lägger in alla targetfeatures i en lista
			if (targetFeatures.Count == 0 || !targetFeatures.Contains(pointList[i][columnList[columnList.Count - 1]].ToString()))
			{
				targetFeatures.Add(pointList[i][columnList[columnList.Count - 1]].ToString());
			}

			if (MainMenu.renderMode == 0)
			{
				dataPoint = Instantiate(PointPrefab, new Vector3(x, y, 0) * plotScale, Quaternion.identity);
				dataPoint.transform.name = pointList[i][xName] + " " + pointList[i][yName];
				dataPoint.transform.parent = PointHolder.transform;
			}
			else
			{
				valueString = pointList[i][zName].ToString();
				z = (float.Parse(valueString, CultureInfo.InvariantCulture) - zMin) / (zMax - zMin);
				dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
                dataPoint.transform.name = pointList[i][columnList[0]] + " " + pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][zName] + " " + pointList[i][columnList[columnList.Count()-1]];
				dataPoint.transform.parent = PointHolder.transform;
               // if (!pointList[i].ContainsKey("DataBall"))
                 //   pointList[i].Add("DataBall", dataPoint);
                //else
                  //  pointList[i]["DataBall"] = dataPoint;
			}

			dataPoint.GetComponent<StoreIndexInDataBall>().Index = i;
			dataPoint.GetComponent<StoreIndexInDataBall>().TargetFeature = pointList[i][columnList[columnList.Count - 1]].ToString();

			int index = targetFeatures.IndexOf(pointList[i][columnList[columnList.Count - 1]].ToString());
			ChangeColor(dataPoint, index);

			if (selectedIndex == i)
			{
				TargetingScript.selectedTarget = dataPoint;
				TargetingScript.colorOff = TargetingScript.selectedTarget.GetComponent<Renderer>().material.color;
				TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = Color.white;
				TargetingScript.selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
				selectedIndex = -1;
			}
		}
       // GameObject newBall = (GameObject)pointList.Last()["DataBall"] as GameObject;
        //transform.LookAt(newBall.transform);
    }

	public static void ChangeColor(GameObject dataPoint, int targetFeatureIndex)
	{
		float colorValue = (float)1 / (targetFeatureIndex + 1);

		if (targetFeatureIndex % 3 == 0)
		{
			dataPoint.GetComponent<Renderer>().material.color = new Color(0, colorValue, 0, 1.0f);
		}
		else if (targetFeatureIndex % 3 == 1)
		{
			dataPoint.GetComponent<Renderer>().material.color = new Color(1, 0, colorValue, 1.0f);
		}
		else if (targetFeatureIndex % 3 == 2)
		{
			dataPoint.GetComponent<Renderer>().material.color = new Color(colorValue, 0, 1, 1.0f);
		}
	}

	private float FindMinValue(string columnName)
	{
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

    static public void AddDataPoint(List<string> newPoint, string k, bool weightedOrNot)
    {
        Dictionary<string, object> last = pointList.Last();

        Dictionary<string, object> newDataPoint = new Dictionary<string, object>();

        newDataPoint.Add("", (Convert.ToInt32(last[""], CultureInfo.InvariantCulture)) + 1);

        for (int i = 0; i < ThisInstans.columnList.Count - 2; i++)
        {
            newDataPoint.Add(ThisInstans.columnList[i + 1], newPoint[i]);
        }

        double[] unknown = new double[newPoint.Count];

        for (int i = 0; i < newPoint.Count; ++i)
        {
            unknown[i] = (Convert.ToDouble(newPoint[i], CultureInfo.InvariantCulture));
            Debug.Log(newPoint[i].ToString());
        }

        var predict = dataClass.Knn(unknown, k, weightedOrNot);
        newDataPoint.Add(ThisInstans.columnList[ThisInstans.columnList.Count - 1], predict);

        pointList.Add(newDataPoint);

        ThisInstans.PlottData();

    }
}