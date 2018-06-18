using UnityEngine;
using System.Collections.Generic;

public class TileManager : MonoBehaviour {
	
	public List<GameObject> enemyList;
	public List<GameObject> collectibleList;
	public List<GameObject> glitchList;
	public List<GameObject> blockList;
	public List<GameObject> objectList;

    public Tile GetTile(ushort type, ushort id, out GameObject tObj, Vector2 tPos) {
		tObj = GetObjectList(type)[id];
		tObj = Instantiate(tObj, tPos, Quaternion.identity);

        return GetTile(tObj.name);
	}

    // This function is deprecated
    // Replaced by GetUnlocksList()
    public bool[] GetAvailableObjects(ushort type) {
		List<GameObject> tObjectList = GetObjectList(type);
		bool[] availableObjects = new bool[tObjectList.Count];


        for (int i = 0; i < tObjectList.Count; i++) {
			if(tObjectList[i] != null)
				availableObjects[i] = true;
			else
				availableObjects[i] = false;
		}

        return availableObjects;
	}

	//Get tile type from prefab name.
	private Tile GetTile(string tObjName) {
		Tile tTile;

		if(tObjName.Contains("CommentBlock"))
			tTile = new Tile_CommentBlock();
		else
			tTile = new Tile_Default();

		return tTile;
	}

    // This functions returns the list of all level editor items
    // The length of each array is used to display the correct number of items
    // It also says if they are unlocked or not
    // Lock objects are to be unlocked by a player
    // These arrays must be saved
    // False = Unlocked / True = Locked
    public bool[] GetUnlocksList(ushort type) {
        bool[] unlockedEntities;

        switch (type) {
            case 0:
                //Enemies
                unlockedEntities =  new bool[] { false, false, false, false, true, true };
                break;

            case 1: 
                //Collect
                unlockedEntities =  new bool[] { false, false, false, false, true, true };
                break;

            case 2:
                //Grounds
                unlockedEntities =  new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false , false };
                break;

            case 3:
                //Glitches
                unlockedEntities =  new bool[] { false, false, false, false, false, true, true, true, true, true };
                break;

            case 4:
                //Objects
                unlockedEntities =  new bool[] { false, false, false, false, false, true, true, true, true, true, true, true };
                break;

            case 5:
                //Levels
                unlockedEntities = new bool[] { false, true, true, true, true, true, true, false, false, false };
                break;

            default:
                Debug.Log("Invalid Category Type");
                unlockedEntities = new bool[] { false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false, false };
                break;
        }
        return unlockedEntities;
    }


    private List<GameObject> GetObjectList(ushort type) {
		List<GameObject> tObjectList;

		switch(type) {
			case 0:
				tObjectList = enemyList;
			break;
                
            case 1:
                tObjectList = collectibleList;
                break;

            case 2:
                tObjectList = blockList;
                break;

            case 3:
                tObjectList = glitchList;
                break;

            case 4:
				tObjectList = objectList;
                break;

			default:
				Debug.Log("Error loading Object. Type is invalid.");
				tObjectList = blockList;
                break;
		}


		return tObjectList;
	}
}