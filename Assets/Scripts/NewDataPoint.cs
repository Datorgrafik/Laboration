using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NewDataPoint : MonoBehaviour
{


    public static void AddDataPoint(List<string> newPoint)
    {
        Dictionary<string, object> last = CSVläsare.pointList.Last();

        Dictionary<string, object> newDataPoint = new Dictionary<string, object>
        {
            { last.Keys.First().ToString(), (Convert.ToInt32(last[last.Keys.First()], CultureInfo.InvariantCulture)) + 1 }
        };

        for (int i = 0; i < CSVläsare.columnList.Count - 2; i++)
            newDataPoint.Add(CSVläsare.columnList[i + 1], newPoint[i]);

        double[] unknown = new double[newPoint.Count];

        for (int i = 0; i < newPoint.Count; ++i)
            unknown[i] = (Convert.ToDouble(newPoint[i], CultureInfo.InvariantCulture));

        var predict = CSVläsare.dataClass.Knn(unknown);
        newDataPoint.Add(CSVläsare.columnList[CSVläsare.columnList.Count - 1], predict);
        CSVläsare.pointList.Add(newDataPoint);

        if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
        {
            ScatterPlotMatrix.ThisInstans.KNNWindow.SetActive(true);
            CameraBehavior.teleportCamera = true;
            ScatterPlotMatrix.ThisInstans.PlottData();
        }
        else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
        {
            ScatterplotDimensions.ThisInstans.KNNWindow.SetActive(true);
            ScatterplotDimensions.ThisInstans.teleportCamera = true;
            ScatterplotDimensions.ThisInstans.PlottData();
        }
        else
        {
            CameraBehavior.teleportCamera = true;
            DataPlotter.ThisInstans.KNNWindow.SetActive(true);
            DataPlotter.ThisInstans.PlottData();
        }

        KNN.KNNMode = true;
        
    }

    public static void ChangeDataPoint()
    {
        Dictionary<string, object> KnnPoint = CSVläsare.pointList.Last();
        CSVläsare.pointList.Remove(KnnPoint);
        
        double[] unknown = new double[KnnPoint.Count - 3];

        for (int i = 0; i < KnnPoint.Count - 3; ++i)
            unknown[i] = (Convert.ToDouble(KnnPoint[CSVläsare.columnList[i + 1]], CultureInfo.InvariantCulture));

        var predict = CSVläsare.dataClass.Knn(unknown);
        KnnPoint[CSVläsare.columnList.Last()] = predict;
        CSVläsare.pointList.Add(KnnPoint);

        if (SceneManager.GetActiveScene().name == "ScatterPlotMatrix")
        {
            ScatterPlotMatrix.ThisInstans.PlottData();
        }
        else if (SceneManager.GetActiveScene().name == "ValfriTeknik")
        {
            ScatterplotDimensions.ThisInstans.PlottData();
        }
        else
        {
            DataPlotter.ThisInstans.PlottData();
        }
    }
}
