using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ChangeTargetFeature : MonoBehaviour
{
	#region Attributes

	public GameObject changeTargetFeaturePanel;
	public Button colorOfTargetFeature;
	public Text targetFeatureText;
	public Dropdown changeTargetFeature;
	public Button renameButton;
	private bool trueOrFalse = false;
	private GameObject selTarget;

	#endregion

	#region Methods

	// Update is called once per frame
	void Update()
	{
		if (TargetingScript.selectedTarget != null && trueOrFalse == false)
		{
			selTarget = TargetingScript.selectedTarget;
			trueOrFalse = true;
			changeTargetFeaturePanel.SetActive(true);
			colorOfTargetFeature.GetComponent<Image>().color = TargetingScript.colorOff;
			changeTargetFeature.AddOptions(CSVläsare.targetFeatures);
			targetFeatureText.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;
		}
		else if (TargetingScript.selectedTarget != selTarget)
		{
			changeTargetFeaturePanel.SetActive(false);
			trueOrFalse = false;
			changeTargetFeature.ClearOptions();
		}
	}

	public void OnClick()
	{
		// Ändra targetfeature
		selTarget.GetComponent<StoreIndexInDataBall>().TargetFeature = CSVläsare.targetFeatures[changeTargetFeature.GetComponent<Dropdown>().value];
        // Lägg in den nya feature i pointlist och spara
        if (SceneManager.GetActiveScene().name == "ValfriTeknik")
        {
            ScatterplotDimensions.pointList[selTarget.GetComponent<StoreIndexInDataBall>().Index][ScatterplotDimensions.ThisInstans.columnList[ScatterplotDimensions.ThisInstans.columnList.Count - 1]] = selTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;           
        }
        else 
        {
            if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
                ScatterPlotMatrix.pointList[selTarget.GetComponent<StoreIndexInDataBall>().Index][ScatterPlotMatrix.ThisInstans.columnList[ScatterPlotMatrix.ThisInstans.columnList.Count - 1]] = selTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;
            else
                CSVläsare.pointList[selTarget.GetComponent<StoreIndexInDataBall>().Index][DataPlotter.ThisInstans.columnList[DataPlotter.ThisInstans.columnList.Count - 1]] = selTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;

            ColorManager.ChangeColor(selTarget, changeTargetFeature.GetComponent<Dropdown>().value);
            TargetingScript.colorOff = selTarget.GetComponent<Renderer>().material.color;
        }

		colorOfTargetFeature.GetComponent<Image>().color = selTarget.GetComponent<Renderer>().material.color;
		targetFeatureText.text = selTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;
	}

	#endregion
}
