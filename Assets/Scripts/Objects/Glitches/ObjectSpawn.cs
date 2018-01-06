using UnityEngine;

public class ObjectSpawn : MonoBehaviour {
	
	private bool _isOn = true;

	private void OnTriggerEnter2D(Collider2D col) {
		if(col.tag == "Player" && _isOn) {
			//Only spawns an object once.
			_isOn = false;

			GameObject tObj;
			//Get a random prefab from the collectible list in tileManager.
			LevelManager.Instance.tileManager.GetTile(1, (ushort)(Random.Range(0, 8)), out tObj, transform.position);
			//Instantiate the prefab and make it a child of map container.
			tObj.transform.parent = LevelManager.Instance.mapContainer.transform;
		}
	}
}