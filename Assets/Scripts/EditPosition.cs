﻿using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditPosition : MonoBehaviour
{
	public GameObject panel;
    public GameObject dataPlotter;

	public InputField inputX;
	public InputField inputY;
	public InputField inputZ;
    public InputField inputÅ;

    public Text Xvalue;
	public Text Yvalue;
	public Text Zvalue;
    public Text Åvalue;
    public Text Xname;
	public Text Yname;
	public Text Zname;
    public Text Åname;

    public Button ChangeX;

	private string newValue;
    private bool addedListener = false;

    // Update is called once per frame
    void Update()
	{
		if (TargetingScript.selectedTarget != null)
		{
			panel.SetActive(true);

            string[] newName = TargetingScript.selectedTarget.name.Split(' ');

            if(SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
            {
                Xname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Column;
                Yname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Row;
                Zname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature3;
                Åname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature4;
                int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
                Xvalue.text = ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Column].ToString();
                Yvalue.text = ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Row].ToString();
                Zvalue.text = ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature3].ToString();
                Åvalue.text = ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature4].ToString();

            }
            else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
            {
                Xname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Column;
                Yname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Row;
                int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
                Xvalue.text = ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Column].ToString();
                Yvalue.text = ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Row].ToString();
            }
            else
            {
                Xname.text = DataPlotter.xName;
                Yname.text = DataPlotter.yName;
                Denormalize();
            }

			if (MainMenu.renderMode == 1)
				Zname.text = DataPlotter.zName;

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
            newValue = newValue.Replace(',', '.');
            int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;

            if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
                ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Column] = newValue;
            
            else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
                ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Column] = newValue;
            
            else
                DataPlotter.pointList[index][DataPlotter.xName] = newValue;

			inputX.text = string.Empty;
        }

		if (inputY.text.Length > 0)
		{
			newValue = inputY.GetComponent<InputField>().text;
            newValue = newValue.Replace(',', '.');
            int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;

            if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
                ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Row] = newValue;

            else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
                ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Row] = newValue;

            else
                DataPlotter.pointList[index][DataPlotter.yName] = newValue;

			inputY.text = string.Empty;
        }

		if (MainMenu.renderMode == 1 || SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
		{
			if (inputZ.text.Length > 0)
			{
				newValue = inputZ.GetComponent<InputField>().text;
                newValue = newValue.Replace(',', '.');
                int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
                if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
                    ScatterPlotMatrix.pointList[index][ScatterPlotMatrix.feature3Name] = newValue;
                else
                    DataPlotter.pointList[index][DataPlotter.zName] = newValue;

				inputZ.text = string.Empty;
			}
		}

        if (inputÅ.text.Length > 0 && SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
        {
            newValue = inputÅ.GetComponent<InputField>().text;
            newValue = newValue.Replace(',', '.');
            int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
            ScatterPlotMatrix.pointList[index][ScatterPlotMatrix.feature4Name] = newValue;
            inputÅ.text = string.Empty;
        }

        KNN.KNNMove = true;

        if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
            ScatterPlotMatrix.ThisInstans.PlottData();

        else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
            ScatterplotDimensions.ThisInstans.PlottData();

        else
            DataPlotter.ThisInstans.PlottData();
    }

    private void Denormalize()
    {
        if (SceneManager.GetActiveScene().name == "ValfriTeknik")
        {

        }
        else
        {
            float mellanskillnad = DataPlotter.ThisInstans.xMax - DataPlotter.ThisInstans.xMin;
            Xvalue.text = (DataPlotter.ThisInstans.xMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.x) / 10).ToString("0.0");

            mellanskillnad = DataPlotter.ThisInstans.yMax - DataPlotter.ThisInstans.yMin;
            Yvalue.text = (DataPlotter.ThisInstans.yMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.y) / 10).ToString("0.0");

            if (MainMenu.renderMode == 1)
            {
                mellanskillnad = DataPlotter.ThisInstans.zMax - DataPlotter.ThisInstans.zMin;
                Zvalue.text = (DataPlotter.ThisInstans.zMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.z) / 10).ToString("0.0");
            }
        }
    }
}
