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
	private List<Dictionary<string, object>> pointList;

	// Lists
	public List<string> columnList;
	public List<string> featureList;
	private List<string> targetFeatures = new List<string>();

	// GameObjects
	public GameObject PointPrefab;
	public GameObject LinePrefab;
	public GameObject TargetFeaturePrefab;
	public GameObject PointHolder;

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
	private string columnName;
	private int nFeatures;

	// PlotColumns
	public List<Dropdown> columnDropdownList = new List<Dropdown>();

	// Column Text Fields
	public List<TMP_Text> columnTextList = new List<TMP_Text>();

	// ChangePanel Lists
	public List<TMP_Text> ChangePanelColumnText = new List<TMP_Text>();
	public List<TMP_Text> ChangePanelColumnValueText = new List<TMP_Text>();
	public List<TMP_InputField> ChangePanelColumnInputfield = new List<TMP_InputField>();

	#endregion

	#region Methods

	// Start is called before the first frame update
	void Start()
	{
		// Set pointlist to results of function Reader with argument inputfile
		//dataClass = CSVläsare.Read(MainMenu.fileData);
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
			ChangePanelColumnText[i].text = featureList[i];

			if (i+1 == 4)
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
					ChangePanelColumnText[iLocal].text = featureList[columnDropdownList[iLocal].value];
				});

			if (i + 1 == 4)
				break;
		}
	}

	private void DrawBackgroundGrid()
	{
		// Draw vertical lines
		for (int i = 1; i <= 4; i++)
		{
			float xPos = SetColumnPosition(i);

			GameObject xLine = Instantiate(LinePrefab, new Vector3(xPos, 0f, -0.001f) * plotScale, Quaternion.identity);
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

		float yMax = 0f;
		float yMin = float.Parse(pointList[0][featureList[0]].ToString(), CultureInfo.InvariantCulture);

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
			yLine.transform.parent = PointHolder.transform;
			yLine.transform.name = $"Value{i}Line";

			LineRenderer yLineRenderer = yLine.GetComponent<LineRenderer>();
			yLineRenderer.positionCount = 2;
			yLineRenderer.startWidth = 0.025f;
			yLineRenderer.endWidth = 0.025f;
			yLineRenderer.SetPosition(0, new Vector3(0.1f, (float)i / 10, -0.001f) * plotScale);
			yLineRenderer.SetPosition(1, new Vector3(1.5f, (float)i / 10, -0.001f) * plotScale);
			yLineRenderer.material.color = new Color(0.5f, 0.5f, 0.5f, 0.4f);

			TMP_Text valuePointY = Instantiate(valuePrefab, new Vector3(1.25f, 0 + i, 0), Quaternion.identity);
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

			float index = targetFeatures.IndexOf(targetFeature.ToString());
			float colorValue = 1 / (index + 1);

			if (index % 3 == 0)
				targetColor = new Color(0, colorValue, 0);
			else if (index % 3 == 1)
				targetColor = new Color(0, 0, colorValue);
			else if (index % 3 == 2)
				targetColor = new Color(colorValue, 0, 0);
			else
				targetColor = Color.black;

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

		// Get MaxValue
		float columnMax = CalculationHelpers.FindMaxValue(columnName, pointList);
		// Get MinValue
		float columnMin = CalculationHelpers.FindMinValue(columnName, pointList);

		//Loop through Pointlist & Render dataset
		for (var i = 0; i < pointList.Count; i++)
		{
			// Set correct color
			targetColor = SetColors(targetFeatures, i);

			// Get original value
			string valueString = pointList[i][columnName].ToString();
			// Set normalized Y-value
			float y = (float.Parse(valueString, CultureInfo.InvariantCulture) - columnMin) / (columnMax - columnMin);

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

	private Color SetColors(List<string> targetFeatures, int i)
	{
		Color targetColor;
		float index = targetFeatures.IndexOf(pointList[i][columnList[columnList.Count - 1]].ToString());
		float colorValue = 1 / (index + 1);

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

	public void ReorderColumns()
	{
		// Destroy Datapoints
		//foreach (var databall in GameObject.FindGameObjectsWithTag("DataBall"))
		//	Destroy(databall);

		// Plot data for the selected columns
		for (int i = 0; i < nFeatures; i++)
		{
			PlotData(columnDropdownList[i].value, i+1);

			if (i + 1 == 4)
				break;
		}
	}

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
		List<string> newDataInputList = new List<string>();

		// Get list of values from newData InputFields
		foreach (GameObject dataInput in GameObject.FindGameObjectsWithTag("PCPNewDataInputField"))
		{
			newDataInputList.Add(dataInput.GetComponent<TMP_InputField>().text);
			dataInput.GetComponent<TMP_InputField>().text = null;
		};

		string kValue = GameObject.FindGameObjectWithTag("PCPkValue").GetComponent<TMP_InputField>().text;
		bool weighted = GameObject.FindGameObjectWithTag("PCPWeighted").GetComponent<Toggle>().isOn;

		Cancel();
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

		ReorderColumns();
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

	#endregion
}
