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
	public GameObject KNNWindow;
	public static  bool KNNMode;
	public static bool KNNMove;
	public static string K;
	public static bool Weighted;
    List<string> columnListDropDown;

    public static string feature1Name;
    public static string feature2Name;
    public static string feature3Name;
    public static string feature4Name;
    public static string feature5Name;

    // Use this for initialization
    void Start()
	{
		dataClass = CSVläsare.Read(MainMenu.fileData);
		pointList = dataClass.CSV;

		ThisInstans = this;
		columnList = new List<string>(pointList[1].Keys);
		columnListDropDown = new List<string>(pointList[1].Keys);
		columnListDropDown.RemoveAt(columnList.Count() - 1);
		columnListDropDown.RemoveAt(0);

		for (int i = 0; i < dropDownList.Length - 1; i++)
		{
			dropDownList[i].AddOptions(columnListDropDown);
			dropDownList[i].value = i;
			dropDownList[i].onValueChanged.AddListener(delegate { DropdownValueChanged(); });
		}

		PlottData();
	}

	public void PlottData()
	{
		float[] Max = new float[dropDownList.Length - 1];
		float[] Min = new float[dropDownList.Length - 1];
		string[] nameList = new string[dropDownList.Length - 1];

		if (TargetingScript.selectedTarget != null)
			selectedIndex = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;

		foreach (GameObject dataValues in GameObject.FindGameObjectsWithTag("DataBall"))
			Destroy(dataValues);

		foreach (var axisValue in GameObject.FindGameObjectsWithTag("3D_Axis_ValueText"))
			Destroy(axisValue);

        feature1Name = columnListDropDown[dropDownList[0].value];
        feature2Name = columnListDropDown[dropDownList[1].value];
        feature3Name = columnListDropDown[dropDownList[2].value];
        feature4Name = columnListDropDown[dropDownList[3].value];
        feature5Name = columnListDropDown[dropDownList[4].value];

        for (int i = 0; i < dropDownList.Length -1; i++)
		{
			nameList[i] = columnList[dropDownList[i].value + 1];
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
			float[] floatList = new float[dropDownList.Length - 1];

			//Normalisering
			for (int j = 0; j < dropDownList.Length - 1; j++)
				floatList[j] = (float.Parse(pointList[i][nameList[j]].ToString(), CultureInfo.InvariantCulture) - Min[j]) / (Max[j] - Min[j]);

			if (targetFeatures.Count == 0 || !targetFeatures.Contains(pointList[i][columnList[columnList.Count - 1]].ToString()))
				targetFeatures.Add(pointList[i][columnList[columnList.Count - 1]].ToString());

			dataPoint = AssignDataBallAttributes_Instantiate(i, floatList);
			dataPoint.GetComponent<StoreIndexInDataBall>().Index = i;
			dataPoint.GetComponent<StoreIndexInDataBall>().TargetFeature = pointList[i][columnList[columnList.Count - 1]].ToString();
			ReselectDataball_IfDataballWasSelected(i, dataPoint);
            dataPoint.GetComponent<StoreIndexInDataBall>().Feature1 = feature1Name;
            dataPoint.GetComponent<StoreIndexInDataBall>().Feature2 = feature2Name;
            dataPoint.GetComponent<StoreIndexInDataBall>().Feature3 = feature3Name;
            dataPoint.GetComponent<StoreIndexInDataBall>().Feature4 = feature4Name;
            dataPoint.GetComponent<StoreIndexInDataBall>().Feature5 = feature5Name;
        }
	}

	private GameObject AssignDataBallAttributes_Instantiate(int i, float[] floatList)
	{
		GameObject dataPoint = Instantiate(PointPrefab, new Vector3(floatList[0], floatList[1], floatList[2]) * plotScale, Quaternion.identity);
		dataPoint.GetComponent<Renderer>().material.color = new Color(1, floatList[3], 0, 1.0f);
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
		KNNMode = true;
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
}
