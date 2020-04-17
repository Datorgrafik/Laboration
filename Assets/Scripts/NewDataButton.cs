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
    public Button save;
    public List<string> dataPoint;

    // Start is called before the first frame update
    void Start()
    {
        newData.onClick.AddListener(OnClick);
        
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
        Button SaveData = Instantiate(save, new Vector2(71, ypos), Quaternion.identity) as Button;
        SaveData.transform.SetParent(newDataWindow.transform, false);
        newDataList.SetActive(true);
        newDataWindow.SetActive(true);
        SaveData.onClick.AddListener(SaveInput);
    }
    public void SaveInput()
    {
        dataPoint.Clear();
        foreach (InputField data in newDataWindow.GetComponentsInChildren<InputField>())
        {
            dataPoint.Add(data.text);
            data.text = null;
           
        }
        foreach (Transform child in newDataWindow.transform)
        {
            Destroy(child.gameObject);
        }

        newDataList.SetActive(false);
        newDataWindow.SetActive(false);
        Debug.Log("Save inpur efter upphämtning av input");
        DataPlotter.AddDataPoint(dataPoint);

    }
}
