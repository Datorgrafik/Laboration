using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.UI;

public class ParallelCoordinatePlotter : MonoBehaviour
{
	#region Attributes

	// List for holding data from CSV reader
	public List<Dictionary<string, object>> pointList;

	// Lists
	public List<string> columnList;
	public List<string> featureList;
	private List<string> targetFeatures = new List<string>();

	// GameObjects
	public GameObject PointPrefab;
	public GameObject LinePrefab;
	public GameObject TargetFeaturePrefab;
	public GameObject PointHolder;

	// AddNewData Attributes
	[SerializeField]
	private GameObject newDataPanel;
	[SerializeField]
	private GameObject newDataContainer;
	[SerializeField]
	private GameObject kAndWeightedPrefab;
	[SerializeField]
	private GameObject newDataInputFieldPrefab;
	[SerializeField]
	private GameObject saveAndCancelButtonsPrefab;

	public static DataClass dataClass;

	// Misc
	public float plotScale = 10;
	public TMP_Text valuePrefab;
	private Color targetColor;
	public string columnName;
	private int nFeatures;
	public float yMax;
	public float yMin;

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

	// PlotColumns
	public List<Dropdown> columnDropdownList = new List<Dropdown>();

	// Column Text Fields
	public List<TMP_Text> columnTextList = new List<TMP_Text>();

	// EditPosition Attributes
	public GameObject EditPanel;
	public TMP_Text ChangePanelColumnText;
	public TMP_Text ChangePanelColumnValueText;
	public TMP_InputField ChangePanelColumnInputfield;
	private string newValue;

	// NewData Attributes
	string kValue;
	bool weighted;

	// KNN Attributes
	public static bool KNNMode = false;
	public static bool KNNMove = false;
	public GameObject KNNWindow;

	public static ParallelCoordinatePlotter ThisInstans;

	#endregion


	#region Methods

	// Start is called before the first frame update
	void Start()
	{
		ThisInstans = this;

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

		// Set correct number of vertices in LinePrefab depending on dataset
		if (nFeatures <= 4)
			LinePrefab.GetComponent<LineRenderer>().positionCount = nFeatures;
		else
			LinePrefab.GetComponent<LineRenderer>().positionCount = 4;

		AddDropdownListeners();

		// Default values for each columntext at start, depending on nFeatures (max 4)
		for (int i = 0; i < nFeatures; i++)
		{
			columnTextList[i].text = featureList[i];

			if (i + 1 == 4)
				break;
		}

		GetDistinctTargetFeatures();

		InstantiateTargetFeatureList();

		DrawBackgroundGrid();

		// Run the default startup plot
		for (int i = 0; i < nFeatures; i++)
		{
			PlotData(i, i + 1);

			if (i + 1 == 4)
				break;
		}
	}

	// Update is called once per frame
	private void Update()
	{
		// Codeblock for EditPosition
		if (TargetingScript.selectedTarget != null)
		{
			EditPanel.SetActive(true);
			ChangePanelColumnText.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;
			Denormalize();
		}
		else
			EditPanel.SetActive(false);

		// Codeblock for KNN
		if (KNNMode && KNNMove)
		{
			ChangeDataPoint(kValue, weighted);
			KNNMove = false;
		}
	}

	private void AddDropdownListeners()
	{
		// Assign column name from columnList to Name variables
		for (int i = 0; i < nFeatures; i++)
		{
			// iLocal for Listener reference
			int iLocal = i;

			columnDropdownList[i].AddOptions(featureList);
			columnDropdownList[i].value = i;

			columnDropdownList[iLocal].onValueChanged
				.AddListener(delegate 
				{
					columnTextList[iLocal].text = featureList[columnDropdownList[iLocal].value];
				});

			if (i + 1 == 4)
				break;
		}
	}

	public void DrawBackgroundGrid()
	{
		foreach (GameObject dataValues in GameObject.FindGameObjectsWithTag("dataValues"))
			Destroy(dataValues);
		foreach (GameObject dataValues in GameObject.FindGameObjectsWithTag("DataLineGrid"))
			Destroy(dataValues);

		// Draw vertical lines
		for (int i = 1; i <= 4; i++)
		{
			float xPos = SetColumnPosition(i);

			GameObject xLine = Instantiate(LinePrefab, new Vector3(xPos, 0f, -0.001f) * plotScale, Quaternion.identity);
			xLine.transform.tag = "DataLineGrid";
			xLine.transform.parent = PointHolder.transform;
			xLine.transform.name = $"Column{i}Line";

			LineRenderer xLineRenderer = xLine.GetComponent<LineRenderer>();
			xLineRenderer.positionCount = 2;
			xLineRenderer.startWidth = 0.025f;
			xLineRenderer.endWidth = 0.025f;
			xLineRenderer.SetPosition(0, new Vector3(xPos, -0.05f, -0.001f) * plotScale);
			xLineRenderer.SetPosition(1, new Vector3(xPos, 1.05f, -0.001f) * plotScale);
			xLineRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
		}

		yMax = 0f;
		yMin = float.Parse(pointList[0][featureList[0]].ToString(), CultureInfo.InvariantCulture);

		// Find and render Max- & Min-values on Y-Axis
		for (int i = 0; i < featureList.Count; i++)
		{
			float yMaxTempValue = CalculationHelpers.FindMaxValue(featureList[i], pointList);
			if (yMaxTempValue > yMax)
				yMax = yMaxTempValue;

			float yMinTempValue = CalculationHelpers.FindMinValue(featureList[i], pointList);
			if (yMinTempValue < yMin)
				yMin = yMinTempValue;
		}

		// Draw horizontal lines
		for (int i = 0; i <= 10; i++)
		{
			// Instantiate lines, set parent, set transform name
			GameObject yLine = Instantiate(LinePrefab, new Vector3(0, 0f, -0.001f) * plotScale, Quaternion.identity);
			yLine.transform.tag = "DataLineGrid";
			yLine.transform.parent = PointHolder.transform;
			yLine.transform.name = $"Value{i}Line";

			LineRenderer yLineRenderer = yLine.GetComponent<LineRenderer>();
			yLineRenderer.positionCount = 2;
			yLineRenderer.startWidth = 0.025f;
			yLineRenderer.endWidth = 0.025f;
			yLineRenderer.SetPosition(0, new Vector3(0.15f, (float)i / 10, -0.001f) * plotScale);
			yLineRenderer.SetPosition(1, new Vector3(1.45f, (float)i / 10, -0.001f) * plotScale);
			yLineRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);

			TMP_Text valuePointY = Instantiate(valuePrefab, new Vector3(1f, 0 + i, 0), Quaternion.identity);
			valuePointY.transform.name = $"Value{i}LineText";

			float yValue = ((yMax - yMin) / 10) * i + yMin;

			valuePointY.text = yValue.ToString("0.00");
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

	private void InstantiateTargetFeatureList()
	{
		float targetXpos = 17f;
		float targetYpos = 8f;
		float targetZpos = 0f;
		Color targetColor;

		// Instantiate a list of targetFeatures to the side of the plot
		foreach (var targetFeature in targetFeatures)
		{
			// Instantiate targetFeaturePoint and name it
			GameObject targetFeaturePoint = Instantiate(TargetFeaturePrefab, new Vector3(targetXpos, targetYpos, targetZpos), Quaternion.identity);
			targetFeaturePoint.name = targetFeature;

			int index = targetFeatures.IndexOf(targetFeature.ToString());
			// Set correct color
			targetColor = SetColors(targetFeatures, index);

			// Set color and text
			targetFeaturePoint.GetComponentInChildren<Renderer>().material.color = targetColor;
			TextMeshPro text = targetFeaturePoint.GetComponentInChildren<TextMeshPro>();
			text.text = targetFeature;
			text.color = targetColor;

			// Change Y-Position for next targetFeature in the loop
			targetYpos -= 1f;
		}
	}

	private void PlotData(int column, int columnPos)
	{
		columnName = featureList[column];

		// Sets the correct X-Axis position for each column
		float xPos = SetColumnPosition(columnPos);

		//Loop through Pointlist & Render dataset
		for (var i = 0; i < pointList.Count; i++)
		{
			int index = targetFeatures.IndexOf(pointList[i][columnList[columnList.Count - 1]].ToString());
			
			// Set correct color
			targetColor = SetColors(targetFeatures, index);

			// Get original value
			string valueString = pointList[i][columnName].ToString();
			// Set normalized Y-value
			float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - yMin) / (yMax - yMin);

			InstantiateAndRenderDatapoints(xPos, i, y, columnPos);

			InstantiateAndRenderLines(columnPos, xPos, i, y);
		}
	}

	private void InstantiateAndRenderDatapoints(float xPos, int i, float y, int columnPos)
	{
		// Create clone
		GameObject dataPoint = Instantiate(PointPrefab, new Vector3(xPos, y, 0) * plotScale, Quaternion.identity);
		// Set color
		dataPoint.GetComponent<Renderer>().material.color = targetColor;
		// Set new scale
		dataPoint.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		// Set parent
		dataPoint.transform.parent = PointHolder.transform;
		// Set name
		dataPoint.transform.name = Convert.ToString($"Point {i+1}.{columnPos}");

		// Add dataPoints as DataBalls to pointList
		if (!pointList[i].ContainsKey($"DataBall{columnPos}"))
			pointList[i].Add($"DataBall{columnPos}", dataPoint);
		else
			pointList[i][$"DataBall{columnPos}"] = dataPoint;

		//Store Index
		dataPoint.GetComponent<StoreIndexInDataBall>().Index = i;
		dataPoint.GetComponent<StoreIndexInDataBall>().TargetFeature = featureList[columnDropdownList[columnPos - 1].value];
	}

	private void InstantiateAndRenderLines(int columnPos, float xPos, int i, float y)
	{
		GameObject dataLine;
		LineRenderer lineRenderer;

		// Instantiate LineRenderer at first column for every point
		if (columnPos == 1 && GameObject.Find("Line " + (i + 1)) == null)
		{
			// Instantiate the line
			dataLine = Instantiate(LinePrefab, new Vector3(xPos, y, -0.001f) * plotScale, Quaternion.identity);
			// Set parent
			dataLine.transform.parent = PointHolder.transform;
			// Set name
			dataLine.transform.name = Convert.ToString("Line " + (i + 1));
			// Get the LineRenderer
			lineRenderer = dataLine.GetComponent<LineRenderer>();
			// Set line color
			lineRenderer.material.color = targetColor;
		}
		else
		{
			// Find the correct dataLine
			dataLine = GameObject.Find("Line " + (i + 1));
			// Get the LineRenderer
			lineRenderer = dataLine.GetComponent<LineRenderer>();
		}

		// Set line position
		lineRenderer.SetPosition((columnPos - 1), new Vector3(xPos, y, -0.001f) * plotScale);
	}

	private float SetColumnPosition(int columnPos)
	{
		float xPos = float.MinValue;

		if (columnPos == 1)
			xPos = 0.2f;
		else if (columnPos == 2)
			xPos = 0.6f;
		else if (columnPos == 3)
			xPos = 1.0f;
		else if (columnPos == 4)
			xPos = 1.4f;

		return xPos;
	}

	private Color SetColors(List<string> targetFeatures, int index)
	{
		Color targetColor;
		float colorValue = 1 / (index + 1);

		if (targetFeatures.Count() <= 10)
		{
			return new Color(colorList[index].r / 255,
							colorList[index].g / 255,
							colorList[index].b / 255, 1.0f);
		}
		else
		{
			if (index % 3 == 0)
				targetColor = new Color(0, colorValue, 0);
			else if (index % 3 == 1)
				targetColor = new Color(0, 0, colorValue);
			else if (index % 3 == 2)
				targetColor = new Color(colorValue, 0, 0);
			else
				targetColor = Color.black;

			return targetColor;
		}
	}

	public void ReorderColumns()
	{
		// Destroy Datapoints
		foreach (var databall in GameObject.FindGameObjectsWithTag("DataBall"))
			Destroy(databall);

		// Plot data for the selected columns
		for (int i = 0; i < nFeatures; i++)
		{
			PlotData(columnDropdownList[i].value, i+1);

			if (i + 1 == 4)
				break;
		}
	}


	#region AddNewData Methods

	public void InputNewData()
	{
		// Set newDataPanel to active
		newDataPanel.SetActive(true);

		// Starting Y Position for inputFields
		float inputFieldYAxis = -17.5f;

		// Instantiate kAndWeighted prefab
		GameObject kAndWeighted = Instantiate(kAndWeightedPrefab, new Vector2(115, inputFieldYAxis), Quaternion.identity);
		// Set parent
		kAndWeighted.transform.SetParent(newDataContainer.transform, false);
		inputFieldYAxis -= 35;

		// Populate newDataPanel with inputFields for each attribute in dataset
		for (int i = 0; i < nFeatures; i++)
		{
			// Instatiate newDataPrefabs
			GameObject newInput = Instantiate(newDataInputFieldPrefab, new Vector2(115, inputFieldYAxis), Quaternion.identity);
			inputFieldYAxis -= 62;
			// Set parent
			newInput.transform.SetParent(newDataContainer.transform, false);
			// Get attribute textfield
			newInput.GetComponentInChildren<TMP_Text>().text = featureList[i];
		}

		// Instantiate SaveAndCancel buttons
		GameObject saveAndCancelButtons = Instantiate(saveAndCancelButtonsPrefab, new Vector2(115, inputFieldYAxis), Quaternion.identity);
		// Set parent
		saveAndCancelButtons.transform.SetParent(newDataContainer.transform, false);
		// Add onClick listener to saveButton
		saveAndCancelButtons.transform.GetChild(0).gameObject.GetComponent<Button>().onClick.AddListener(Save);
		// Add onClick listener to cancelButton
		saveAndCancelButtons.transform.GetChild(1).gameObject.GetComponent<Button>().onClick.AddListener(Cancel);

		// While newDataPanel shows, newDataButton is none-Interactable
		GameObject.FindGameObjectWithTag("PCPNewDataButton").GetComponent<Button>().interactable = false;
	}

	private void Save()
	{
		// Create a list to hold new dataValues
		List<string> newDataInputList = new List<string>();

		// Get list of values from newData InputFields
		foreach (GameObject dataInput in GameObject.FindGameObjectsWithTag("PCPNewDataInputField"))
		{
			newDataInputList.Add(dataInput.GetComponent<TMP_InputField>().text);
			dataInput.GetComponent<TMP_InputField>().text = null;
		};

		// Get kValue InputField and weighted Toggle
		kValue = GameObject.FindGameObjectWithTag("PCPkValue").GetComponent<TMP_InputField>().text;
		weighted = GameObject.FindGameObjectWithTag("PCPWeighted").GetComponent<Toggle>().isOn;

		// Run Cancel() to clear and hide the NewData Panel after the values have been stored
		Cancel();
		// Add the new data
		AddDataPoints(newDataInputList, kValue, weighted);
	}

	private void AddDataPoints(List<string> newDataInputList, string kValue, bool weighted)
	{
		Dictionary<string, object> last = pointList.Last();

		Dictionary<string, object> newDataPoint = new Dictionary<string, object>
		{
			{ last.Keys.First().ToString(), (Convert.ToInt32(last[last.Keys.First()], CultureInfo.InvariantCulture)) + 1 }
		};

		for (int i = 0; i < columnList.Count - 2; i++)
			newDataPoint.Add(columnList[i + 1], newDataInputList[i]);

		double[] unknown = new double[newDataInputList.Count];

		for (int i = 0; i < newDataInputList.Count; ++i)
			unknown[i] = (Convert.ToDouble(newDataInputList[i], CultureInfo.InvariantCulture));

		var predict = dataClass.Knn(unknown, kValue, weighted);
		newDataPoint.Add(columnList[columnList.Count - 1], predict);

		pointList.Add(newDataPoint);

		// Render the dataPlot again with newly added data
		DrawBackgroundGrid();
		ReorderColumns();

		Blink(KNN.kPoints);
		KNNMode = true;
		KNNWindow.SetActive(true);

		// Target the last DataBall (column4) within the newly added instance
		GameObject newBall = (GameObject)pointList.Last()["DataBall4"] as GameObject;
		TargetingScript.selectedTarget = newBall;
		TargetingScript.colorOff = TargetingScript.selectedTarget.GetComponent<Renderer>().material.color;
		TargetingScript.selectedTarget.GetComponent<Renderer>().material.color = Color.white;
		TargetingScript.selectedTarget.transform.localScale += new Vector3(+0.01f, +0.01f, +0.01f);
	}

	public void ChangeDataPoint(string k, bool weightedOrNot)
	{
		Dictionary<string, object> KnnPoint = pointList.Last();
		pointList.Remove(KnnPoint);

		double[] unknown = new double[KnnPoint.Count - 6]; //TODO: "[KnnPoint.Count - 6]" är nog bara ok för dataset >= 4 features.

		for (int i = 0; i < KnnPoint.Count - 6; ++i) //TODO: "KnnPoint.Count - 6" är nog bara ok för dataset >= 4 features.
			unknown[i] = (Convert.ToDouble(KnnPoint[columnList[i + 1]], CultureInfo.InvariantCulture));

		var predict = dataClass.Knn(unknown, k, weightedOrNot);
		KnnPoint[columnList.Last()] = predict;
		pointList.Add(KnnPoint);
		ReorderColumns();

		if (KNN.kPoints != null && KNN.kPoints.Count > 0)
			Blink(KNN.kPoints);
	}

	private void Cancel()
	{
		// Empty the panel when leaving it
		foreach (Transform child in newDataContainer.transform)
			Destroy(child.gameObject);

		// Hide the panel when leaving it
		newDataPanel.SetActive(false);

		// Make newDataButton interactable again
		GameObject.FindGameObjectWithTag("PCPNewDataButton").GetComponent<Button>().interactable = true;
	}

	void Blink(List<int> kPoints)
	{
		foreach (int data in kPoints)
		{
			for (int i = 1; i <= 4; i++)
			{
				GameObject ball = (GameObject)pointList[data - 1][$"DataBall{i}"];
				ball.GetComponent<Blink>().enabled = true;
			}
		}
	}

	#endregion


	#region EditPosition Methods

	public void ChangeButtonOnClick()
	{
		if (ChangePanelColumnInputfield.text.Length > 0)
		{
			newValue = ChangePanelColumnInputfield.GetComponent<TMP_InputField>().text;
			newValue = newValue.Replace(',', '.');
			int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
			pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().TargetFeature] = newValue;
			ChangePanelColumnInputfield.text = string.Empty;
		}

		KNNMove = true;

		DrawBackgroundGrid();
		ReorderColumns();
	}

	private void Denormalize()
	{
		float mellanskillnad = yMax - yMin;
		ChangePanelColumnValueText.text = (yMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.y) / 10).ToString("0.0");
	}

	#endregion


	#endregion
}
