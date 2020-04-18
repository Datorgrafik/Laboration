using System.Collections;
using System.Collections.Generic;
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

    }
}
