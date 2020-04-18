using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDataBalls : MonoBehaviour
{
	private bool grabItem = false;
	private Vector3 pointOnScreen;
	private Vector3 ObjectPoint;
    private Vector3 mousePosition;
    private GameObject selectedTarget;

    private float timeChecker = 0f;

    private float newValue;
    private int index;

    // Update is called once per frame
    void Update()
	{
		if (Input.GetMouseButtonDown(0))
        {
            if (grabItem == true)
            {
                Denormalize();
                grabItem = false;
            }
        }

        if (TargetingScript.selectedTarget != null && MainMenu.renderMode == 0)
        {
            if (Input.GetMouseButtonDown(0))
            {
                selectedTarget = TargetingScript.selectedTarget;
                timeChecker = 0f;
            }
            if (Input.GetMouseButton(0))
            {
                timeChecker += Time.unscaledDeltaTime;
                if(timeChecker > 0.3F)
                {
                    mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, TargetingScript.selectedTarget.transform.position.z) * -1);
                    TargetingScript.selectedTarget.transform.position = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                Denormalize();
            }
        }
        else if (TargetingScript.selectedTarget != null)
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                if (grabItem == true)
                {
                    grabItem = false;
                    Denormalize();
                }
                else
                {
                    grabItem = true;
                    selectedTarget = TargetingScript.selectedTarget;
                }
            }

            if (grabItem == true)
            {
                TargetingScript.selectedTarget.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
            }
        }
	}

    private void Denormalize()
    {
        float mellanskillnad = DataPlotter.ThisInstans.xMax - DataPlotter.ThisInstans.xMin;
        string newPosition = (DataPlotter.ThisInstans.xMin + mellanskillnad * selectedTarget.transform.position.x).ToString();
        index = selectedTarget.GetComponent<StoreIndexInDataBall>().index;
        DataPlotter.pointList[index][DataPlotter.xName] = newValue;

        mellanskillnad = DataPlotter.ThisInstans.yMax - DataPlotter.ThisInstans.yMin;
        newPosition = (DataPlotter.ThisInstans.yMin + mellanskillnad * selectedTarget.transform.position.y).ToString();
        index = selectedTarget.GetComponent<StoreIndexInDataBall>().index;
        DataPlotter.pointList[index][DataPlotter.xName] = newValue;

        string NewPos = (DataPlotter.ThisInstans.xMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.x) / 10).ToString();
        newPosition = newPosition.Replace(',', '.');
        index = selectedTarget.GetComponent<StoreIndexInDataBall>().index;
        DataPlotter.pointList[index][DataPlotter.xName] = newPosition;

        mellanskillnad = DataPlotter.ThisInstans.yMax - DataPlotter.ThisInstans.yMin;
        newPosition = (DataPlotter.ThisInstans.yMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.y) / 10).ToString();
        newPosition = newPosition.Replace(',', '.');
        index = selectedTarget.GetComponent<StoreIndexInDataBall>().index;
        DataPlotter.pointList[index][DataPlotter.yName] = newPosition;


        if (MainMenu.renderMode == 1)
        {
            mellanskillnad = DataPlotter.ThisInstans.zMax - DataPlotter.ThisInstans.zMin;
            newPosition = (DataPlotter.ThisInstans.zMin + mellanskillnad * selectedTarget.transform.position.z).ToString();
            index = selectedTarget.GetComponent<StoreIndexInDataBall>().index;
            DataPlotter.pointList[index][DataPlotter.xName] = newValue;

            newPosition = (DataPlotter.ThisInstans.zMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.z) / 10).ToString();
            newPosition = newPosition.Replace(',', '.');
            index = selectedTarget.GetComponent<StoreIndexInDataBall>().index;
            DataPlotter.pointList[index][DataPlotter.zName] = newPosition;

        }

        DataPlotter.ThisInstans.PlottData();
    }
}
