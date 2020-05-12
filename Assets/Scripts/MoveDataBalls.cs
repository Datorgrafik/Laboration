using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class MoveDataBalls : MonoBehaviour
{
    #region Attributes

    private bool grabItem = false;
    private Vector3 mousePosition;
    private GameObject selectedTarget;
    private EventSystem eventSys;

    private float timeChecker = 0f;
    private int index;

    #endregion

    #region Methods

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
                eventSys = GameObject.Find("EventSystem").GetComponent<EventSystem>();

                if (eventSys.IsPointerOverGameObject())
                    return;

                timeChecker += Time.unscaledDeltaTime;

                if (timeChecker > 0.3F)
                {
                    mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, TargetingScript.selectedTarget.transform.position.z) * -1);

                    if (SceneManager.GetActiveScene().name == "ParallelCoordinatePlot")
                    {
                        TargetingScript.selectedTarget.transform.position = new Vector3(TargetingScript.selectedTarget.transform.position.x, mousePosition.y, mousePosition.z);
                    }
                    else
                    {
                        TargetingScript.selectedTarget.transform.position = new Vector3(mousePosition.x, mousePosition.y, mousePosition.z);
                    }
                }
            }

            if (Input.GetMouseButtonUp(0) && timeChecker > 0.3F)
                Denormalize();
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
                TargetingScript.selectedTarget.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 1;
        }
    }

    private void Denormalize()
    {
        float mellanskillnad = DataPlotter.ThisInstans.xMax - DataPlotter.ThisInstans.xMin;
        string newPosition = (DataPlotter.ThisInstans.xMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.x) / 10).ToString();
        newPosition = newPosition.Replace(',', '.');
        index = selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
        DataPlotter.pointList[index][DataPlotter.xName] = newPosition;

        mellanskillnad = DataPlotter.ThisInstans.yMax - DataPlotter.ThisInstans.yMin;
        newPosition = (DataPlotter.ThisInstans.yMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.y) / 10).ToString();
        newPosition = newPosition.Replace(',', '.');
        index = selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
        DataPlotter.pointList[index][DataPlotter.yName] = newPosition;

        if (MainMenu.renderMode == 1)
        {
            mellanskillnad = DataPlotter.ThisInstans.zMax - DataPlotter.ThisInstans.zMin;
            newPosition = (DataPlotter.ThisInstans.zMin + (mellanskillnad * TargetingScript.selectedTarget.transform.position.z) / 10).ToString();
            newPosition = newPosition.Replace(',', '.');
            index = selectedTarget.GetComponent<StoreIndexInDataBall>().Index;
            DataPlotter.pointList[index][DataPlotter.zName] = newPosition;
        }

        DataPlotter.ThisInstans.PlottData();
    }

    #endregion
}
