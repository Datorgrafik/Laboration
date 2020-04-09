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
    private string newValue;

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
    }

    public void OnClick()
    {
        if (inputX.text.Length > 0)
        {
            newValue = inputX.GetComponent<InputField>().text;
            X.text = newValue;
            selectedTarget.transform.position = new Vector3(Convert.ToSingle(X.text), posY, posZ);
        }
        else if (inputY.text.Length > 0)
        {
            newValue = inputY.GetComponent<InputField>().text;
            Y.text = newValue;
            selectedTarget.transform.position = new Vector3(posX, Convert.ToSingle(Y.text), posZ);
        }
        else if (inputZ.text.Length > 0)
        {
            newValue = inputZ.GetComponent<InputField>().text;
            Z.text = newValue;
            selectedTarget.transform.position = new Vector3(posX, posY, Convert.ToSingle(Z.text));
        }
    }
}
