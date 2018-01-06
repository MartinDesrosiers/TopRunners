using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SerializedLevelData {
	/// <summary>
	/// This list contains all map tiles in serializable form.
	/// Used as a container to load level files or to save levels.
	/// The objectList works as 10x10 sectors.
	///		i.e. An object with a world position of [42,18] would be set in objectList[4][1][object].
	/// The first list represents Columns, the second list represents Rows and the third list
	///		contains all objects inside the specified sector.
	/// </summary>
	public List<List<List<Tile>>> objectList;
	public int theme;
	public int scrollerSpeed;


	[System.Serializable]
	public class SerializableColor {
		public float r, g, b, a;

		public SerializableColor() {
			r = g = b = a = 1.0f;
		}

		public Color GetColor() {
			return new Color(r, g, b, a);
		}

		public void SetRGB(float tR, float tG, float tB) {
			r = tR;
			g = tG;
			b = tB;
		}

		public void SetRGBA(float tR, float tG, float tB, float tA) {
			r = tR;
			g = tG;
			b = tB;
			a = tA;
		}
	}
	public SerializableColor[][] colorScheme;


	//Initialize tile list.
	public SerializedLevelData() {
		Initialize();
	}


	public void Initialize() {
		theme = 0;

        if (objectList != null)
        {
            Debug.Log("Salut");
            objectList.Clear();
        }
		objectList = new List<List<List<Tile>>>();
		for(int i = 0; i < LevelManager.Instance.mapSize.x / 10; i++) {
			objectList.Add(new List<List<Tile>>());
			for(int j = 0; j < LevelManager.Instance.mapSize.y / 10; j++) {
				objectList[i].Add(new List<Tile>());
			}
		}

		colorScheme = new SerializableColor[8][];
		for(int i = 0; i < colorScheme.Length; i++) {
			colorScheme[i] = new SerializableColor[2];
			for(int k = 0; k < colorScheme[i].Length; k++)
				colorScheme[i][k] = new SerializableColor();
		}
	}
}