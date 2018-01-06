using UnityEngine;
using System.Collections.Generic;

public class LevelData {
	/// <summary>
	/// The objectList works as 10x10 sectors.
	///		i.e. An object with a world position of [42,18] would be set in objectList[4][1][object].
	/// The first list represents Columns, the second list represents Rows and the third list
	///		contains all objects inside the specified sector.
	/// </summary>
	public List<List<List<GameObject>>> objectList;
	public Color[][] colorScheme;


	public LevelData() {
		Initialize();
	}


	public void Initialize() {
        if (objectList != null)
        {
            Debug.Log("Tabarnak");
            objectList.Clear();
        }
		
		//Initialization of the object list.
		objectList = new List<List<List<GameObject>>>();
		for(int i = 0; i < LevelManager.Instance.mapSize.x / 10; i++) {
			objectList.Add(new List<List<GameObject>>());
			for(int j = 0; j < LevelManager.Instance.mapSize.y / 10; j++) {
				objectList[i].Add(new List<GameObject>());
			}
		}

		//Enemy color scheme.
		//To be replaced with premade sprites.
		colorScheme = new Color[8][];
		for(int i = 0; i < colorScheme.Length; i++) {
			colorScheme[i] = new Color[2];
			for(int k = 0; k < colorScheme[i].Length; k++)
				colorScheme[i][k] = new Color();
		}
	}
}
