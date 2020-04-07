using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DatasetLoader
{
	private List<string[]> datasetList = new List<string[]>();

	private string filePath;
	
	public DatasetLoader(string filePath = @"Assets/Datasets/Iris.csv")
	{
		this.filePath = filePath;
	}

	public List<string[]> GetDataset()
	{
		using (var reader = new StreamReader(filePath))
		{
			while (!reader.EndOfStream)
			{
				var line = reader.ReadLine();
				var values = line.Split(',');

				datasetList.Add(values);
			}
		}

		return datasetList;
	}
}
