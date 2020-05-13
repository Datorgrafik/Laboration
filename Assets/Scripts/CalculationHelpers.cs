using System;
using System.Collections.Generic;
using System.Globalization;

public static class CalculationHelpers
{
    #region Methods

    public static float FindMaxValue(string columnName, List<Dictionary<string, object>> pointList)
    {
        string maxValueString = pointList[0][columnName].ToString();
        float maxValue = float.Parse(maxValueString, CultureInfo.InvariantCulture);

        //Loop through Dictionary, overwrite existing maxValue if new value is larger
        for (var i = 0; i < pointList.Count; i++)
        {
            string maxValueStringLoop = pointList[i][columnName].ToString();

            try
            {
                if (maxValue < float.Parse(maxValueStringLoop, CultureInfo.InvariantCulture))
                    maxValue = float.Parse(maxValueStringLoop, CultureInfo.InvariantCulture);
            }
            // Catches missing values i.e. '?' that cannot be converted to floats in the dataset.
            catch (Exception)
            {
                // Removes the instance with the missing value
                pointList.RemoveAt(i);
            }
        }

        return maxValue;
    }

    public static float FindMinValue(string columnName, List<Dictionary<string, object>> pointList)
    {
        string minValueString = pointList[0][columnName].ToString();
        float minValue = float.Parse(minValueString, CultureInfo.InvariantCulture);

        //Loop through Dictionary, overwrite existing minValue if new value is smaller
        for (var i = 0; i < pointList.Count; i++)
        {
            string minValueStringLoop = pointList[i][columnName].ToString();

            try
            {
                if (float.Parse(minValueStringLoop, CultureInfo.InvariantCulture) < minValue)
                    minValue = float.Parse(minValueStringLoop, CultureInfo.InvariantCulture);
            }
            // Catches missing values i.e. '?' that cannot be converted to floats in the dataset.
            catch (Exception)
            {
                // Removes the instance with the missing value
                pointList.RemoveAt(i);
            }
        }

        return minValue;
    }

    public static string FindAverage(string attribute, List<Dictionary<string, object>> pointlist)
    {
        double sum = 0.0;
        int n = 0;
        
        for (int i = 0; i < pointlist.Count - 1; ++i)
        {
            sum += Convert.ToDouble(pointlist[i][attribute], CultureInfo.InvariantCulture);
            ++n;
        }

        return Convert.ToString(Math.Round((sum / (pointlist.Count - 1)), 2), CultureInfo.InvariantCulture);
    }

    #endregion
}
