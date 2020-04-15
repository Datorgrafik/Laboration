using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;

//Plagiatvarning här med?

public class kNN 
{
    private static List<string> attributes;

    static public object ClassifyReg(double[] unknown,
List<Dictionary<string, object>> trainData, int k)
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

        object result = VoteReg(info, trainData, k);
        return result;
    }

    static public object ClassifyClass(double[] unknown,
List<Dictionary<string, object>> trainData, int k)
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

        object result = Vote(info, trainData, k);
        return result;
    }

    static object Vote(IndexAndDistance[] info,
      List<Dictionary<string, object>> trainData, int k)
    {

        Dictionary<string, int> votes = new Dictionary<string, int>(); // One cell per class
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
                votes.Add(c, 1);
            }
        }
        var Maxvotes = votes.FirstOrDefault(x => x.Value == votes.Values.Max()).Key;
        return Maxvotes;
    }

    static object VoteReg(IndexAndDistance[] info,
    List<Dictionary<string, object>> trainData, int k)
    {
       
        double sum = 0.0;
        for (int i = 0; i < k; ++i)
        {
            int idx = info[i].idx;
            double c = Convert.ToDouble(trainData[idx][attributes[attributes.Count - 1]], CultureInfo.InvariantCulture);
            sum += c;

        }
        return sum / k;
    }


    static double Distance(double[] unknown,
      Dictionary<string, object> data)
    {
        
        double sum = 0.0;
        for (int i = 1; i < unknown.Length; ++i)
        {

            sum += (unknown[i] - Convert.ToDouble(data[attributes[i]], CultureInfo.InvariantCulture)) * (unknown[i] - Convert.ToDouble(data[attributes[i]], CultureInfo.InvariantCulture));
        }
        return Math.Sqrt(sum);
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

