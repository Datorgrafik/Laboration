using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LeaveKNNMode : MonoBehaviour
{
    public Button LeaveKNN;
    public GameObject KNNWindow;
    // Start is called before the first frame update
    void Start()
    {
        LeaveKNN.onClick.AddListener(Leave);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Leave()
    {
        DataPlotter.KNNMode = false;
        KNNWindow.SetActive(false);
        KNN.kPoints.Clear();
        DataPlotter.ThisInstans.PlottData();
    }
}
