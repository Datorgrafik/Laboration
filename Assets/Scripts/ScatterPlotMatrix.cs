﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ScatterPlotMatrix : MonoBehaviour
{
	#region Attributes

	// List for holding data from CSV reader
	public static List<Dictionary<string, object>> pointList;

	public List<string> featureList;
	private int nFeatures;

	// Indices for columns to be assigned
	public int columnX = 1;
	public int columnY = 2;
	public int columnZ = 3;

	// Full column names
	public static string feature1Name;
	public static string feature2Name;

	public float plotScale = 10;
	public GameObject PointPrefab;
	public GameObject LineSeparatorPrefab;

	[SerializeField]
	public TMP_Text valuePrefab;

	// PlotColumns
	public List<Dropdown> columnDropdownList = new List<Dropdown>();

	public GameObject PointHolder;
	public GameObject planePointBackground;
	public TMP_Text ScatterplotMatrixText;
	public List<string> columnList;
	public List<string> targetFeatures;

	#endregion

	#region Methods

	// Use this for initialization
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

		AddDropdownListeners();

		PlottData();

		// Set Camera position
		Camera.main.transform.position = new Vector3(19.3F, 22.5F, -45.7F);
	}

	private void AddDropdownListeners()
	{
		// Assign column name from columnList to Name variables
		for (int i = 0; i < nFeatures; i++)
		{
			columnDropdownList[i].AddOptions(featureList);
			columnDropdownList[i].value = i;
			columnDropdownList[i].onValueChanged.AddListener(delegate { DropdownValueChanged(); });

			if (i + 1 == 4)
				break;
		}
	}

	private void PlottData()
	{
		ResetDataPlot();
        GetDistinctTargetFeatures();

        for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				try
				{
					feature1Name = featureList[columnDropdownList[j].value];
					feature2Name = featureList[columnDropdownList[k].value];

					if (j == k)
					{
                        GameObject summonPlane = Instantiate(planePointBackground,
                                    new Vector3(k * 1.2F + 0.5F, j * 1.2F + 0.5F, 0) * plotScale,
                                    Quaternion.Euler(0, 90, -90));
                        //Le textfält
                        TMP_Text textField = Instantiate(ScatterplotMatrixText,
														new Vector3(k * 1.2F + 1, j * 1.2F + 0.3F, 0) * plotScale,
														Quaternion.identity);

						textField.text = feature2Name;
					}
					else if(k > j)
					{
                        GameObject summonPlane = Instantiate(planePointBackground,
                                    new Vector3(k * 1.2F + 0.5F, j * 1.2F + 0.5F, 0) * plotScale,
                                    Quaternion.Euler(0, 90, -90));
                        // Get maxes of each axis
                        float xMax = FindMinMaxValue.FindMaxValue(feature1Name, pointList);
						float yMax = FindMinMaxValue.FindMaxValue(feature2Name, pointList);

						// Get minimums of each axis
						float xMin = FindMinMaxValue.FindMinValue(feature1Name, pointList);
						float yMin = FindMinMaxValue.FindMinValue(feature2Name, pointList);

						string valueString;

						//Loop through Pointlist
						for (var i = 0; i < pointList.Count; i++)
						{
							GameObject dataPoint;

							// Get value in poinList at ith "row", in "column" Name, normalize
							valueString = pointList[i][feature1Name].ToString();
							float x = (float.Parse(valueString, CultureInfo.InvariantCulture) - xMin) / (xMax - xMin);

							valueString = pointList[i][feature2Name].ToString();
							float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - yMin) / (yMax - yMin);

							float index = targetFeatures.IndexOf(pointList[i][columnList[columnList.Count - 1]].ToString());
							float colorValue = 1 / (index + 1);

							// Instantiate dataPoint
							dataPoint = Instantiate(PointPrefab, 
													new Vector3(x + k * 1.2F, y + j * 1.2F, 0) * plotScale, 
													Quaternion.identity);
							// Set transform name
							dataPoint.transform.name = pointList[i][feature1Name] + " " + pointList[i][feature2Name];
							// Set parent
							dataPoint.transform.parent = PointHolder.transform;

							dataPoint.GetComponent<StoreIndexInDataBall>().Index = i;
							dataPoint.GetComponent<StoreIndexInDataBall>().TargetFeature =
								pointList[i][columnList[columnList.Count - 1]].ToString();


                            // Set color
                            bool ClassCheck = float.TryParse((pointList[i][columnList[columnList.Count() - 1]].ToString().Replace('.', ',')), out float n);
                            if (!ClassCheck)
                            {
                                if (index % 3 == 0)
                                    dataPoint.GetComponent<Renderer>().material.color = new Color(0, colorValue, 0, 1.0f);
                                else if (index % 3 == 1)
                                    dataPoint.GetComponent<Renderer>().material.color = new Color(1, 0, colorValue, 1.0f);
                                else if (index % 3 == 2)
                                    dataPoint.GetComponent<Renderer>().material.color = new Color(colorValue, 0, 1, 1.0f);
                            }
                            else
                            {
                                dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, y, 1.0f);
                            }
						}
					}
				}
				catch (Exception) { }
			}
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

	private static void ResetDataPlot()
	{
		GameObject[] allGameObjects = GameObject.FindGameObjectsWithTag("TextValues");
		foreach (GameObject dataValues in allGameObjects)
		{
			Destroy(dataValues);
		}

		GameObject[] allDataBalls = GameObject.FindGameObjectsWithTag("DataBall");
		foreach (GameObject dataValues in allDataBalls)
		{
			Destroy(dataValues);
		}
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

	#endregion
}
