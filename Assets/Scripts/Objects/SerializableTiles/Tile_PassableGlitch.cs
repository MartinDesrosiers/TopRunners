using UnityEngine;

[System.Serializable]
public class Tile_PassableGlitch : Tile {
	public override bool isExtended {
		get {
			return true;
		}
	}

	public ushort fakeType;
	public ushort fakeId;

	public override void Serialize(GameObject tObj) {
		//Debug.Log("Serialize Passable");
		fakeType = tObj.GetComponent<Passable>().type;
		fakeId = tObj.GetComponent<Passable>().id;
	}

	public override void Deserialize(ref GameObject tObj) {
		//Debug.Log("Deserialize Passable1");
	}

	public void Deserialize(ref GameObject tObj, TileManager tTileManager) {
		//Debug.Log("Deserialize Passable2");
		GameObject tObj2;
		tTileManager.GetTile(fakeType, fakeId, out tObj2, tObj.transform.position);
		
		GameObject.Destroy(tObj2.GetComponent<BoxCollider2D>());
		GameObject.Destroy(tObj2.GetComponent<EdgeCollider2D>());
		GameObject.Destroy(tObj2.GetComponent<PolygonCollider2D>());
		MonoBehaviour[] scripts = tObj2.GetComponents<MonoBehaviour>();
		for(int i = 0; i < scripts.Length; i++)
			GameObject.Destroy(scripts[i]);

		tObj2.transform.parent = tObj.transform;
		tObj.GetComponent<Passable>().type = fakeType;
		tObj.GetComponent<Passable>().id = fakeId;
		if(tObj2.tag == "Connectable" || tObj2.tag == "Connector")
			tObj.tag = tObj2.tag;
	}
}