using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorManager : MonoBehaviour
{
	#region Attributes

	// ColorList
	private static readonly Color[] colorList =
	{
        // Peter River (Blue)
        new Color(52, 152, 219, 1),
        // Pomegranate (Red)
        new Color(192, 57, 43,1),
        // Emerald (Green)
        new Color(46, 204, 113,1),
        // Turquoise
        new Color(26, 188, 156,1),
        // Amethyst (Purple)
        new Color(155, 89, 182,1),
        // Wet Asphalt (Gray)
        new Color(52, 73, 94,1),
        // Sun Flower (Yellow)
        new Color(241, 196, 15,1),
        // Carrot (Orange)
        new Color(230, 126, 34,1),
        // Silver
        new Color(189, 195, 199,1),
        // Concrete (Gray)
        new Color(149, 165, 166,1)
	};

	#endregion

	#region Methods

	public static void ChangeColor(GameObject dataPoint, int targetFeatureIndex)
	{
		dataPoint.GetComponent<Renderer>().material.color = new Color(colorList[targetFeatureIndex].r / 255,
																		colorList[targetFeatureIndex].g / 255,
																		colorList[targetFeatureIndex].b / 255, 1.0f);
	}

	// Overloaded for ParallelCoordinatePlotter
	public static Color ChangeColor(int targetFeatureIndex)
	{
		return new Color(colorList[targetFeatureIndex].r / 255,
						colorList[targetFeatureIndex].g / 255,
						colorList[targetFeatureIndex].b / 255, 1.0f);
	}

	public static void Blink(List<int> kPoints, List<Dictionary<string, object>> pointList)
	{
		foreach (int data in kPoints)
		{
			GameObject ball = (GameObject)pointList[data - 1]["DataBall"];
			ball.GetComponent<Blink>().enabled = true;
		}
	}

	#endregion
}
