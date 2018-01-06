using UnityEngine;

[System.Serializable]
public class Tile_SpawnPoint : Tile {
	public override bool isExtended {
		get {
			return true;
		}
	}

	public bool isFacingRight = true;

	public override void Serialize(GameObject tObj) {
		Debug.Log("Serialize Spawpoint");
		isFacingRight = tObj.GetComponent<SpawnPoint>().isFacingRight;
	}

	public override void Deserialize(ref GameObject tObj) {
		Debug.Log("Deserialize Spawpoint");
		tObj.GetComponent<SpawnPoint>().isFacingRight = isFacingRight;
	}
}