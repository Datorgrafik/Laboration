using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;

public class DataPlotter : MonoBehaviour
{
    // List for holding data from CSV reader
    private List<Dictionary<string, object>> pointList;

    // Indices for columns to be assigned
    public int columnX = 1;
    public int columnY = 2;
    public int columnZ = 3;

    // Full column names
    public string xName;
    public string yName;
    public string zName;

    public float plotScale = 10;
    public GameObject PointPrefab;

    public Dropdown xList;

    public Dropdown ylist;

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

        xList.onValueChanged.AddListener(delegate { DropdownValueChanged(xList); });
        ylist.AddOptions(columnList);

        ylist.onValueChanged.AddListener(delegate { DropdownValueChanged(ylist); });
        zList.AddOptions(columnList);

        zList.onValueChanged.AddListener(delegate { DropdownValueChanged(zList); });

        zList.value = 1;
        xList.value = 2;
        ylist.value = 3;

        PlottData();



    }

    private float FindMaxValue(string columnName)
    {
        //set initial value to first value
        string maxValueString = pointList[0][columnName].ToString();
        float maxValue = Single.Parse(maxValueString, CultureInfo.InvariantCulture);

        //float maxValue = Convert.ToSingle(pointList[0][columnName]);

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++)
        {
            string maxValueStringLoop = pointList[i][columnName].ToString();

            if (maxValue < Single.Parse(maxValueStringLoop, CultureInfo.InvariantCulture))
                maxValue = Single.Parse(maxValueStringLoop, CultureInfo.InvariantCulture);
        }

        //Spit out the max value
        return maxValue;
    }

    private void PlottData()
    { 

        xName = columnList[xList.value];
        yName = columnList[ylist.value];
        zName = columnList[zList.value];

        // Get maxes of each axis
        float xMax = FindMaxValue(xName);
        float yMax = FindMaxValue(yName);
        float zMax = FindMaxValue(zName);

        // Get minimums of each axis
        float xMin = FindMinValue(xName);
        float yMin = FindMinValue(yName);
        float zMin = FindMinValue(zName);
        string valueString;

        //Loop through Pointlist
        for (var i = 0; i < pointList.Count; i++)
        {
            // Get value in poinList at ith "row", in "column" Name, normalize
            valueString = pointList[i][xName].ToString();
            float x =
            (Single.Parse(valueString, CultureInfo.InvariantCulture) - xMin) / (xMax - xMin);

            valueString = pointList[i][yName].ToString();
            float y =
            (Single.Parse(valueString, CultureInfo.InvariantCulture) - yMin) / (yMax - yMin);

            valueString = pointList[i][zName].ToString();
            float z =
            (Single.Parse(valueString, CultureInfo.InvariantCulture) - zMin) / (zMax - zMin);

            //instantiate the prefab with coordinates defined above
            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);

            // Gets material color and sets it to a new RGBA color we define
            dataPoint.GetComponent<Renderer>().material.color =
            new Color(x, y, z, 1.0f);
            dataPoint.transform.parent = PointHolder.transform;

            string dataPointName = pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][yName];

            dataPoint.transform.name = dataPointName;
        }
    }

    private float FindMinValue(string columnName)
    {

        //float minValue = Convert.ToSingle(pointList[0][columnName]);

        string minValueString = pointList[0][columnName].ToString();
        float minValue = Single.Parse(minValueString, CultureInfo.InvariantCulture);

        //Loop through Dictionary, overwrite existing minValue if new value is smaller
        for (var i = 0; i < pointList.Count; i++)
        {
            string minValueStringLoop = pointList[i][columnName].ToString();

            if (Single.Parse(minValueStringLoop, CultureInfo.InvariantCulture) < minValue)
                minValue = Single.Parse(minValueStringLoop, CultureInfo.InvariantCulture);
        }

        return minValue;
    }

    public void DropdownValueChanged(Dropdown value)
    {

        GameObject ScatterPlotter = GameObject.Find("Scatterplot");
        foreach (Transform child in ScatterPlotter.transform)
        {
            Destroy(child.gameObject);
        }
        PlottData();
    }
}
