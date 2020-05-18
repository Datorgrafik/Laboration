using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Regression : DataClass
{
	#region Methods

	public Regression(List<Dictionary<string, object>> data)
	: base(data) { }

	public override object Knn(double[] unknown, string k, bool weightedOrNot)
	{
		KNN.kValue = Convert.ToInt32(k);
		KNN.trueOrFalse = weightedOrNot;
		return KNN.ClassifyReg(unknown, CSV);
	}

	#endregion
}
