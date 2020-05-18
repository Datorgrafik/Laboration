using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using UnityEngine.Animations;

public class DataPlotter : MonoBehaviour
{
	#region Attributes

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
	public GameObject LinePrefab;
	//public GameObject LineSeparatorPrefab;

	[SerializeField]
	public TMP_Text valuePrefab;

	public Dropdown xList;
	public Dropdown yList;
	public Dropdown zList;

	public GameObject PointHolder;
	public List<string> columnList;
    public List<string> targetFeatures;

    public TMP_Text axisValueTextPrefab;

	public float xMax;
	public float yMax;
	public float zMax;
	public float xMin;
	public float yMin;
	public float zMin;

	public static DataPlotter ThisInstans;
	public static DataClass dataClass;
	private int selectedIndex = -1;
	public bool teleportCamera = false;
	public GameObject KNNWindow;

	public static string K;
	public static bool Weighted;

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

	#endregion

	#region Methods

	// Use this for initialization
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		dataClass = CSVläsare.Read(MainMenu.fileData);
		pointList = dataClass.CSV;
		ThisInstans = this;
        targetFeatures = CSVläsare.targetFeatures;
        // Declare list of strings, fill with keys (column names)
        columnList = new List<string>(pointList[1].Keys);
        List<string> features = columnList.GetRange(1, columnList.Count - 2);

		// Assign column name from columnList to Name variables
		xList.AddOptions(features);
		xList.value = 1;
		xList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		yList.AddOptions(features);
		yList.value = 2;
		yList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });

		if (MainMenu.renderMode == 1)
		{
			zList.AddOptions(features);
			zList.value = 3;
			zList.onValueChanged.AddListener(delegate { DropdownValueChanged(); });
		}

		PlottData();
	}
	private void Update()
	{
		if (KNN.KNNMode && KNN.KNNMove)
		{
			ChangeDataPoint(K, Weighted);
			KNN.KNNMove = false;
		}
	}

	public void PlottData()
	{
		if (TargetingScript.selectedTarget != null)
			selectedIndex = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;

		DestroyDataBallsAndAxisValues();

		xName = columnList[xList.value+1];
		yName = columnList[yList.value+1];

		xAxisText.text = xName;
		yAxisText.text = yName;

		// Get maxes of each axis
		xMax = CalculationHelpers.FindMaxValue(xName, pointList);
		yMax = CalculationHelpers.FindMaxValue(yName, pointList);
		zMax = 0f;

		// Get minimums of each axis
		xMin = CalculationHelpers.FindMinValue(xName, pointList);
		yMin = CalculationHelpers.FindMinValue(yName, pointList);
		zMin = 0f;

		// If renderMode is 3D
		if (MainMenu.renderMode == 1)
		{
			zName = columnList[zList.value+1];
			zAxisText.text = zName;
			zMax = CalculationHelpers.FindMaxValue(zName, pointList);
			zMin = CalculationHelpers.FindMinValue(zName, pointList);
		}

		string valueString;

		// If renderMode is 2D
		if (MainMenu.renderMode == 0)
		{
			// Destroy all dataValues before plotting new ones
			foreach (GameObject dataValues in GameObject.FindGameObjectsWithTag("dataValues"))
				Destroy(dataValues);

			DrawBackgroundGridAndValues();
		}

		// If renderMode is 3D
		if (MainMenu.renderMode == 1)
			RenderAxisValues();

		// Loop through Pointlist
		for (var i = 0; i < pointList.Count; i++)
		{
			GameObject dataPoint;

			// Get value in poinList at ith "row", in "column" Name, normalize
			valueString = pointList[i][xName].ToString();
			float x = (float.Parse(valueString, CultureInfo.InvariantCulture) - xMin) / (xMax - xMin);

			valueString = pointList[i][yName].ToString();
			float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - yMin) / (yMax - yMin);

			float z = 1;

			//Instantiate datapoints
			if (MainMenu.renderMode == 0)
			{
				dataPoint = Instantiate(PointPrefab, new Vector3(x, y, 0) * plotScale, Quaternion.identity);
				dataPoint.transform.name = pointList[i][columnList[0]] + " " + pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][columnList[columnList.Count() - 1]];
				dataPoint.transform.parent = PointHolder.transform;
			}
			else
			{
				valueString = pointList[i][zName].ToString();
				z = (float.Parse(valueString, CultureInfo.InvariantCulture) - zMin) / (zMax - zMin);
				dataPoint = Instantiate(PointPrefab, new Vector3(x, y, z) * plotScale, Quaternion.identity);
				dataPoint.transform.name = pointList[i][columnList[0]] + " " + pointList[i][xName] + " " + pointList[i][yName] + " " + pointList[i][zName] + " " + pointList[i][columnList[columnList.Count() - 1]];
				dataPoint.transform.parent = PointHolder.transform;

			}

			if (!pointList[i].ContainsKey("DataBall"))
				pointList[i].Add("DataBall", dataPoint);
			else
				pointList[i]["DataBall"] = dataPoint;

			//Store values in dataPoint
			dataPoint.GetComponent<StoreIndexInDataBall>().Index = i;
			dataPoint.GetComponent<StoreIndexInDataBall>().TargetFeature = pointList[i][columnList[columnList.Count - 1]].ToString();

			//Assign color to dataPoint
			int index = targetFeatures.IndexOf(pointList[i][columnList[columnList.Count - 1]].ToString());
			if (targetFeatures.Count() <= 10)
				ChangeColor(dataPoint, index);
			else
				dataPoint.GetComponent<Renderer>().material.color = new Color(x, y, z, 1.0f);

			//Reselect target if one was selected before.
			if (selectedIndex == i)
			{
				TargetingScript.selectedTarget = dataPoint;
				TargetingScript.colorOff = TargetingScript.selectedTarget.GetComponent<Renderer>().material.color;
				TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = Color.white;
				TargetingScript.selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
				selectedIndex = -1;
			}
		}

		//Focus camera on new dataPoint
		if (ThisInstans.teleportCamera)
		{

			//ThisInstans.teleportCamera = false;
			GameObject newBall = (GameObject)pointList.Last()["DataBall"] as GameObject;
			if (MainMenu.renderMode == 1)
				Camera.main.transform.position = new Vector3(newBall.transform.position.x + 2.5f, newBall.transform.position.y + 1.5f, newBall.transform.position.z - 2.5f);
			else
				Camera.main.transform.position = new Vector3(newBall.transform.position.x, newBall.transform.position.y, newBall.transform.position.z - 8f);

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
		if (KNN.kPoints != null)
			if (KNN.kPoints.Count > 0)
				Blink(KNN.kPoints);
	}

	private static void DestroyDataBallsAndAxisValues()
	{
		// Destroy all DataBalls before rendering new plot
		foreach (GameObject dataValues in GameObject.FindGameObjectsWithTag("DataBall"))
			Destroy(dataValues);

		// Destroy axisValues before rendering new plot
		foreach (var axisValue in GameObject.FindGameObjectsWithTag("3D_Axis_ValueText"))
			Destroy(axisValue);
	}

	private void DrawBackgroundGridAndValues()
	{
		for (int i = 0; i < 11; i++)
		{
			// Draw vertical lines
			GameObject xLine = Instantiate(LinePrefab, new Vector3(i, 0, -0.001f), Quaternion.identity);
			xLine.transform.parent = PointHolder.transform;
			xLine.transform.name = $"Column{i}Line";

			LineRenderer xLineRenderer = xLine.GetComponent<LineRenderer>();
			// Number of vertices
			xLineRenderer.positionCount = 2;
			// Set line width
			xLineRenderer.startWidth = 0.025f;
			xLineRenderer.endWidth = 0.025f;
			// Set start & end position
			xLineRenderer.SetPosition(0, new Vector3(i, -0.5f, -0.001f));
			xLineRenderer.SetPosition(1, new Vector3(i, 10.5f, -0.001f));
			// Set line color
			xLineRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);


			// Draw horizontal lines
			GameObject yLine = Instantiate(LinePrefab, new Vector3(0, 0f, -0.001f), Quaternion.identity);
			yLine.transform.parent = PointHolder.transform;
			yLine.transform.name = $"Value{i}Line";

			LineRenderer yLineRenderer = yLine.GetComponent<LineRenderer>();
			// Number of vertices
			yLineRenderer.positionCount = 2;
			// Set line width
			yLineRenderer.startWidth = 0.025f;
			yLineRenderer.endWidth = 0.025f;
			// Set start & end position
			yLineRenderer.SetPosition(0, new Vector3(-0.5f, i, -0.001f));
			yLineRenderer.SetPosition(1, new Vector3(10.5f, i, -0.001f));
			// Set line color
			yLineRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);


			// Draw axis values
			TMP_Text valuePointX = Instantiate(valuePrefab, new Vector3(i, -1, 0), Quaternion.identity);
			TMP_Text valuePointY = Instantiate(valuePrefab, new Vector3(-1.5f, i, 0), Quaternion.identity);

			float xValue = ((xMax - xMin) / 10) * i + xMin;
			float yValue = ((yMax - yMin) / 10) * i + yMin;

			valuePointX.text = xValue.ToString("0.00");
			valuePointY.text = yValue.ToString("0.00");
		}
	}

	private void RenderAxisValues()
	{
		for (int i = 0; i <= 11; i++)
		{
			// Skip the first index for X-Axis because text is crowded there
			if (i != 0)
			{
				// Render X-Axis
				TMP_Text xAxisValue = Instantiate(axisValueTextPrefab, new Vector3(i, 0, -0.5f), Quaternion.Euler(90, -90, 0));
				float xValue = ((xMax - xMin) / 10) * i + xMin;
				xAxisValue.text = xValue.ToString("0.0");
			}

			// Render Y-Axis
			TMP_Text yAxisValue = Instantiate(axisValueTextPrefab, new Vector3(0, i, -0.5f), Quaternion.Euler(0, -90, 0));
			float yValue = ((yMax - yMin) / 10) * i + yMin;
			yAxisValue.text = yValue.ToString("0.0");

			// Render Z-Axis
			TMP_Text zAxisValue = Instantiate(axisValueTextPrefab, new Vector3(12.5f, 0, i + 0.3f), Quaternion.Euler(90, -90, -90));
			float zValue = ((zMax - zMin) / 10) * i + zMin;
			zAxisValue.text = zValue.ToString("0.0");
		}
	}

	public static void ChangeColor(GameObject dataPoint, int targetFeatureIndex)
	{
		dataPoint.GetComponent<Renderer>().material.color = new Color(colorList[targetFeatureIndex].r / 255, colorList[targetFeatureIndex].g / 255, colorList[targetFeatureIndex].b / 255, 1.0f);
	}

	public void DropdownValueChanged()
	{
		PlottData();
	}

	public static void AddDataPoint(List<string> newPoint, string k, bool weightedOrNot)
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

		ThisInstans.PlottData();
		Blink(KNN.kPoints);
		KNN.KNNMode = true;
		ThisInstans.KNNWindow.SetActive(true);
	}

	public static void ChangeDataPoint(string k, bool weightedOrNot)
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
