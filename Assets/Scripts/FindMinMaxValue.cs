using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class FindMinMaxValue
{
    public static float FindMaxValue(string columnName, List<Dictionary<string, object>> pointList)
    {
        //set initial value to first value
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
            catch (Exception)
            {
                pointList[i][columnName] = pointList[i - 1][columnName];
            }
        }

        //Spit out the max value
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
            catch (Exception)
            {
                pointList[i][columnName] = pointList[i - 1][columnName];
            }
        }

        return minValue;
    }
}
