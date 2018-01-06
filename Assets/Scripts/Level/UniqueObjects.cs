using UnityEngine;

public class UniqueObjects {

	public class UniqueObject {
		public bool isUsed = false;
		public GameObject obj;
		public int[] colRow;
	}

	public enum Mode { Add, Delete };

	private UniqueObject _spawnPoint = new UniqueObject();
	private UniqueObject _end = new UniqueObject();
	//private UniqueObject _enemyBoss = new UniqueObject();
	public UniqueObject EndPoint { get { return _end; } }

	public bool CheckUniqueObject(int[] tColRow, GameObject tObject, Mode mode, bool checkPoint) {
		if(tObject.name.Contains("SpawnPoint")) {
			if(mode == Mode.Add) {
				if(_end.isUsed && Mathf.Abs((_end.obj.transform.position - tObject.transform.position).magnitude) < 10)
					return false;

				if(_spawnPoint.isUsed) {
					for(int i = 0; i < LevelManager.Instance.levelData.objectList[_spawnPoint.colRow[0]][_spawnPoint.colRow[1]].Count; i++) {
						if(tObject.transform.position != _spawnPoint.obj.transform.position) {
							if(LevelManager.Instance.levelData.objectList[_spawnPoint.colRow[0]][_spawnPoint.colRow[1]][i].transform.position == new Vector3(_spawnPoint.obj.transform.position.x, _spawnPoint.obj.transform.position.y, 0.0f)) {
								LevelManager.Instance.DeleteObject(_spawnPoint.colRow, i);
								break;
							}
						}
					}
				}

				_spawnPoint.isUsed = true;
				_spawnPoint.obj = tObject;
				_spawnPoint.colRow = tColRow;

				if(!checkPoint)
					LevelManager.Instance.spawnPoint = tObject.transform.position;

				return true;
			}
			else {
				if(_spawnPoint.isUsed) {
					_spawnPoint.isUsed = false;

                    if (!checkPoint)
                        LevelManager.Instance.spawnPoint = new Vector2(4,4);

                    return true;
				}
			}
		}
		else if(tObject.name.Contains("End")) {
			if(mode == Mode.Add) {
				if(_spawnPoint.isUsed && Mathf.Abs((_spawnPoint.obj.transform.position - tObject.transform.position).magnitude) < 10)
					return false;

				if(_end.isUsed) {
					for(int i = 0; i < LevelManager.Instance.levelData.objectList[_end.colRow[0]][_end.colRow[1]].Count; i++) {
						if(tObject.transform.position != _end.obj.transform.position) {
							if(LevelManager.Instance.levelData.objectList[_end.colRow[0]][_end.colRow[1]][i].transform.position == new Vector3(_end.obj.transform.position.x, _end.obj.transform.position.y, 0.0f)) {
								LevelManager.Instance.DeleteObject(_end.colRow, i);
								break;
							}
						}
					}
				}

				_end.isUsed = true;
				_end.obj = tObject;
				_end.colRow = tColRow;

				return true;
			}
			else {
				if(_end.isUsed) {
					_end.isUsed = false;

					return true;
				}
			}
		}
		else
			return true;

		return false;
	}
}