using UnityEngine;

public class Teleport : MonoBehaviour {

	public Vector2 destination;
	public bool isEntrance;

	private bool _isRemoveActive = true;
	
	private void OnTriggerEnter2D(Collider2D col) {
		if(col.tag == "Player" && isEntrance)
			col.transform.parent.parent.transform.position = destination;
	}

	//Needed to prevent NullReference errors.
	private void OnApplicationQuit() {
		_isRemoveActive = false;
	}

	private void OnDestroy() {
		if(_isRemoveActive && !LevelManager.Instance.isReloading) {
			int[] tColRow = new int[2];
			tColRow[0] = (int)(destination.x / 10);
			tColRow[1] = (int)(destination.y / 10);
			for(int i = 0; i < LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]].Count; i++) {
				if(LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].transform.position == new Vector3(destination.x, destination.y, 0.0f)) {
					LevelManager.Instance.DeleteObject(tColRow, i);
				}
			}
		}
	}
}