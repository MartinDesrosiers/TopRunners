using UnityEngine;

[System.Serializable]
public class Tile_TeleportGlitch : Tile {
	public override bool isExtended { get { return true; } }

	public float destinationX;
	public float destinationY;
	public bool isEntrance;

	public override void Serialize(GameObject tObj) {
		Teleport tTeleport = tObj.GetComponent<Teleport>();
		destinationX = tTeleport.destination.x;
		destinationY = tTeleport.destination.y;
		isEntrance = tTeleport.isEntrance;
	}

	public override void Deserialize(ref GameObject tObj) {
		tObj.GetComponent<Teleport>().destination = new Vector2(destinationX, destinationY);
		tObj.GetComponent<Teleport>().isEntrance = isEntrance;

		if(isEntrance)
			tObj.transform.GetChild(0).gameObject.SetActive(true);
		else
			tObj.transform.GetChild(1).gameObject.SetActive(true);
	}
}