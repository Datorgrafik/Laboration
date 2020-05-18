using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataClass 
{
    #region Attributes

    public List<Dictionary<string, object>> CSV { get; set;}

    #endregion

    #region Methods

    public DataClass(List<Dictionary<string, object>> data)
    {
        CSV = data;
    }

    public abstract object Knn(double[] unknown);

    #endregion
}
