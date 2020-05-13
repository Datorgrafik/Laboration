using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        KNNWindow.SetActive(false);
        KNN.kPoints.Clear();

        if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
        {
            ScatterPlotMatrix.KNNMode = false;
            ScatterPlotMatrix.ThisInstans.PlottData();
        }
        if (SceneManager.GetActiveScene().name == "ValfriTeknik")
        {
            ScatterplotDimensions.KNNMode = false;
            ScatterplotDimensions.ThisInstans.PlottData();
        }
        else
        {
            DataPlotter.KNNMode = false;
            DataPlotter.ThisInstans.PlottData();

        }

    }
}
