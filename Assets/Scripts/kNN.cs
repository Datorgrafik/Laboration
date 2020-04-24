using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class KNN : MonoBehaviour
{
    public static List<string> attributes;
    //public Toggle weights { get { return instance.myNormalVar; } }
    //public static List<Dictionary<string, object>> PointsToColor;
    public static List<int> kPoints;
    public string kValue;
    public bool trueOrFalse;

    public KNN(double[] unknown, string k, bool weightedOrNot)
    {
        kValue = k;
        trueOrFalse = weightedOrNot;
        int u = 1;
    }

    void Start()
    {
        
    }

    public object ClassifyReg(double[] unknown,
List<Dictionary<string, object>> trainData)
    {
        IndexAndDistance[] info = SortedDistanceArray(unknown, trainData);

        return VoteReg(info, trainData,3);
    }

    public object ClassifyClass(double[] unknown,
List<Dictionary<string, object>> trainData)
    {
        IndexAndDistance[] info = SortedDistanceArray(unknown, trainData);
        
        return Vote(info, trainData, 3);
    }

    public IndexAndDistance[] SortedDistanceArray(double[] unknown,
List<Dictionary<string, object>> trainData)
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

    public object Vote(IndexAndDistance[] info,
      List<Dictionary<string, object>> trainData, int k)
    {
        kPoints = new List<int>();
        //PointsToColor.Clear();
        Dictionary<string, int> votes = new Dictionary<string, int>(); // One cell per class
        for (int i = 0; i < k; ++i)
        {       // Just first k
            int idx = info[i].idx;            // Which train item
            string c = (string)trainData[idx][attributes[attributes.Count - 2]];   // Class in last cell
            if (votes.ContainsKey(c))
            {
                ++votes[c];
            }
            else
            {
                votes.Add(c, 1);
            }
            kPoints.Add(Convert.ToInt32(trainData[idx][""], CultureInfo.InvariantCulture));

        }
        var Maxvotes = votes.FirstOrDefault(x => x.Value == votes.Values.Max()).Key;
        return Maxvotes;
    }

    public object VoteReg(IndexAndDistance[] info,
    List<Dictionary<string, object>> trainData, int k)
    {
        kPoints = new List<int>();
        double sum = 0.0;
        for (int i = 0; i < k; ++i)
        {
            int idx = info[i].idx;
            double c = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 1]], CultureInfo.InvariantCulture);
            sum += c;
            kPoints.Add(Convert.ToInt32(trainData[idx][""], CultureInfo.InvariantCulture));
        }
        return sum / k;
    }

    public double Distance(double[] unknown,
      Dictionary<string, object> data)
    {

        double sum = 0.0;
        for (int i = 0; i < unknown.Length; ++i)
        {

            sum += (unknown[i] - Convert.ToDouble(data[attributes[i + 1]], CultureInfo.InvariantCulture)) * (unknown[i] - Convert.ToDouble(data[attributes[i + 1]], CultureInfo.InvariantCulture));
        }
        return Math.Sqrt(sum);
    }

    public object WeightClass(IndexAndDistance[] info,
    List<Dictionary<string, object>> trainData, int k)
    {

        Dictionary<string, double> votes = new Dictionary<string, double>(); // One cell per class
        for (int i = 0; i < k; ++i)
        {       // Just first k
            int idx = info[i].idx;            // Which train item
            string c = (string)trainData[idx][attributes[attributes.Count - 1]];   // Class in last cell
            if (votes.ContainsKey(c))
            {
                ++votes[c];
            }
            else
            {
                votes.Add(c, (votes[c] += info[i].dist));
            }
            //kPoints.Add(Convert.ToInt32(trainData[idx][""], CultureInfo.InvariantCulture));

        }
        var Maxvotes = votes.FirstOrDefault(x => x.Value == votes.Values.Max()).Key;
        return Maxvotes;


    }

    public object WeightReg(IndexAndDistance[] info,
List<Dictionary<string, object>> trainData, int k)
    {
        double sumWeight = 0.0;
        double sumWeightXReg = 0.0;

        for (int i = 0; i < k; ++i)
        {
            int idx = info[i].idx;
            double weight = (1 / Math.Sqrt(info[i].dist));
            sumWeight += weight; 
            double reg = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 1]], CultureInfo.InvariantCulture);
            sumWeightXReg += weight * reg;
        }
        
        return sumWeightXReg/sumWeight;
    }
} // Program class

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

