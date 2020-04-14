using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditPosition : MonoBehaviour
{
	public GameObject panel;

	public InputField inputX;
	public InputField inputY;
	public InputField inputZ;

	public Text Xvalue;
	public Text Yvalue;
	public Text Zvalue;
	public Text Xname;
	public Text Yname;
	public Text Zname;

	public Button ChangeX;

	private string newValue;

	// Update is called once per frame
	void Update()
	{
		if (TargetingScript.selectedTarget != null)
		{
			panel.SetActive(true);

			Xname.text = DataPlotter.xName;
			Yname.text = DataPlotter.yName;

			string[] newName = TargetingScript.selectedTarget.name.Split(' ');

			Xvalue.text = newName[0];
			Yvalue.text = newName[1];

			if (MainMenu.renderMode == 1)
			{
				Zname.text = DataPlotter.zName;
				Zvalue.text = newName[2];
			}

			ChangeX.onClick.AddListener(OnClick);
		}
		else
		{
			panel.SetActive(false);
		}
	}

	public void OnClick()
	{
		//Fullt funktionell, saknas endast normalisering för att få till rätt position vid ändring
		if (inputX.text.Length > 0)
		{
			newValue = inputX.GetComponent<InputField>().text;

			if (MainMenu.renderMode == 0)
			{
				TargetingScript.selectedTarget.transform.name = newValue + " " + Yvalue.text;
				TargetingScript.selectedTarget.transform.position = new Vector3(float.Parse(newValue, CultureInfo.InvariantCulture), float.Parse(Yvalue.text, CultureInfo.InvariantCulture), (0));
			}
			else
			{
				TargetingScript.selectedTarget.transform.name = newValue + " " + Yvalue.text + " " + Zvalue.text;
				TargetingScript.selectedTarget.transform.position = new Vector3(float.Parse(newValue, CultureInfo.InvariantCulture), float.Parse(Yvalue.text, CultureInfo.InvariantCulture), float.Parse(Zvalue.text, CultureInfo.InvariantCulture));
			}
			Xvalue.text = newValue;
		}

		if (inputY.text.Length > 0)
		{
			newValue = inputY.GetComponent<InputField>().text;

			if (MainMenu.renderMode == 0)
			{
				TargetingScript.selectedTarget.transform.name = Xvalue.text + " " + newValue;
				TargetingScript.selectedTarget.transform.position = new Vector3(float.Parse(Xvalue.text, CultureInfo.InvariantCulture), float.Parse(newValue, CultureInfo.InvariantCulture), (0));
			}
			else
			{
				TargetingScript.selectedTarget.transform.name = Xvalue.text + " " + newValue + " " + Zvalue.text;
				TargetingScript.selectedTarget.transform.position = new Vector3(float.Parse(Xvalue.text, CultureInfo.InvariantCulture), float.Parse(newValue, CultureInfo.InvariantCulture), float.Parse(Zvalue.text, CultureInfo.InvariantCulture));
			}
			Yvalue.text = newValue;
		}

		if (MainMenu.renderMode == 1)
		{
			if (inputZ.text.Length > 0)
			{
				newValue = inputZ.GetComponent<InputField>().text;

				TargetingScript.selectedTarget.transform.name = Xvalue.text + " " + Yvalue.text + " " + newValue;
				TargetingScript.selectedTarget.transform.position = new Vector3(float.Parse(Xvalue.text, CultureInfo.InvariantCulture), float.Parse(Yvalue.text, CultureInfo.InvariantCulture), float.Parse(newValue, CultureInfo.InvariantCulture));
			}
		}
	}
}
