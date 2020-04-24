using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classification : DataClass
{
    #region Attributes

    #endregion

    #region Methods

    public Classification(List<Dictionary<string, object>> data)
    : base(data) { }

    public override object Knn(double[] unknown, string k, bool weightedOrNot)
    {
        KNN knn = new KNN(unknown, k, weightedOrNot);
        return knn.ClassifyClass(unknown, CSV);
    }

    #endregion
}
