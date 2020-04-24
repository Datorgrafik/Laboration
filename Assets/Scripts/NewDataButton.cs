using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewDataButton : MonoBehaviour
{
    public Button newData;
    public GameObject newDataWindow;
    public GameObject newDataList;
    public InputField input;
    public Text description;
    public Button button;
    public List<string> dataPoint;


    // Start is called before the first frame update
    void Start()
    {
        newData.onClick.AddListener(OnClick);
    }
    void Cancel()
    {
        foreach (Transform child in newDataWindow.transform)
        {
            Destroy(child.gameObject);
        }

        newDataList.SetActive(false);
        newDataWindow.SetActive(false);
        newData.interactable = true;
    }

    void OnClick()
    {
        List<string> attributes = CSVläsare.columnList;
        Debug.Log(attributes.Count.ToString());
        int ypos = 224;
        for (int i = 1; i < attributes.Count - 1; ++i)
        {
            Text descrip = Instantiate(description, new Vector2(-80, ypos), Quaternion.identity) as Text;
            descrip.transform.SetParent(newDataWindow.transform, false);
            descrip.text = attributes[i];
            descrip.name = attributes[i];

            InputField inputfield = Instantiate(input, new Vector2(71, ypos), Quaternion.identity) as InputField;
            inputfield.transform.SetParent(newDataWindow.transform, false);
            inputfield.name = attributes[i];


            ypos = ypos - 20;
        }
        Button SaveData = Instantiate(button, new Vector2(71, ypos), Quaternion.identity) as Button;
        SaveData.GetComponentInChildren<Text>().text = "Save";
        SaveData.transform.SetParent(newDataWindow.transform, false);
        SaveData.onClick.AddListener(SaveInput);

        Button CancelButton = Instantiate(button, new Vector2(71, ypos-20), Quaternion.identity) as Button;
        CancelButton.GetComponentInChildren<Text>().text = "Cancel";
        CancelButton.onClick.AddListener(Cancel);
        CancelButton.transform.SetParent(newDataWindow.transform, false);


        newDataList.SetActive(true);
        newDataWindow.SetActive(true);
        newData.interactable = false;
    }
    public void SaveInput()
    {
        dataPoint.Clear();
        foreach (InputField data in newDataWindow.GetComponentsInChildren<InputField>())
        {
            dataPoint.Add(data.text);
            data.text = null;
           
        }
        Cancel();
        //newDataList.SetActive(false);
       // newDataWindow.SetActive(false);
        Debug.Log("Save inpur efter upphämtning av input");
        DataPlotter.AddDataPoint(dataPoint);
        //newData.interactable = true;
    }
}
