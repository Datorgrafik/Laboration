using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class DataPlotter : MonoBehaviour
{

    // Name of the input file, no extension
    public string inputfile;


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

    // Use this for initialization
    void Start()
    {

        // Set pointlist to results of function Reader with argument inputfile
        pointList = CSVläsare.Read(inputfile);

        //Log to console
        Debug.Log(pointList);

        // Declare list of strings, fill with keys (column names)
        List<string> columnList = new List<string>(pointList[1].Keys);

        // Print number of keys (using .count)
        Debug.Log("There are " + columnList.Count + " columns in CSV");

        foreach (string key in columnList)
            Debug.Log("Column name is " + key);

        // Assign column name from columnList to Name variables
        xName = columnList[columnX];
        yName = columnList[columnY];
        zName = columnList[columnZ];

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
            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z)* plotScale, Quaternion.identity);

            // Gets material color and sets it to a new RGBA color we define
            dataPoint.GetComponent<Renderer>().material.color =
            new Color(x, y, z, 1.0f);

        }
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
}
