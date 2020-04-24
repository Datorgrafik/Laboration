using UnityEngine;
using UnityEngine.UI;

public class ChangeTargetFeature : MonoBehaviour
{
    public GameObject changeTargetFeaturePanel;
    public Button colorOfTargetFeature;
    public Text targetFeatureText;
    public Dropdown changeTargetFeature;
    public Button renameButton;
    private bool trueOrFalse = false;
    private GameObject selTarget;

    // Update is called once per frame
    void Update()
    {
        if (TargetingScript.selectedTarget != null && trueOrFalse == false)
        {
            selTarget = TargetingScript.selectedTarget;
            trueOrFalse = true;
            changeTargetFeaturePanel.SetActive(true);
            colorOfTargetFeature.GetComponent<Image>().color = TargetingScript.colorOff;
            changeTargetFeature.AddOptions(DataPlotter.ThisInstans.targetFeatures);
            targetFeatureText.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;

        }
        else if (TargetingScript.selectedTarget != selTarget)
        {
            changeTargetFeaturePanel.SetActive(false);
            trueOrFalse = false;
            changeTargetFeature.ClearOptions();
        }
    }

    public void onClick()
    {
        selTarget.GetComponent<StoreIndexInDataBall>().TargetFeature = DataPlotter.ThisInstans.targetFeatures[changeTargetFeature.GetComponent<Dropdown>().value];
        DataPlotter.pointList[selTarget.GetComponent<StoreIndexInDataBall>().index][DataPlotter.ThisInstans.columnList[DataPlotter.ThisInstans.columnList.Count - 1]] = selTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;
        DataPlotter.ChangeColor(selTarget, changeTargetFeature.GetComponent<Dropdown>().value);
        TargetingScript.colorOff = selTarget.GetComponent<Renderer>().material.color;
        colorOfTargetFeature.GetComponent<Image>().color = selTarget.GetComponent<Renderer>().material.color;
        targetFeatureText.text = selTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;
    }
}
