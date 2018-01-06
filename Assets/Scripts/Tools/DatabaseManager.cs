using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

/// <summary>
/// This script contains methods needed to save and load levels to the database.
/// SaveLevel()
///		- String tName : Name of the file to be saved i.e. level1.gd, ".gd" being the level extension.
///		- LevelProperties tLProp : Level to be saved ( LevelProperties contains all informations needed to save a level ).
///	LoadLevel()
///		- string tName : Name of the file to be opened i.e. level1.gd, ".gd" being the level extension.
///		- out LevelProperties tLProp : Container LevelProperties to be filled by the data in the opened file.
/// </summary>
public static class DatabaseManager {

//	public static void SaveLevel(string tName, SerializedLevelData tLProp) {
//
//	}
//
//	public static bool LoadLevel(string tName, out SerializedLevelData tLProp) {
//		
//	}
}