using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;
using TMPro;

public class ScatterPlotMatrix : MonoBehaviour
{
    // List for holding data from CSV reader
    public static List<Dictionary<string, object>> pointList;

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

    public Dropdown feature1;
    public Dropdown feature2;
    public Dropdown feature3;
    public Dropdown feature4;
    private static Dropdown[] features;

    public GameObject PointHolder;
    public GameObject planePointBackground;
    public TMP_Text ScatterplotMatrixText;
    public List<string> columnList;
    public List<string> targetFeatures;

    // Use this for initialization
    void Start()
    {
        // Set pointlist to results of function Reader with argument inputfile
        Debug.Log(MainMenu.fileData);
        DataClass dataClass = CSVläsare.Read(MainMenu.fileData);
        pointList = dataClass.CSV;
        //Log to console
        Debug.Log(pointList);

        // Declare list of strings, fill with keys (column names)
        columnList = new List<string>(pointList[1].Keys);

        // Print number of keys (using .count)
        //Debug.Log("There are " + columnList.Count + " columns in CSV");

        //foreach (string key in columnList)
        //    Debug.Log("Column name is " + key);

        // Assign column name from columnList to Name variables
        feature1.AddOptions(columnList);
        feature1.value = 1;
        feature1.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

        feature2.AddOptions(columnList);
        feature2.value = 2;
        feature2.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

        feature3.AddOptions(columnList);
        feature3.value = 3;
        feature3.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

        feature4.AddOptions(columnList);
        feature4.value = 4;
        feature4.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

        features = new Dropdown[4];
        features[0] = feature1;
        features[1] = feature2;
        features[2] = feature3;
        features[3] = feature4;

        PlottData();

        Camera.main.transform.position = new Vector3(19.3F, 22.5F, -45.7F);
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

    private void PlottData()
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

        for (int j = 0; j < 4; j++)
        {
            for (int k = 0; k < 4; k++)
            {
                try
                {
                    GameObject summonPlane = Instantiate(planePointBackground, new Vector3(k * 1.2F + 0.5F, j * 1.2F + 0.5F, 0) * plotScale, Quaternion.Euler(0, 90, -90));
                    feature1Name = columnList[features[j].value];
                    feature2Name = columnList[features[k].value];
                    

                    if (j == k)
                    {
                        //Le textfält
                        TMP_Text textField = Instantiate(ScatterplotMatrixText, new Vector3(k * 1.2F + 1, j * 1.2F + 0.3F, 0) * plotScale, Quaternion.identity);
                        textField.text = feature2Name;
                    }
                    else
                    {
                        // Get maxes of each axis
                        float xMax = FindMaxValue(feature1Name);
                        float yMax = FindMaxValue(feature2Name);

                        // Get minimums of each axis
                        float xMin = FindMinValue(feature1Name);
                        float yMin = FindMinValue(feature2Name);

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

                            //Lägger in alla targetfeatures i en lista
                            if (targetFeatures.Count == 0 || !targetFeatures.Contains(pointList[i][columnList[columnList.Count - 1]].ToString()))
                            {
                                targetFeatures.Add(pointList[i][columnList[columnList.Count - 1]].ToString());
                            }

                            float index = targetFeatures.IndexOf(pointList[i][columnList[columnList.Count - 1]].ToString());
                            float colorValue = 1 / (index + 1);

                            dataPoint = Instantiate(PointPrefab, new Vector3(x + k * 1.2F, y + j * 1.2F, 0) * plotScale, Quaternion.identity);
                            dataPoint.transform.name = pointList[i][feature1Name] + " " + pointList[i][feature2Name];
                            dataPoint.transform.parent = PointHolder.transform;
                            dataPoint.GetComponent<StoreIndexInDataBall>().Index = i;
                            dataPoint.GetComponent<StoreIndexInDataBall>().TargetFeature = pointList[i][columnList[columnList.Count - 1]].ToString();

                            if (index % 3 == 0)
                            {
                                dataPoint.GetComponent<Renderer>().material.color = new Color(0, colorValue, 0, 1.0f);
                            }
                            else if (index % 3 == 1)
                            {
                                dataPoint.GetComponent<Renderer>().material.color = new Color(1, 0, colorValue, 1.0f);

                            }
                            else if (index % 3 == 2)
                            {
                                dataPoint.GetComponent<Renderer>().material.color = new Color(colorValue, 0, 1, 1.0f);

                            }
                        }
                    }
                }
                catch
                {

                }
            }
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
}
