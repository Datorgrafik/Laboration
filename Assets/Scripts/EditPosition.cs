using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditPosition : MonoBehaviour
{
    public GameObject selectedTarget = TargetingScript.selectedTarget;

    public GameObject panel;
    public InputField inputX;
    public InputField inputY;
    public InputField inputZ;

    float posX;
    float posY;
    float posZ;

    // Update is called once per frame
    void Update()
    {
        if (selectedTarget != null)
        {
            panel.SetActive(true);
            posX = selectedTarget.transform.position[0];
            posY = selectedTarget.transform.position[1];
            posZ = selectedTarget.transform.position[2];
            
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
