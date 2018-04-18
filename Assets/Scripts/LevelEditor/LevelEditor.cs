using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class LevelEditor : MonoBehaviour {
	public InputField newLevelName;
    Transform transObj;
    public ushort objType;
	public ushort objId;
    public bool cursor, eraser;
	public GlitchUI glitchUI;
	public bool isLocked = false;

	public void CreateNewLevel() {
		if(newLevelName.text.Length < 20 && newLevelName.text.Length > 4) {
			//Directory containning all the levels.
			DirectoryInfo directoryInfo = new DirectoryInfo(Application.persistentDataPath + "/Levels");
			//Creates an array of files with all the files inside "/Levels".
			FileInfo[] fileInfo = directoryInfo.GetFiles();

			bool isAvailable = true;
			foreach(FileInfo k in fileInfo) {
				if(k.Name == newLevelName + ".sld")
					isAvailable = false;
			}

			if(isAvailable) {
				ClearLevel();
				FileManager.SaveLevel(newLevelName.text + ".sld", new SerializedLevelData());
				GameManager.Instance.currentLevel = newLevelName.text + ".sld";
				LoadLevel();
			}
			else
				Debug.Log("Level name is not available.");
		}
		else
			Debug.Log("Level name must be between 4 and 20 characters.");

	}

	/// <summary>
	/// Loads the level from the serialized data.
	/// </summary>
	public void LoadLevel() {
		LevelManager.Instance.LoadSerializedData();
	}

	/// <summary>
	/// Loads the level from the database using the unique Id.
	/// </summary>
	/// <param name="uniqueId">Unique id.</param>
//	public void LoadLevel(string uniqueId) {
//		LevelManager.Instance.LoadLevelDataFromDb(uniqueId, levelData => {
//			if (levelData != null) {
//				Debug.Log(string.Format("Successfully loaded level {0}.", uniqueId));
//
//				LevelManager.Instance.serializedData = levelData;
//				LoadLevel();
////				LevelManager.Instance.DeserializeLevelData();
//			} else {
//				Debug.Log(string.Format("Could not load level {0}.", uniqueId));
//			}
//		});
//    }

	public void ClearLevel() {
		//Allows to start directly in the LevelEditor Scene.
		LevelManager.Instance.InitializeLevel();
	}


	public void SetCursor(bool isActive) {
		cursor = isActive;
		Camera.main.GetComponent<CameraController>().isControlActive = cursor;
	}


	public void DragObject() {
		//If cursor isn't over UI;
		bool tPointerOverUI;
#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
		tPointerOverUI = EventSystem.current.IsPointerOverGameObject();
#else
			tPointerOverUI = IsPointerOverUIObject();
#endif

		if(!tPointerOverUI) {
			int[] objColRow = new int[2];   //0 = column, 1 = row
			float[] objPos = new float[2];  //0 = x, 1 = y
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            GetObjPosition(ref objColRow, ref objPos, mousePosition);
            AddDeleteTile(objColRow, objPos, true);
		}
	}


	private void Awake() {
		objType = 2;
		objId = 2;
	}


	private void Start() {
		cursor = eraser = false;
		LevelManager.Instance.isReloading = true;
		ClearLevel();
		LoadLevel();
		LevelManager.Instance.isReloading = false;
	}


	private void Update() {
		if(!isLocked) {
			if(GameManager.Instance.currentState == GameManager.GameState.LevelEditor) {

				//If cursor isn't over UI;
				bool tPointerOverUI;

#if UNITY_STANDALONE || UNITY_WEBPLAYER || UNITY_EDITOR
				tPointerOverUI = EventSystem.current.IsPointerOverGameObject();
                if (Input.GetMouseButtonDown(0))
                {
                    //Check if touch an object no matter where you press on it;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    RaycastHit2D[] hit = Physics2D.GetRayIntersectionAll(ray, 10f);
                    foreach (RaycastHit2D o in hit)
                    {
                        transObj = o.transform;
                        break;
                    }
                }            
#else
				tPointerOverUI = IsPointerOverUIObject();
#endif
                if (!tPointerOverUI) {
                    int[] objColRow = new int[2];   //0 = column, 1 = row
					float[] objPos = new float[2];  //0 = x, 1 = y
                    if (transObj != null) {
                        GetObjPosition(ref objColRow, ref objPos, transObj.position);
                        transObj = null;
                    }
                    else {
                        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                        GetObjPosition(ref objColRow, ref objPos, mousePosition);
                    }

                    if (LevelEditorInputs.GetBrush()) {
						if(eraser)
							AddDeleteTile(objColRow, objPos, false);
						else if(!cursor) {
                            if (glitchUI.isActive)
                                glitchUI.AddTeleport(objPos);
                            else if(objPos[0] > 0 && objPos[0] < LevelManager.Instance.mapSize.x && objPos[1] > 0 && objPos[1] < LevelManager.Instance.mapSize.y)
								AddDeleteTile(objColRow, objPos, true);
						}
					}
					else if(LevelEditorInputs.GetEraser())
						AddDeleteTile(objColRow, objPos, false);
				}
				GlobalInputs.ClearInputs();
            }
		}
	}


	//MOBILE ONLY
	//Called on player input. Returns true if the player's finger(s) is over part of the UI.
	private bool IsPointerOverUIObject() {
		//Get current event data and set the pointer position.
		PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
		eventDataCurrentPosition.position = Input.mousePosition;

		//Container for raycast results.
		List<RaycastResult> results = new List<RaycastResult>();
		//Detect all UI objects below the player's finger(s).
		EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
		
		return results.Count > 0;
	}


	private void GetObjPosition(ref int[] tColRow, ref float[] tPos, Vector2 trans) {
		tColRow[0] = (int)trans.x;
		tColRow[1] = (int)trans.y;
		tPos[0] = tColRow[0] + 0.5f;
		tPos[1] = tColRow[1] + 0.5f;
		tColRow[0] /= 10;
		tColRow[1] /= 10;
	}


	/// <summary>
	/// Using the param tColRow, this function goes through all the objects in the given 10x10 sector and verifies if their position
	///		is equal to tPos which is the current mouse position or mobile tap input position.
	/// If the player is trying to add a block and an object is found with the same position as tPos,
	///		the player won't be able to add a new block since the spot is already taken.
	///	If the player is trying to delete a block and an object is found with the same position as tPos,
	///		the object will be deleted.
	/// </summary>
	/// <param name="tColRow">
	///		This is 10x10 the sector in which the object to add / delete is in.
	///		Used to reduce the load when parsing through existing objects since instead of going through all existing objects,
	///			it only goes through a 10x10 section.
	/// </param>
	/// <param name="tPos">Position of the object to add / delete.</param>
	/// <param name="isAdd">Is the player trying to add a block or delete one.</param>
	private void AddDeleteTile(int[] tColRow, float[] tPos, bool isAdd) {
		Vector3 rotation = Vector3.zero;
        for (int i = 0; i < LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]].Count; i++) {
			if(LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].transform.position == new Vector3(tPos[0], tPos[1], 0.0f)) {
				if(isAdd) {
					isAdd = false;
					if(objType == 3 && objId == 7) {
						Tile tTile = LevelManager.Instance.serializedData.objectList[tColRow[0]][tColRow[1]][i];
						if(tTile.type != 3 && tTile.type != 0) {
                            LevelManager.Instance.DeleteObject(tColRow, i);
							StartCoroutine(PassableEndFrame(tPos, tColRow, tTile.type, tTile.id));
						}
					}
					else {
						if(LevelEditorInputs.Rotate()) {
							int tRotation = ObjectRotation.GetObectRotation(LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].name);
							if(tRotation < 0) {
								//if(LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].tag == "Enemies")
								//	LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].GetComponent<EnemyAI>().Flip();
								//else {
									rotation = LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].transform.localScale;
									rotation.x *= tRotation;
									LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].transform.localScale = rotation;
									LevelManager.Instance.serializedData.objectList[tColRow[0]][tColRow[1]][i].horizontalMirror = (rotation.x < 0);
								//}
							}
							else {
								rotation = LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].transform.localEulerAngles;
								rotation.z += tRotation;
								LevelManager.Instance.levelData.objectList[tColRow[0]][tColRow[1]][i].transform.localEulerAngles = rotation;
								LevelManager.Instance.serializedData.objectList[tColRow[0]][tColRow[1]][i].rotation = rotation.z;
							}
						}
					}
				}
				else
					LevelManager.Instance.DeleteObject(tColRow, i);
			}
		}
        if (isAdd) {
            if (objType == 3 && objId == 9)
            {
                SetCursor(true);
                glitchUI.TeleportUI(tPos);
            }
            else if (objType != 3 || objId != 7)
            {
                LevelManager.Instance.AddObject(tPos, tColRow, objType, objId);
            }
		}
	}


	private IEnumerator PassableEndFrame(float[] tPos, int[] tColRow, ushort fakeType, ushort fakeId) {
		yield return new WaitForEndOfFrame();
		LevelManager.Instance.AddPassable(tPos, tColRow, fakeType, fakeId);
	}
}