using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regression : DataClass
{
    public Regression(List<Dictionary<string, object>> data)
    : base(data)
    {

    }

    public override object Knn(double[] unknown, int k)
    {
        return kNN.ClassifyReg(unknown, CSV, k);
    }
}
