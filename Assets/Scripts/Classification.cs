﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Classification : DataClass
{
    public Classification(List<Dictionary<string, object>> data)
    : base(data)
    {

    }

    public override object Knn(double[] unknown, int k)
    {
        return kNN.ClassifyClass(unknown, CSV, k);
    }
}