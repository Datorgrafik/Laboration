using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EditPosition : MonoBehaviour
{
    private GameObject selectedTarget;

    public GameObject panel;
    public InputField inputX;
    public InputField inputY;
    public InputField inputZ;
    public Text X;
    public Text Y;
    public Text Z;
    public Button ChangeX;
    public Button ChangeY;
    public Button ChangeZ;

    float posX = 0;
    float posY = 0;
    float posZ = 0;

    // Update is called once per frame
    void Update()
    {
        selectedTarget = TargetingScript.selectedTarget;
        if (selectedTarget != null)
        {
            panel.SetActive(true);
            posX = selectedTarget.transform.position[0];
            posY = selectedTarget.transform.position[1];
            posZ = selectedTarget.transform.position[2];
            X.text = Convert.ToString(posX);
            Y.text = Convert.ToString(posY);
            Z.text = Convert.ToString(posZ);

            ChangeX.onClick.AddListener(OnClick);
            ChangeY.onClick.AddListener(OnClick);
            ChangeZ.onClick.AddListener(OnClick);
        }
        else
        {
            panel.SetActive(false);
        }

        void OnClick()
        {
            if (inputX.text.Length > 0)
            {
                X.text = inputX.text;
                //Change Databalls X position
            }
            else if (inputY.text.Length > 0)
            {
                Y.text = inputY.text;
            }
            else if (inputZ.text.Length > 0)
            {
                Z.text = inputZ.text;
            }
        }
    }
}
