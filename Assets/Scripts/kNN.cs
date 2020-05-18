using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public static class KNN
{
    #region Attributes

    public static List<string> attributes;

    // k datapoints index the Knn uses for pred
    public static List<int> kPoints;

    // the k value that the KNN uses
    public static int kValue;

    // if the KNN uses weights or not 
    public static bool trueOrFalse;

    // flags to determine when to enter/leave KNNmode and when the new datapoint is moved to be predicted again
    public static bool KNNMode = false;
    public static bool KNNMove = false;

    #endregion

    #region Methods

    // KNN for regression problems
    public static object ClassifyReg(double[] unknown, List<Dictionary<string, object>> trainData)
    {
        // list of datapoints index  and distance to new datapoint
        IndexAndDistance[] info = SortedDistanceArray(unknown, trainData);

        if (trueOrFalse)
            // weighted KNN
            return WeightReg(info, trainData);
        else
            // Not weighted KNN
            return VoteReg(info, trainData);
    }

    // KNN for classificatin problems
    public static object ClassifyClass(double[] unknown, List<Dictionary<string, object>> trainData)
    {
        // list of datapoints index  and distance to new datapoint
        IndexAndDistance[] info = SortedDistanceArray(unknown, trainData);

        return Vote(info, trainData);
    }

    // Calculates the distance to new datapoint and sorts the list 
    public static IndexAndDistance[] SortedDistanceArray(double[] unknown, List<Dictionary<string, object>> trainData)
    {
        attributes = new List<string>(trainData[0].Keys);

        int n = trainData.Count;
        IndexAndDistance[] info = new IndexAndDistance[n];

        for (int i = 0; i < n; ++i)
        {
            IndexAndDistance curr = new IndexAndDistance();
            double dist = Distance(unknown, trainData[i]);
            curr.idx = i;
            curr.dist = dist;
            info[i] = curr;
        }

        Array.Sort(info);

        return info;
    }

    // caluclates the class for the k nearest neigbor to the new point to determine what the prediction is
    public static object Vote(IndexAndDistance[] info, List<Dictionary<string, object>> trainData)
    {
        kPoints = new List<int>();

        // Dictionary with classes and how many votes on each
        Dictionary<string, double> votes = new Dictionary<string, double>(); // One cell per class

        for (int i = 0; i < kValue; ++i)
        {   // Just first k
            int idx = info[i].idx; // Which train item
            string c;

            // Special cast because of how PCP works
            if (SceneManager.GetActiveScene().name == "ParallelCoordinatePlot")
                c = (string)trainData[idx][attributes[attributes.Count - 5]];   // Class in last cell
            else
                c = (string)trainData[idx][attributes[attributes.Count - 2]];   // Class in last cell

            if (votes.ContainsKey(c))
            {
                if (trueOrFalse)
                    votes[c] = votes[c] + (1 / Math.Pow(info[i].dist, 2));
                else
                    ++votes[c];
            }
            else
            {
                if (trueOrFalse)
                    votes.Add(c, 1 / Math.Pow(info[i].dist, 2));
                else
                    votes.Add(c, 1);
            }
            kPoints.Add(Convert.ToInt32(trainData[idx][trainData[0].Keys.First()], CultureInfo.InvariantCulture));
        }
        var Maxvotes = votes.FirstOrDefault(x => x.Value == votes.Values.Max()).Key;

        return Maxvotes;
    }

    // caluclates the target value for the new point by using k  nearest neigbor 
    public static object VoteReg(IndexAndDistance[] info, List<Dictionary<string, object>> trainData)
    {
        kPoints = new List<int>();
        double sum = 0.0;
        double c;

        for (int i = 0; i < kValue; ++i)
        {
            int idx = info[i].idx;
            // Special cast because of how PCP works
            if (SceneManager.GetActiveScene().name == "ParallelCoordinatePlot")
                c = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 5]], CultureInfo.InvariantCulture);
            else
                c = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 2]], CultureInfo.InvariantCulture);

            sum += c;
            kPoints.Add(Convert.ToInt32(trainData[idx-1][trainData[0].Keys.First()], CultureInfo.InvariantCulture));
        }

        return sum / kValue;
    }

    // calculates the distance
    public static double Distance(double[] unknown, Dictionary<string, object> data)
    {
        double sum = 0.0;

        for (int i = 0; i < unknown.Length; ++i)
            sum += (unknown[i] - Convert.ToDouble(data[attributes[i + 1]], CultureInfo.InvariantCulture)) * (unknown[i] - Convert.ToDouble(data[attributes[i + 1]], CultureInfo.InvariantCulture));

        return Math.Sqrt(sum);
    }
    //Calculates the weigthed value for the new datapoint
    public static object WeightReg(IndexAndDistance[] info, List<Dictionary<string, object>> trainData)
    {
        double sumWeight = 0.0;
        double sumWeightXReg = 0.0;
        kPoints = new List<int>();

        for (int i = 0; i < kValue; ++i)
        {
            int idx = info[i].idx;
            double weight = (1 / Math.Pow(info[i].dist, 2));
            sumWeight += weight;
            double reg;

            // Special cast because of how PCP works
            if (SceneManager.GetActiveScene().name == "ParallelCoordinatePlot")
                reg = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 5]], CultureInfo.InvariantCulture);
            else
                reg = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 2]], CultureInfo.InvariantCulture);

            sumWeightXReg += weight * reg;
            kPoints.Add(Convert.ToInt32(trainData[idx-1][trainData[0].Keys.First()], CultureInfo.InvariantCulture));
        }

        return sumWeightXReg / sumWeight;
    }

    #endregion
}


// Program class to use flr sorting the list by distance to the new datapoint
public class IndexAndDistance : IComparable<IndexAndDistance>
{
    public int idx;  // Index of a training item
    public double dist;  // To unknown
                         // Need to sort these to find k closest
    public int CompareTo(IndexAndDistance other)
    {
        if (this.dist < other.dist) return -1;
        else if (this.dist > other.dist) return +1;
        else return 0;
    }
}
