using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PCPEditPosition : MonoBehaviour
{
	public GameObject EditPanel;
	public TMP_Text ChangePanelColumnText;
	public TMP_Text ChangePanelColumnValueText;
	public TMP_InputField ChangePanelColumnInputfield;

	private string newValue;

	// Update is called once per frame
	void Update()
	{
		if (TargetingScript.selectedTarget != null)
		{
			EditPanel.SetActive(true);
			ChangePanelColumnText.text = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().TargetFeature;
            Denormalize();
		}
		else
			EditPanel.SetActive(false);
	}

	public void ChangeButtonOnClick()
	{
		if (ChangePanelColumnInputfield.text.Length > 0)
		{
			newValue = ChangePanelColumnInputfield.GetComponent<TMP_InputField>().text;
            newValue = newValue.Replace(',', '.');
            int index = TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
            ParallelCoordinatePlotter.ThisInstans.pointList[index][TargetingScript.selectedTarget.GetComponent<StoreIndexInDataBall>().TargetFeature] = newValue;
			ChangePanelColumnInputfield.text = string.Empty;
        }

		ParallelCoordinatePlotter.ThisInstans.DrawBackgroundGrid();
		ParallelCoordinatePlotter.ThisInstans.ReorderColumns();
	}

    private void Denormalize()
    {
		float mellanskillnad = ParallelCoordinatePlotter.ThisInstans.yMax - ParallelCoordinatePlotter.ThisInstans.yMin;
		ChangePanelColumnValueText.text = (ParallelCoordinatePlotter.ThisInstans.yMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.y) / 10 ).ToString("0.0");
    }
}
