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

    public void Leave()
    {
        if (SceneManager.GetActiveScene().name == "ParallelCoordinatePlot")
        {
            ParallelCoordinatePlotter.KNNMode = false;
            KNNWindow.SetActive(false);
            KNN.kPoints.Clear();
            ParallelCoordinatePlotter.ThisInstans.ReorderColumns();
        }
        else
        {
            DataPlotter.KNNMode = false;
            KNNWindow.SetActive(false);
            KNN.kPoints.Clear();
            DataPlotter.ThisInstans.PlottData();
        }
    }
}
