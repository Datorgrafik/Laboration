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
    public GameObject dataLine;
    public Material lineColor;
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
        //column1.AddOptions(columnList);
        //column1.onValueChanged.AddListener(delegate { DropdownValueChanged(1); });

        //column2.AddOptions(columnList);
        //column2.onValueChanged.AddListener(delegate { DropdownValueChanged(2); });

        //column3.AddOptions(columnList);
        //column3.onValueChanged.AddListener(delegate { DropdownValueChanged(3); });

        //column4.AddOptions(columnList);
        //column4.onValueChanged.AddListener(delegate { DropdownValueChanged(4); });

        // Default values for each column at start
        column1.value = 1;
        column2.value = 2;
        column3.value = 3;
        column4.value = 4;

        column1Text.text = columnList[1];
        column2Text.text = columnList[2];
        column3Text.text = columnList[3];
        column4Text.text = columnList[4];

        PlotData2();

        // Run the default startup plot
        //for (int i = 1; i <= 4; i++)
        //{
        //	PlotData(i, i);
        //}
    }

    private void PlotData(int column, int columnPos)
    {
        columnName = columnList[column];

        // Sets the correct X-Axis position for each column
        float xPos = float.MinValue;
        if (columnPos == 1)
            xPos = 0.2f;
        else if (columnPos == 2)
            xPos = 0.6f;
        else if (columnPos == 3)
            xPos = 1.0f;
        else if (columnPos == 4)
            xPos = 1.4f;

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
            dataPoint.GetComponent<Renderer>().material.color = new Color(1.0f, y, 1.0f, 1.0f);

            dataPoint.transform.parent = PointHolder.transform;
            string dataPointName = Convert.ToString("Column" + columnPos + ": " + i + 1);
            dataPoint.transform.name = dataPointName;
        }

        for (int i = 0; i < pointList.Count; i++)
        {
            // LineRenderer
            GameObject line = new GameObject();
            LineRenderer lineRenderer = line.AddComponent<LineRenderer>();
            lineRenderer.positionCount = 4;
        }
    }

    private void PlotData2()
    {
        // Sets the columnName for each column
        string column1Name = columnList[1];
        string column2Name = columnList[2];
        string column3Name = columnList[3];
        string column4Name = columnList[4];

        // Sets the correct X-Axis position for each column
        float xPos1 = 0.2f;
        float xPos2 = 0.6f;
        float xPos3 = 1.0f;
        float xPos4 = 1.4f;

        // Get MaxValues
        float column1Max = FindMaxValue(column1Name);
        float column2Max = FindMaxValue(column2Name);
        float column3Max = FindMaxValue(column3Name);
        float column4Max = FindMaxValue(column4Name);
        // Get MinValues
        float column1Min = FindMinValue(column1Name);
        float column2Min = FindMinValue(column2Name);
        float column3Min = FindMinValue(column3Name);
        float column4Min = FindMinValue(column4Name);

        List<string> targetFeatures = new List<string>();
        for (int i = 0; i < pointList.Count; i++)
        {
            targetFeatures.Add(pointList[i][columnList[5]].ToString());
        }

        targetFeatures = targetFeatures.Distinct().ToList();
        Color targetColor;

        //Loop through Pointlist
        for (var i = 0; i < pointList.Count; i++)
        {
            targetColor = SetColors(targetFeatures, i);

            // Column 1
            // Get original value
            string column1ValueString = pointList[i][column1Name].ToString();
            // Set normalized Y-value
            float column1Y = (float.Parse(column1ValueString, CultureInfo.InvariantCulture) - column1Min) / (column1Max - column1Min);
            // Create clone
            GameObject dataPoint = Instantiate(PointPrefab, new Vector3(xPos1, column1Y, 0) * plotScale, Quaternion.identity);
            // Set color
            dataPoint.GetComponent<Renderer>().material.color = targetColor;
            // Set parent
            dataPoint.transform.parent = PointHolder.transform;
            // Set name
            dataPoint.transform.name = Convert.ToString(pointList[i][column1Name]);


            // LineRenderer
            // Instantiate the line
            dataLine = Instantiate(dataLine, new Vector3(xPos1, column1Y, -0.001f) * plotScale, Quaternion.identity);
            // Set parent
            dataLine.transform.parent = PointHolder.transform;
            // Set name
            dataLine.transform.name = Convert.ToString("Line " + (i + 1));
            // Get the LineRenderer
            LineRenderer lineRenderer = dataLine.GetComponent<LineRenderer>();
            // Set line position
            lineRenderer.SetPosition(0, new Vector3(xPos1, column1Y, -0.001f) * plotScale);
            // Set line color
            lineRenderer.material.color = targetColor;


            // Column 2
            // Get original value
            string column2ValueString = pointList[i][column2Name].ToString();
            // Set normalized Y-value
            float column2Y = (float.Parse(column2ValueString, CultureInfo.InvariantCulture) - column2Min) / (column2Max - column2Min);
            // Create clone
            dataPoint = Instantiate(PointPrefab, new Vector3(xPos2, column2Y, 0) * plotScale, Quaternion.identity);
            // Set color
            dataPoint.GetComponent<Renderer>().material.color = targetColor;
            // Set parent
            dataPoint.transform.parent = PointHolder.transform;
            // Set name
            dataPoint.transform.name = Convert.ToString(pointList[i][column2Name]);
            // Set line position
            lineRenderer.SetPosition(1, new Vector3(xPos2, column2Y, -0.001f) * plotScale);


            // Column 3
            // Get original value
            string column3ValueString = pointList[i][column3Name].ToString();
            // Set normalized Y-value
            float column3Y = (float.Parse(column3ValueString, CultureInfo.InvariantCulture) - column3Min) / (column3Max - column3Min);
            // Create clone
            dataPoint = Instantiate(PointPrefab, new Vector3(xPos3, column3Y, 0) * plotScale, Quaternion.identity);
            // Set color
            dataPoint.GetComponent<Renderer>().material.color = targetColor;
            // Set parent
            dataPoint.transform.parent = PointHolder.transform;
            // Set name
            dataPoint.transform.name = Convert.ToString(pointList[i][column3Name]);
            // Set line position
            lineRenderer.SetPosition(2, new Vector3(xPos3, column3Y, -0.001f) * plotScale);


            // Column 4
            // Get original value
            string column4ValueString = pointList[i][column4Name].ToString();
            // Set normalized Y-value
            float column4Y = (float.Parse(column4ValueString, CultureInfo.InvariantCulture) - column4Min) / (column4Max - column4Min);
            // Create clone
            dataPoint = Instantiate(PointPrefab, new Vector3(xPos4, column4Y, 0) * plotScale, Quaternion.identity);
            // Set color
            dataPoint.GetComponent<Renderer>().material.color = targetColor;
            // Set parent
            dataPoint.transform.parent = PointHolder.transform;
            // Set name
            dataPoint.transform.name = Convert.ToString(pointList[i][column4Name]);
            // Set line position
            lineRenderer.SetPosition(3, new Vector3(xPos4, column4Y, -0.001f) * plotScale);


        }
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

    private void DropdownValueChanged(int column)
    {
        // Find the changed column and destroy it.
        GameObject parallelCoordinatePlot = GameObject.Find("ParallelCoordinatePlot");

        foreach (Transform child in parallelCoordinatePlot.transform)
        {
            Destroy(child.gameObject);
        }

        if (column == 1)
            column1Text.text = columnList[column1.value];
        else if (column == 2)
            column2Text.text = columnList[column2.value];
        else if (column == 3)
            column3Text.text = columnList[column3.value];
        else if (column == 4)
            column4Text.text = columnList[column4.value];

        // Run PlotData() with the selected columns.
        PlotData(column1.value, 1);
        PlotData(column2.value, 2);
        PlotData(column3.value, 3);
        PlotData(column4.value, 4);
    }

}
