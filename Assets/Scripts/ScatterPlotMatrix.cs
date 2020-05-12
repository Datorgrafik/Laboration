using System.Collections;
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
    private static Color[] colorList = { new Color(52, 152, 219, 1), new Color(192, 57, 43,1), new Color(46, 204, 113,1), new Color(26, 188, 156,1), new Color(155, 89, 182,1),
                                         new Color(52, 73, 94,1), new Color(241, 196, 15,1), new Color(230, 126, 34,1), new Color(189, 195, 199,1), new Color(149, 165, 166,1)};

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
														new Vector3(k * 1.2F + 1, j * 1.2F + 0.3F, -0.01f) * plotScale,
														Quaternion.identity);

						textField.text = feature2Name;
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

                            // Set color
                            if (targetFeatures.Count() <= 10)
                            {
                                dataPoint.GetComponent<Renderer>().material.color = new Color(colorList[index].r / 255, colorList[index].g / 255, colorList[index].b / 255, 1.0f);
                            }
                            else
                                dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, y, 1.0f);
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

	#endregion
}
