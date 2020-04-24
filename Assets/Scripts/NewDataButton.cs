using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
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
    private void Start()
    {
        newData.onClick.AddListener(OnClick);
    }
    private void Cancel()
    {
        foreach (Transform child in newDataWindow.transform)
        {
            Destroy(child.gameObject);
        }

        newDataList.SetActive(false);
        newDataWindow.SetActive(false);
        newData.interactable = true;
    }

    private void OnClick()
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
            inputfield.text = FindAverage(attributes[i]);


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
    private void SaveInput()
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
        DataPlotter.AddDataPoint(dataPoint);
        //newData.interactable = true;
    }
    //public void AddDataPoint(List<string> newPoint)
    //{
    //    Dictionary<string, object> last = DataPlotter.pointList.Last();

    //    Dictionary<string, object> newDataPoint = new Dictionary<string, object>();

    //    newDataPoint.Add("", (Convert.ToInt32(last[""], CultureInfo.InvariantCulture)) + 1);

    //    for (int i = 0; i < DataPlotter.ThisInstans.columnList.Count - 2; i++)
    //    {
    //        newDataPoint.Add(DataPlotter.ThisInstans.columnList[i + 1], newPoint[i]);
    //    }

    //    double[] unknown = new double[newPoint.Count];

    //    for (int i = 0; i < newPoint.Count; ++i)
    //    {
    //        unknown[i] = (Convert.ToDouble(newPoint[i], CultureInfo.InvariantCulture));
    //        Debug.Log(newPoint[i].ToString());
    //    }

    //    var predict = DataPlotter.dataClass.Knn(unknown);
    //    newDataPoint.Add(DataPlotter.ThisInstans.columnList[DataPlotter.ThisInstans.columnList.Count - 1], predict);

    //    DataPlotter.pointList.Add(newDataPoint);

    //    DataPlotter.ThisInstans.PlottData();

    //}
    private string FindAverage(string attribute)
    {
        double  sum = 0.0;
        for (int i = 0; i < DataPlotter.dataClass.CSV.Count - 1; ++i)
        {
            sum += Convert.ToDouble(DataPlotter.dataClass.CSV[i][attribute], CultureInfo.InvariantCulture);
        }
        return Convert.ToString(sum / DataPlotter.dataClass.CSV.Count - 1);

    }
}
