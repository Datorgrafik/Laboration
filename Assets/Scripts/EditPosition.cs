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
    public GameObject dataPlotter;

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
    private bool addedListener = false;

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

            if (!addedListener)
            {
                addedListener = true;
                ChangeX.onClick.AddListener(OnClick);
            }
		}
		else
		{
			panel.SetActive(false);
            addedListener = false;
            ChangeX.onClick.RemoveListener(OnClick);
        }
	}

	public void OnClick()
	{
		if (inputX.text.Length > 0)
		{
			newValue = inputX.GetComponent<InputField>().text;
            int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().GetIndex();
            DataPlotter.pointList[index][DataPlotter.xName] = newValue;
        }

		if (inputY.text.Length > 0)
		{
			newValue = inputY.GetComponent<InputField>().text;
            int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().GetIndex();
            DataPlotter.pointList[index][DataPlotter.yName] = newValue;
        }

		if (MainMenu.renderMode == 1)
		{
			if (inputZ.text.Length > 0)
			{
				newValue = inputZ.GetComponent<InputField>().text;

                int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().GetIndex();
                DataPlotter.pointList[index][DataPlotter.zName] = newValue;
			}
		}

        dataPlotter.GetComponent<DataPlotter>().PlottData();
    }
}
