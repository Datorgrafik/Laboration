using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KNN : MonoBehaviour
{
    #region Attributes

    public static List<string> attributes;
    public static List<int> kPoints;
    public int kValue;
    public bool trueOrFalse;

    #endregion

    #region Methods

    public KNN(double[] unknown, string k, bool weightedOrNot)
    {
        kValue = Convert.ToInt32(k);
        trueOrFalse = weightedOrNot;
    }

    public object ClassifyReg(double[] unknown, List<Dictionary<string, object>> trainData)
    {
        IndexAndDistance[] info = SortedDistanceArray(unknown, trainData);

        if (trueOrFalse)
            return WeightReg(info, trainData);
        else
            return VoteReg(info, trainData);
    }

    public object ClassifyClass(double[] unknown, List<Dictionary<string, object>> trainData)
    {
        IndexAndDistance[] info = SortedDistanceArray(unknown, trainData);

        return Vote(info, trainData);
    }

    public IndexAndDistance[] SortedDistanceArray(double[] unknown, List<Dictionary<string, object>> trainData)
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

    public object Vote(IndexAndDistance[] info, List<Dictionary<string, object>> trainData)
    {
        kPoints = new List<int>();
        Dictionary<string, double> votes = new Dictionary<string, double>(); // One cell per class

        for (int i = 0; i < kValue; ++i)
        {   // Just first k
            int idx = info[i].idx; // Which train item
            string c = (string)trainData[idx][attributes[attributes.Count - 2]];   // Class in last cell

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
            kPoints.Add(Convert.ToInt32(trainData[idx - 1][trainData[0].Keys.First()], CultureInfo.InvariantCulture));
        }
        var Maxvotes = votes.FirstOrDefault(x => x.Value == votes.Values.Max()).Key;

        return Maxvotes;
    }

    public object VoteReg(IndexAndDistance[] info, List<Dictionary<string, object>> trainData)
    {
        kPoints = new List<int>();
        double sum = 0.0;

        for (int i = 0; i < kValue; ++i)
        {
            int idx = info[i].idx;
            double c = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 1]], CultureInfo.InvariantCulture);
            sum += c;
            kPoints.Add(Convert.ToInt32(trainData[idx - 1][trainData[0].Keys.First()], CultureInfo.InvariantCulture));
        }

        return sum / kValue;
    }

    public double Distance(double[] unknown, Dictionary<string, object> data)
    {
        double sum = 0.0;

        for (int i = 0; i < unknown.Length; ++i)
            sum += (unknown[i] - Convert.ToDouble(data[attributes[i + 1]], CultureInfo.InvariantCulture)) * (unknown[i] - Convert.ToDouble(data[attributes[i + 1]], CultureInfo.InvariantCulture));

        return Math.Sqrt(sum);
    }

    public object WeightReg(IndexAndDistance[] info, List<Dictionary<string, object>> trainData)
    {
        double sumWeight = 0.0;
        double sumWeightXReg = 0.0;
        kPoints = new List<int>();

        for (int i = 0; i < kValue; ++i)
        {
            int idx = info[i].idx;
            double weight = (1 / Math.Pow(info[i].dist, 2));
            sumWeight += weight;
            double reg = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 2]], CultureInfo.InvariantCulture);
            sumWeightXReg += weight * reg;
            kPoints.Add(Convert.ToInt32(trainData[idx][trainData[0].Keys.First()], CultureInfo.InvariantCulture));
        }

        return sumWeightXReg / sumWeight;
    }

    #endregion
}


// Program class
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
