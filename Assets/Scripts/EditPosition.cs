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
    public InputField inputA;
    public InputField inputB;

    public Text Xvalue;
	public Text Yvalue;
	public Text Zvalue;
    public Text Avalue;
    public Text Bvalue;

    public Text Xname;
	public Text Yname;
	public Text Zname;
    public Text Aname;
    public Text Bname;

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

            if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
            {
                Xname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature1;
                Yname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature2;
                Zname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature3;
                Aname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature4;
                int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
                Xvalue.text = ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature1].ToString();
                Yvalue.text = ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature2].ToString();
                Zvalue.text = ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature3].ToString();
                Avalue.text = ScatterPlotMatrix.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature4].ToString();

            }
            else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
            {
                Xname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature1;
                Yname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature2;
                Zname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature3;
                Aname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature4;
                Bname.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature5;
                int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
                Xvalue.text = ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature1].ToString();
                Yvalue.text = ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature2].ToString();
                Zvalue.text = ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature3].ToString();
                Avalue.text = ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature4].ToString();
                Bvalue.text = ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature5].ToString();
            }
            else
            {
                Xname.text = DataPlotter.xName;
                Yname.text = DataPlotter.yName;
                Denormalize();
            }

			if (MainMenu.renderMode == 1 && SceneManager.GetActiveScene().name == "ScatterPlot")
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
                ScatterPlotMatrix.pointList[index][ScatterPlotMatrix.feature1] = newValue;
            
            else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
                ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature1] = newValue;

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
                ScatterPlotMatrix.pointList[index][ScatterPlotMatrix.feature2] = newValue;

            else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
                ScatterplotDimensions.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Feature2] = newValue;

            else
                DataPlotter.pointList[index][DataPlotter.yName] = newValue;

			inputY.text = string.Empty;
        }

		if (MainMenu.renderMode == 1 || SceneManager.GetActiveScene().name == "ScatterPlotMatrix" || SceneManager.GetActiveScene().name == "ValfriTeknik")
		{
			if (inputZ.text.Length > 0)
			{
				newValue = inputZ.GetComponent<InputField>().text;
                newValue = newValue.Replace(',', '.');
                int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;

                if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
                    ScatterPlotMatrix.pointList[index][ScatterPlotMatrix.feature3] = newValue;

                else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
                    ScatterplotDimensions.pointList[index][ScatterplotDimensions.feature3Name] = newValue;

                else
                    DataPlotter.pointList[index][DataPlotter.zName] = newValue;

				inputZ.text = string.Empty;
			}
		}

        if ((SceneManager.GetActiveScene().name == "ScatterPlotMatrix" || SceneManager.GetActiveScene().name == "ValfriTeknik") && inputA.text.Length > 0)
        {
            newValue = inputA.GetComponent<InputField>().text;
            newValue = newValue.Replace(',', '.');
            int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;

            if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
                ScatterPlotMatrix.pointList[index][ScatterPlotMatrix.feature4] = newValue;
            else
               ScatterplotDimensions.pointList[index][ScatterplotDimensions.feature4Name] = newValue;

            inputA.text = string.Empty;
        }

        if ( SceneManager.GetActiveScene().name == "ValfriTeknik" && inputB.text.Length > 0)
        {
            newValue = inputB.GetComponent<InputField>().text;
            newValue = newValue.Replace(',', '.');
            int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
            ScatterplotDimensions.pointList[index][ScatterplotDimensions.feature5Name] = newValue;
            inputB.text = string.Empty;
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
