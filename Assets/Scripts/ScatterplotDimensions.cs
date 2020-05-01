using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class ScatterplotDimensions : MonoBehaviour
{
    // Start is called before the first frame update
    public static List<Dictionary<string, object>> pointList;
    public int[] columns = new int[5];

    [SerializeField]
    public TMP_Text[] textList = new TMP_Text[5];
    public Dropdown[] dropDownList = new Dropdown[5];

    public float plotScale = 10;
    public GameObject PointPrefab;
    public GameObject LineSeparatorPrefab;
    public TMP_Text axisValueTextPrefab;

    private Color ColorTop = new Color(1, 1, 1, 1.0f);
    private Color ColorBottom = new Color(1, 0, 1, 1.0f);
    public GameObject GradientPlane;

    [SerializeField]
    public TMP_Text valuePrefab;

    public GameObject PointHolder;
    public List<string> columnList;
    public List<string> targetFeatures;
    public int Dimensions;

    public static ScatterplotDimensions ThisInstans;
    public static DataClass dataClass;
    private int selectedIndex = -1;
    private bool teleportCamera = false;

    // Use this for initialization
    void Start()
    {
        dataClass = CSVläsare.Read(MainMenu.fileData);
        pointList = dataClass.CSV;

        ThisInstans = this;
        columnList = new List<string>(pointList[1].Keys);

        Dimensions = columnList.Count - 2;
        if (Dimensions > 5)
            Dimensions = 5;

        for (int i = 0; i < Dimensions; i++)
        {
            dropDownList[i].AddOptions(columnList);
            dropDownList[i].value = i+1;
            dropDownList[i].onValueChanged.AddListener(delegate { DropdownValueChanged(); });
        }

        PlottData();
    }

    public void PlottData()
    {
        float[] Max = new float[Dimensions];
        float[] Min = new float[Dimensions];
        string[] nameList = new string[Dimensions];

        if (TargetingScript.selectedTarget != null)
        {
            selectedIndex = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
        }

        GameObject[] allDataBalls = GameObject.FindGameObjectsWithTag("DataBall");
        foreach (GameObject dataValues in allDataBalls)
        {
            Destroy(dataValues);
        }

        GameObject[] allAxisValueTexts = GameObject.FindGameObjectsWithTag("3D_Axis_ValueText");
        foreach (var axisValue in allAxisValueTexts)
        {
            Destroy(axisValue);
        }

        for (int i = 0; i < Dimensions; i++)
        {
            nameList[i] = columnList[dropDownList[i].value];
            textList[i].text = nameList[i];
            Min[i] = CalculationHelpers.FindMinValue(nameList[i], pointList);
            Max[i] = CalculationHelpers.FindMaxValue(nameList[i], pointList);
        }

        InstantiateDataPoint(Max, Min, nameList);

        RenderAxisValues(Max, Min);

        if (ThisInstans.teleportCamera)
        {
            ThisInstans.teleportCamera = false;
            GameObject newBall = (GameObject)pointList.Last()["DataBall"] as GameObject;
            Camera.main.transform.position = new Vector3(newBall.transform.position.x + 2.5f, newBall.transform.position.y + 1.5f, newBall.transform.position.z - 2.5f);
            Camera.main.transform.LookAt(newBall.transform);

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

    private void RenderAxisValues(float[] Max, float[] Min)
    {
        for (int i = 0; i <= 11; i++)
        {
            // Skip the first index for X-Axis because text is crowded there
            if (i != 0)
            {
                // Render X-Axis
                TMP_Text xAxisValue = Instantiate(axisValueTextPrefab, new Vector3(i, 0, -0.5f), Quaternion.Euler(90, -90, 0));
                float xValue = ((Max[0] - Min[0]) / 10) * i + Min[0];
                xAxisValue.text = xValue.ToString("0.0");
            }

            // Render Y-Axis
            TMP_Text yAxisValue = Instantiate(axisValueTextPrefab, new Vector3(0, i, -0.5f), Quaternion.Euler(0, -90, 0));
            float yValue = ((Max[1] - Min[1]) / 10) * i + Min[1];
            yAxisValue.text = yValue.ToString("0.0");

            // Render Z-Axis
            TMP_Text zAxisValue = Instantiate(axisValueTextPrefab, new Vector3(12.5f, 0, i + 0.3f), Quaternion.Euler(90, -90, -90));
            float zValue = ((Max[2] - Min[2]) / 10) * i + Min[2];
            zAxisValue.text = zValue.ToString("0.0");
        }
    }

    private void InstantiateDataPoint(float[] Max, float[] Min, string[] nameList)
    {
        for (var i = 0; i < pointList.Count; i++)
        {
            GameObject dataPoint;
            float[] floatList = new float[Dimensions];

            //Normalisering
            for (int j = 0; j < Dimensions; j++)
            {
                floatList[j] = (float.Parse(pointList[i][nameList[j]].ToString(), CultureInfo.InvariantCulture) - Min[j]) / (Max[j] - Min[j]);
            }

            //Används för ChangeTargetFeature klassen. Ta ej bort ännu. 
            if (targetFeatures.Count == 0 || !targetFeatures.Contains(pointList[i][columnList[columnList.Count - 1]].ToString()))
            {
                targetFeatures.Add(pointList[i][columnList[columnList.Count - 1]].ToString());
            }

            dataPoint = AssignDataBallAttributes_Instantiate(i, floatList);
            dataPoint.GetComponent<StoreIndexInDataBall>().Index = i;
            dataPoint.GetComponent<StoreIndexInDataBall>().TargetFeature = pointList[i][columnList[columnList.Count - 1]].ToString();
            ReselectDataball_IfDataballWasSelected(i, dataPoint);
        }
    }

    private GameObject AssignDataBallAttributes_Instantiate(int i, float[] floatList)
    {
        GameObject dataPoint = Instantiate(PointPrefab, new Vector3(floatList[0], floatList[1], floatList[2]) * plotScale, Quaternion.identity);
        if (Dimensions > 3)
        {
            dataPoint.GetComponent<Renderer>().material.color = new Color(1, floatList[3], 1, 1.0f);
        }
        if (Dimensions > 4)
            dataPoint.transform.localScale += new Vector3(floatList[4] / 2, floatList[4] / 2, floatList[4] / 2);

        dataPoint.transform.name = pointList[i][columnList[0]] + " " + pointList[i][columnList[columnList.Count() - 1]];
        dataPoint.transform.parent = PointHolder.transform;
        if (!pointList[i].ContainsKey("DataBall"))
            pointList[i].Add("DataBall", dataPoint);
        else
            pointList[i]["DataBall"] = dataPoint;
        return dataPoint;
    }

    private void ReselectDataball_IfDataballWasSelected(int i, GameObject dataPoint)
    {
        if (selectedIndex == i)
        {
            TargetingScript.selectedTarget = dataPoint;
            TargetingScript.colorOff = TargetingScript.selectedTarget.GetComponent<Renderer>().material.color;
            TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = Color.white;
            TargetingScript.selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
            selectedIndex = -1;
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

        }

        var predict = dataClass.Knn(unknown, k, weightedOrNot);
        newDataPoint.Add(ThisInstans.columnList[ThisInstans.columnList.Count - 1], predict);
        pointList.Add(newDataPoint);
        ThisInstans.teleportCamera = true;
        ThisInstans.PlottData();
        Blink(KNN.kPoints);
    }

    static void Blink(List<int> kPoints)
    {
        foreach (int data in kPoints)
        {
            GameObject ball = (GameObject)pointList[data]["DataBall"];
            ball.GetComponent<Blink>().enabled = true;
        }
    }
}
