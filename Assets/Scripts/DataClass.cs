using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataClass 
{
    public List<Dictionary<string, object>> CSV { get; set; }

    public DataClass (List<Dictionary<string, object>> data)
    {
        CSV = data;
    }

    public abstract object Knn(double[] unknown, string k, bool weightedOrNot);
}
