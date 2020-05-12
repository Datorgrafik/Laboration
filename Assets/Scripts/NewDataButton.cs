using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NewDataButton : MonoBehaviour
{
	#region Attributes

	public Button newData;
	public GameObject newDataWindow;
	public GameObject newDataList;
	public InputField input;
	public Text description;
	public Button button;
	public List<string> dataPoint;
	public InputField k;
	public Toggle weighted;
	public static string kValue;
	public static bool weightedOrNot;

	#endregion

	#region Methods

	// Start is called before the first frame update
	private void Start()
	{
		newData.onClick.AddListener(OnClick);
	}

	private void Cancel()
	{
		foreach (Transform child in newDataWindow.transform)
			Destroy(child.gameObject);

		newDataList.SetActive(false);
		newDataWindow.SetActive(false);
		newData.interactable = true;
	}

	private void OnClick()
	{
		List<string> attributes = CSVläsare.columnList;

		int ypos = 224;
		for (int i = 1; i < attributes.Count - 1; ++i)
		{
			Text descrip = Instantiate(description, new Vector2(-80, ypos), Quaternion.identity) as Text;
			descrip.transform.SetParent(newDataWindow.transform, false);
			descrip.text = attributes[i];
			descrip.name = attributes[i];

			InputField inputfield = Instantiate(input, new Vector2(71, ypos), Quaternion.identity) as InputField;
			inputfield.transform.SetParent(newDataWindow.transform, false);
			inputfield.name = attributes[i];
			inputfield.text = CalculationHelpers.FindAverage(attributes[i]);

			ypos -= 20;
		}

		Button SaveData = Instantiate(button, new Vector2(71, ypos), Quaternion.identity) as Button;
		SaveData.GetComponentInChildren<Text>().text = "Save";
		SaveData.transform.SetParent(newDataWindow.transform, false);
		SaveData.onClick.AddListener(SaveInput);

		Button CancelButton = Instantiate(button, new Vector2(71, ypos - 20), Quaternion.identity) as Button;
		CancelButton.GetComponentInChildren<Text>().text = "Cancel";
		CancelButton.onClick.AddListener(Cancel);
		CancelButton.transform.SetParent(newDataWindow.transform, false);

		newDataList.SetActive(true);
		newDataWindow.SetActive(true);
		newData.interactable = false;
	}

	private void SaveInput()
	{
		dataPoint.Clear();

		foreach (InputField data in newDataWindow.GetComponentsInChildren<InputField>())
		{
			dataPoint.Add(data.text);
			data.text = null;
		}

		kValue = k.GetComponent<InputField>().text;

		if (weighted.GetComponent<Toggle>().isOn == true)
			weightedOrNot = true;
		else
			weightedOrNot = false;

		Cancel();
		DataPlotter.AddDataPoint(dataPoint, kValue, weightedOrNot);

	}
	#endregion
}
