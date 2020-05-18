using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
    // ColorList
    private static readonly Color[] colorList =
    {
        new Color(52, 152, 219, 1),
        new Color(192, 57, 43,1),
        new Color(46, 204, 113,1),
        new Color(26, 188, 156,1),
        new Color(155, 89, 182,1),
        new Color(52, 73, 94,1),
        new Color(241, 196, 15,1),
        new Color(230, 126, 34,1),
        new Color(189, 195, 199,1),
        new Color(149, 165, 166,1)
    };

    public static void ChangeColor(GameObject dataPoint, int targetFeatureIndex)
    {
        dataPoint.GetComponent<Renderer>().material.color = new Color(colorList[targetFeatureIndex].r / 255, colorList[targetFeatureIndex].g / 255, colorList[targetFeatureIndex].b / 255, 1.0f);
    }

    public static void Blink(List<int> kPoints, List<Dictionary<string, object>> pointList)
    {
        foreach (int data in kPoints)
        {
            GameObject ball = (GameObject)pointList[data - 1]["DataBall"];
            ball.GetComponent<Blink>().enabled = true;
        }
    }
}
