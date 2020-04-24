using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regression : DataClass
{
    public Regression(List<Dictionary<string, object>> data)
    : base(data)
    {

    }

    public override object Knn(double[] unknown, string k, bool weightedOrNot)
    {
        KNN knn = new KNN(unknown, k, weightedOrNot);
        return knn.ClassifyReg(unknown, CSV);
    }
}
