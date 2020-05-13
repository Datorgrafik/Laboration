using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LeaveKNNMode : MonoBehaviour
{
    public Button LeaveKNN;
    public GameObject KNNWindow;
    public Button NewData;

    // Start is called before the first frame update
    void Start()
    {
        LeaveKNN.onClick.AddListener(Leave);
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

        if (SceneManager.GetActiveScene().name == "ParallelCoordinatePlot")
        {
            ParallelCoordinatePlotter.KNNMode = false;
            KNN.kPoints.Clear();
            ParallelCoordinatePlotter.ThisInstans.ReorderColumns();

        }
        else
        {
            DataPlotter.KNNMode = false;
            DataPlotter.ThisInstans.PlottData();

        }
        NewData.interactable = true;
    }
}
    
