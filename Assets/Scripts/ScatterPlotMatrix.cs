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
    public static string feature3Name;
    public static string feature4Name;

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
	public static ScatterPlotMatrix ThisInstans;

	// ColorList
	private static readonly Color[] colorList =
	{
		new Color(52, 152, 219, 1),
		new Color(192, 57, 43,1),
		new Color(46, 204, 113,1),
		new Color(26, 188, 156,1),
		new Color(155, 89, 182,1),
		new Color(52, 73, 94,1),
		new Color(241, 196, 15,1),
		new Color(230, 126, 34,1),
		new Color(189, 195, 199,1),
		new Color(149, 165, 166,1)
	};

	public static DataClass dataClass;
	private int selectedIndex = -1;
    private string selectedRow = "";
    private string selectedColumn = "";
	public bool teleportCamera = false;
	public GameObject KNNWindow;
	public Color colorOff;


	public static string K;
	public static bool Weighted;

	#endregion

	#region Methods

	// Use this for initialization
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		dataClass = CSVläsare.Read(MainMenu.fileData);
		pointList = dataClass.CSV;

		// Declare list of strings, fill with keys (column names)
		columnList = new List<string>(pointList[1].Keys);

		// FeatureList without first and last index: Id / TargetColumn
		featureList = new List<string>();
		featureList.AddRange(columnList);
		featureList.RemoveAt(columnList.Count - 1);
		featureList.RemoveAt(0);
		nFeatures = featureList.Count;
		ThisInstans = this;

		AddDropdownListeners();

		PlottData();

		// Set Camera position
		Camera.main.transform.position = new Vector3(19.3F, 22.5F, -45.7F);
	}
	private void Update()
	{
		if (KNN.KNNMode && KNN.KNNMove)
		{
			ChangeDataPoint(K, Weighted);
			KNN.KNNMove = false;
		}
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

	public void PlottData()
	{
        if (TargetingScript.selectedTarget != null)
        {
            selectedIndex = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
            selectedRow = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Row;
            selectedColumn = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Column;
        }
        Debug.Log(selectedRow);
        Debug.Log(selectedColumn);

        ResetDataPlot();
		GetDistinctTargetFeatures();

		for (int j = 0; j < 4; j++)
		{
			for (int k = 0; k < 4; k++)
			{
				try
				{
					feature1Name = featureList[columnDropdownList[0].value];
					feature2Name = featureList[columnDropdownList[1].value];
                    feature3Name = featureList[columnDropdownList[2].value];
                    feature4Name = featureList[columnDropdownList[3].value];

                    if (j == k)
					{
						GameObject summonPlane = Instantiate(planePointBackground,
									new Vector3(k * 1.2F + 0.5F, j * 1.2F + 0.5F, 0) * plotScale,
									Quaternion.Euler(0, 90, -90));
						
						//Le textfält
						TMP_Text textField = Instantiate(ScatterplotMatrixText,
														new Vector3(k * 1.2F + 1, j * 1.2F + 0.3F, -0.01f) * plotScale,
														Quaternion.identity);

						textField.text = featureList[columnDropdownList[j].value];
					}
					else if(k < j)
					{
						GameObject summonPlane = Instantiate(planePointBackground,
									new Vector3(k * 1.2F + 0.5F, j * 1.2F + 0.5F, 0) * plotScale,
									Quaternion.Euler(0, 90, -90));

						// Get maxes of each axis
						float xMax = CalculationHelpers.FindMaxValue(feature1Name, pointList);
						float yMax = CalculationHelpers.FindMaxValue(feature2Name, pointList);

						// Get minimums of each axis
						float xMin = CalculationHelpers.FindMinValue(feature1Name, pointList);
						float yMin = CalculationHelpers.FindMinValue(feature2Name, pointList);

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

							int index = targetFeatures.IndexOf(pointList[i][columnList[columnList.Count - 1]].ToString());

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
                            dataPoint.GetComponent<StoreIndexInDataBall>().Column = featureList[columnDropdownList[j].value];
                            dataPoint.GetComponent<StoreIndexInDataBall>().Row = featureList[columnDropdownList[k].value];
                            dataPoint.GetComponent<StoreIndexInDataBall>().Feature3 = feature3Name;
                            dataPoint.GetComponent<StoreIndexInDataBall>().Feature4 = feature4Name;


                            if (!pointList[i].ContainsKey("DataBall"))
								pointList[i].Add("DataBall", dataPoint);
							else
								pointList[i]["DataBall"] = dataPoint;

							// Set color
							if (targetFeatures.Count() <= 10)
								dataPoint.GetComponent<Renderer>().material.color = new Color(colorList[index].r / 255, colorList[index].g / 255, colorList[index].b / 255, 1.0f);
							else
								dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, y, 1.0f);

							if (KNN.KNNMode && i == pointList.Count() - 1)
							{
								dataPoint.GetComponent<Renderer>().material.color = Color.white;
								dataPoint.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
							}
                            //Reselect target if one was selected before.
                            if (selectedIndex == i && dataPoint.GetComponent<StoreIndexInDataBall>().Column == selectedColumn && dataPoint.GetComponent<StoreIndexInDataBall>().Row == selectedRow)
                            {
                                TargetingScript.selectedTarget = dataPoint;
                                TargetingScript.colorOff = TargetingScript.selectedTarget.GetComponent<Renderer>().material.color;
                                TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = Color.white;
                                TargetingScript.selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
                                selectedIndex = -1;
                            }
                        }
					}
				}
				catch (Exception) { }

				if (KNN.kPoints != null)
					if (KNN.kPoints.Count > 0)
						Blink(KNN.kPoints);
			}
        }

		if (ThisInstans.teleportCamera)
		{
			// ThisInstans.teleportCamera = false;
			GameObject newBall = (GameObject)pointList.Last()["DataBall"] as GameObject;

			if (TargetingScript.selectedTarget != null)
			{
				TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = TargetingScript.colorOff;
				TargetingScript.selectedTarget.transform.localScale += new Vector3(-0.01f, -0.01f, -0.01f);
			}

			TargetingScript.selectedTarget = newBall;
			TargetingScript.colorOff = TargetingScript.selectedTarget.GetComponent<Renderer>().material.color;
			TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = Color.white;
			TargetingScript.selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
		}

	}

	private void GetDistinctTargetFeatures()
	{
		// Add targetFeatures to a seperate list
		for (int i = 0; i < pointList.Count; i++)
			targetFeatures.Add(pointList[i][columnList[columnList.Count - 1]].ToString());

		// Only keep distinct targetFeatures
		targetFeatures = targetFeatures.Distinct().ToList();
	}

	private static void ResetDataPlot()
	{
		foreach (GameObject dataValues in GameObject.FindGameObjectsWithTag("TextValues"))
			Destroy(dataValues);

		foreach (GameObject dataValues in GameObject.FindGameObjectsWithTag("DataBall"))
			Destroy(dataValues);
	}

	public void DropdownValueChanged()
	{
		foreach (Transform child in GameObject.Find("Scatterplot").transform)
			Destroy(child.gameObject);

		PlottData();
	}

	static public void AddDataPoint(List<string> newPoint, string k, bool weightedOrNot)
	{
		K = k;
		Weighted = weightedOrNot;

		Dictionary<string, object> last = pointList.Last();

		Dictionary<string, object> newDataPoint = new Dictionary<string, object>
		{
			{ last.Keys.First().ToString(), (Convert.ToInt32(last[last.Keys.First()], CultureInfo.InvariantCulture)) + 1 }
		};

		for (int i = 0; i < ThisInstans.columnList.Count - 2; i++)
			newDataPoint.Add(ThisInstans.columnList[i + 1], newPoint[i]);

		double[] unknown = new double[newPoint.Count];

		for (int i = 0; i < newPoint.Count; ++i)
			unknown[i] = (Convert.ToDouble(newPoint[i], CultureInfo.InvariantCulture));

		var predict = dataClass.Knn(unknown, k, weightedOrNot);
		newDataPoint.Add(ThisInstans.columnList[ThisInstans.columnList.Count - 1], predict);
		pointList.Add(newDataPoint);

		ThisInstans.teleportCamera = true;
		KNN.KNNMode = true;
		ThisInstans.PlottData();
		Blink(KNN.kPoints);
		ThisInstans.KNNWindow.SetActive(true);
	}

	static public void ChangeDataPoint(string k, bool weightedOrNot)
	{
		Dictionary<string, object> KnnPoint = pointList.Last();
		pointList.Remove(KnnPoint);

		double[] unknown = new double[KnnPoint.Count - 3];

		for (int i = 0; i < KnnPoint.Count - 3; ++i)
			unknown[i] = (Convert.ToDouble(KnnPoint[ThisInstans.columnList[i + 1]], CultureInfo.InvariantCulture));

		var predict = dataClass.Knn(unknown, k, weightedOrNot);
		KnnPoint[ThisInstans.columnList.Last()] = predict;
		pointList.Add(KnnPoint);
		ThisInstans.PlottData();

	}

	static void Blink(List<int> kPoints)
	{
		foreach (int data in kPoints)
		{
			GameObject ball = (GameObject)pointList[data - 1]["DataBall"];
			ball.GetComponent<Blink>().enabled = true;
		}
	}

	#endregion
}
