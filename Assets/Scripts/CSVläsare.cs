using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Globalization;
using System.Linq;

public class CSVläsare : MonoBehaviour
{
    // List of id, feature and targetfeature
	public static List<string> columnList;

    // List of target features
	public static List<string> targetFeatures;

    // class of the datser reg or classification
	public static DataClass dataClass;

    // List of all datapoints
	public static List<Dictionary<string, object>> pointList;


	// Define delimiters, regular expression craziness
	static string SPLIT_RE = @",(?=(?:[^""]*""[^""]*"")*(?![^""]*""))";
	// Define line delimiters, regular experession craziness
	static string LINE_SPLIT_RE = @"\r\n|\n\r|\n|\r";
	static char[] TRIM_CHARS = { '\"' };

	public static void Read(string file)
	{
		//Declare dictionary list
		var list = new List<Dictionary<string, object>>();

		// Split data.text into lines using LINE_SPLIT_RE characters
		var lines = Regex.Split(file, LINE_SPLIT_RE);
		//Split header (element 0)
		var header = Regex.Split(lines[0], SPLIT_RE);

		// Loops through lines
		for (var i = 1; i < lines.Length; i++)
		{
			//Split lines according to SPLIT_RE, store in var (usually string array)
			var values = Regex.Split(lines[i], SPLIT_RE);

			// Skip to end of loop (continue) if value is 0 length OR first value is empty
			if (values.Length == 0 || values[0] == "")
				continue;

			var entry = new Dictionary<string, object>(); // Creates dictionary object

			// Loops through every value
			for (var j = 0; j < header.Length && j < values.Length; j++)
			{
				// Set local variable value
				string value = values[j];
				// Trim characters
				value = value.TrimStart(TRIM_CHARS).TrimEnd(TRIM_CHARS).Replace("\\", "");
				// Set final value
				object finalvalue = value;

				// If-else to attempt to parse value into int or float
				if (int.TryParse(value, out int n))
					finalvalue = n;
				else if (float.TryParse(value, out float f))
					finalvalue = f;

				entry[header[j]] = finalvalue;
			}

			// Add Dictionary ("entry" variable) to list
			list.Add(entry);
		}

		pointList = list;

		float r;

		columnList = new List<string>(list[0].Keys);

		if (float.TryParse(list[list.Count - 1][columnList[columnList.Count - 1]].ToString(), NumberStyles.Any, CultureInfo.InvariantCulture, out r))
			dataClass = new Regression(pointList);
		else
			dataClass = new Classification(pointList);

		targetFeatures = new List<string>();


		for (int i = 0; i < list.Count; i++)
			targetFeatures.Add(list[i][columnList[columnList.Count - 1]].ToString());

		// Only keep distinct targetFeatures
		targetFeatures = targetFeatures.Distinct().ToList();

	}
}
