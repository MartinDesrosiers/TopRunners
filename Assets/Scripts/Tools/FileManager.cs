using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// This script contains methods needed to save and load levels.
/// SaveLevel()
///		- String tName : Name of the file to be saved i.e. level1.gd, ".gd" being the level extension.
///		- LevelProperties tLProp : Level to be saved ( LevelProperties contains all informations needed to save a level ).
///	LoadLevel()
///		- string tName : Name of the file to be opened i.e. level1.gd, ".gd" being the level extension.
///		- out LevelProperties tLProp : Container LevelProperties to be filled by the data in the opened file.
/// </summary>
public static class FileManager {

	public static void SaveLevel(string tName, SerializedLevelData tLProp) {
		CheckDirectory("/Levels");

		BinaryFormatter bf = new BinaryFormatter();
		//Creates a file with given level name.
		FileStream file = File.Create(Application.persistentDataPath + "/Levels/" + tName);
		//Uses created file to write current level data.
		bf.Serialize(file, tLProp);
		file.Close();
	}

	public static bool LoadLevel(string tName, out SerializedLevelData tLProp) {
		if(tName != "Template.sld" && tName != "Tutorial.sld" && tName != "Demo ( Easy )" && tName != "Demo ( Hard )") {
			CheckDirectory("/Levels");

			//Checks if level exists.
			if(!File.Exists(Application.persistentDataPath + "/Levels/" + tName)) {
				Debug.Log("Error loading level. Level cannot be found.");
				tLProp = null;
				return false;
			}

			BinaryFormatter bf = new BinaryFormatter();
			//Opens level file according to given file name.
			FileStream file = File.Open(Application.persistentDataPath + "/Levels/" + tName, FileMode.Open);
			//Uses opened file to paste level data in container tLProp.
			tLProp = (SerializedLevelData)bf.Deserialize(file);
			file.Close();
		}
		else {
			//Checks if level exists.
			if(!File.Exists(Application.streamingAssetsPath + "/" + tName)) {
				Debug.Log("Error loading template. Level cannot be found.");
				tLProp = null;
				return false;
			}

			BinaryFormatter bf = new BinaryFormatter();
			//Opens level file according to given file name.
			FileStream file = File.Open(Application.streamingAssetsPath + "/" + tName, FileMode.Open);
			//Uses opened file to paste level data in container tLProp.
			tLProp = (SerializedLevelData)bf.Deserialize(file);
			file.Close();
		}

		return true;
	}

	public static void DeleteLevel(string tName) {
		CheckDirectory("/Levels");

		// Avoid deleting the directory.
		if (string.IsNullOrEmpty (tName)) {
			return;
		}

		string path = Application.persistentDataPath + "/Levels/" + tName;
		if (File.Exists(path))
			File.Delete(path);
	}

	public static void CheckDirectory(string path) {
		if(!Directory.Exists(Application.persistentDataPath + path))
			Directory.CreateDirectory(Application.persistentDataPath + path);
	}
}