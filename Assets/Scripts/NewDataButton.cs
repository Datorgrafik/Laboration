using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    public InputField KUpdate;

    public DataClass dataClass;
    private Button SaveData;


	#endregion

	#region Methods

	// Start is called before the first frame update
	void Start()
	{
		newData.onClick.AddListener(CreateWindow);
	}

    private void Cancel()
	{
        newDataList.SetActive(false);
        newData.interactable = true;

        foreach (Transform child in newDataWindow.transform)
			Destroy(child.gameObject);

	}

	private void CreateWindow()
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
			inputfield.text = CalculationHelpers.FindAverage(attributes[i], CSVläsare.pointList);

			ypos -= 20;
		}

		SaveData = Instantiate(button, new Vector2(71, ypos), Quaternion.identity) as Button;
		SaveData.GetComponentInChildren<Text>().text = "Save";
		SaveData.transform.SetParent(newDataWindow.transform, false);
		SaveData.onClick.AddListener(SaveInput);
        SaveData.interactable = false;

        

		Button CancelButton = Instantiate(button, new Vector2(71, ypos - 20), Quaternion.identity) as Button;
		CancelButton.GetComponentInChildren<Text>().text = "Cancel";
		CancelButton.onClick.AddListener(Cancel);
		CancelButton.transform.SetParent(newDataWindow.transform, false);

		newDataList.SetActive(true);
		newData.interactable = false;
        InvokeRepeating("SaveCheck", 0, 0.2f);
	}

	private void SaveInput()
	{
        CancelInvoke("SaveCheck");
		dataPoint.Clear();

		foreach (InputField data in newDataWindow.GetComponentsInChildren<InputField>())
		{
			dataPoint.Add(data.text);
			data.text = null;
		}

		kValue = k.GetComponent<InputField>().text;

        if (Convert.ToInt32(kValue) < 1)
            kValue = "1";

        if (Convert.ToInt32(kValue) > CSVläsare.pointList.Count())
            kValue = CSVläsare.pointList.Count().ToString();

		if (weighted.GetComponent<Toggle>().isOn == true)
			weightedOrNot = true;
		else
			weightedOrNot = false;

        newDataList.SetActive(false);

        foreach (Transform child in newDataWindow.transform)
            Destroy(child.gameObject);

        KUpdate.text = k.text;

        KNN.kValue = Convert.ToInt32(kValue);
        KNN.trueOrFalse = weightedOrNot;

        NewDataPoint.AddDataPoint(dataPoint);


    }
    private void SaveCheck()
    {
        bool Empty = false;

        foreach (InputField data in newDataWindow.GetComponentsInChildren<InputField>())
        {
            if (data.text == "")
            {
                SaveData.interactable = false;
                Empty = true;
                break;
            }

        }
        if (k.text == "")
        {
            Empty = true;
            SaveData.interactable = false;

        }
        if (!Empty)
            SaveData.interactable = true;
    }
    #endregion
}
