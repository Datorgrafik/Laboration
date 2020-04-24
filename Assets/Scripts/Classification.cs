﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classification : DataClass
{
    public Classification(List<Dictionary<string, object>> data)
    : base(data)
    {

    }

    public override object Knn(double[] unknown)
    {
        KNN knn = new KNN();
        return knn.ClassifyClass(unknown, CSV);
    }
}
