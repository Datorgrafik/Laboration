using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DataClass 
{
    #region Attributes

    public List<Dictionary<string, object>> CSV { get; set; }

    #endregion

    #region Methods

    public DataClass(List<Dictionary<string, object>> data)
    {
        CSV = data;
    }

<<<<<<< HEAD
    public abstract object Knn(double[] unknown, string k, bool weightedOrNot);
=======
    public abstract object Knn(double[] unknown);

    #endregion
>>>>>>> 22739ca145c1e46963ba50647d63d119ce40432e
}
