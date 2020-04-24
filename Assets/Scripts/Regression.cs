using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regression : DataClass
{
    public Regression(List<Dictionary<string, object>> data)
    : base(data)
    {

    }

    public override object Knn(double[] unknown)
    {
        KNN knn = new KNN();
        return knn.ClassifyReg(unknown, CSV);
    }
}
