using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public class LevelManager : Singleton<LevelManager> {

	//Needed to prevent non singleton constructor calls.
	protected LevelManager() { }

    public bool isPaused;
    //bool hasBeenInit = false;
    public List<GameObject> monsterList;
    public List<GameObject> destructibleList;
    public List<GameObject> doorList;
    /*public List<GameObject> ghostObjects;
    public GameObject ghostPlayer;
    public GhostReplay ghostReplay;
    public bool isGhostReplayActive = false;
    public List<GameObject> finalGhostObjects = new List<GameObject>();*/
    public GameObject player;
    GameObject loadingScreen;
    GameObject finishScreen;
    GameObject checkPointFlag;
	public LevelData levelData;
	public SerializedLevelData serializedData;
	public Vector2 spawnPoint;
	public Vector2 mapSize = new Vector2(200, 60);
	public TileManager tileManager;
	public GameObject mapContainer;
	public bool isReloading = false;
    public int theme = 0;

	private UniqueObjects _uniqueObjects;
	private TileConnector _tileConnector;
	private ushort _reloadFrame = 0;
    bool newCheckPointSet;
    bool containExit = false;
    public bool finishLoading = false;
    public bool GetNewCheckPointSet { get { return newCheckPointSet; } }
    public bool GetContainExit { get { return containExit; } }
    public UniqueObjects GetUniqueObject { get { return _uniqueObjects; } }

	public enum SortOption {
		Latest,
		Official,
		Id,
		Creator,
		Name,
		Uid
	}

    private void Start()
    {
        checkPointFlag = Instantiate(Resources.Load("Flag", typeof(GameObject)) as GameObject);
    }

    public void InitializeLevel() {
        //Security clean up.
        Destroy(mapContainer);
        CreateMapContainer();
        isPaused = true;
        player = GameObject.Find("PlayerTest");
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
		_tileConnector = GameObject.Find("TileConnector").GetComponent<TileConnector>();
        //Initialize the level data, serialized level data and unique objects list to prevent nullreferences.
		levelData = new LevelData();
		serializedData = new SerializedLevelData();
		_uniqueObjects = new UniqueObjects();

        //ghostObjects = new List<GameObject>();
    }

    /*void InitialzeGhostPlayer()
    {
        ghostPlayer = Instantiate(Resources.Load("GhostPlayer", typeof(GameObject)) as GameObject);
        ghostPlayer.GetComponent<PlayerController>().Initialize();
        ghostPlayer.transform.position = player.transform.position;
        ghostPlayer.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        ghostReplay = new GhostReplay();
        ghostReplay.InitGhostList();
        //ghostReplay.SetGhostList();
    }*/


	public void ReloadLevel()
    {
        isPaused = true; //FIX dan 26 fevrier
        foreach (GameObject o in monsterList)
        {
            o.gameObject.SetActive(true);
            o.transform.position = o.GetComponent<EnemyAI>().GetOrigin;
            o.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        foreach (GameObject o in doorList)
        {
            o.gameObject.SetActive(true);
        }
        foreach (GameObject o in destructibleList)
        {
            o.gameObject.SetActive(true);
        }
        isReloading = true;
        SetParallax();//FIX
        /*finalGhostObjects = ghostObjects;
        for (int i = 0; i < ghostObjects.Count; i++)
            Destroy(ghostObjects[i].gameObject);
        ghostObjects.Clear();
        if (isGhostReplayActive)
        {
            if (!hasBeenInit)
            {
                hasBeenInit = true;
                InitialzeGhostPlayer();
                //player.GetComponent<PlayerController>().inputScript.Reset();
            }
        }*/
        SetEnemiesDynamique(RigidbodyType2D.Dynamic);
        System.GC.Collect();
        System.GC.WaitForPendingFinalizers();
    }
    void CreateMapContainer()
    {
        mapContainer = new GameObject();
        mapContainer.name = "Map Container";
    }

	//Reset all the level variables.
	public void ClearLevel() {
		isPaused = true;
		player = null;
        levelData.objectList.Clear();
        levelData = null;
        serializedData.objectList.Clear();
		serializedData = null;
		_uniqueObjects = null;
	}


	//Fill the serialized level data.
	public bool LoadSerializedData() {
		//If there's no level selected, change selected level to the template.
		if(GameManager.Instance.currentLevel == "") {
			//Debug.Log("No level selected, loading template level.");
			GameManager.Instance.currentLevel = "Tutorial2.sld";

			string filePath = Path.Combine(Application.streamingAssetsPath, "Tutorial2.sld");
			if(filePath.Contains("://")) {
				StartCoroutine(LoadAndroidFile(filePath));
				return true;
			}
		}
		else
			Debug.Log("Loading selected level.");

		//Load the data inside the specified .sld file.
		if(!FileManager.LoadLevel(GameManager.Instance.currentLevel, out serializedData)) {
			Debug.Log("Error loading level.");
			serializedData = new SerializedLevelData();
			return false;
		}
		DeserializeLevelData();

        return true;
	}


	public IEnumerator LoadAndroidFile(string filePath) {
		WWW www = new WWW(filePath);
		yield return www;
		MemoryStream ms = new MemoryStream(www.bytes);
		BinaryFormatter bForm = new BinaryFormatter();
		serializedData = (SerializedLevelData)bForm.Deserialize(ms);
		ms.Close();
		DeserializeLevelData();
	}

	//Transform the serialized level data list into playable gameobjects ( fills the level data list ).
	public void DeserializeLevelData() {
        //Create a new GameObject to contain the level objects.
        monsterList = new List<GameObject>();
        doorList = new List<GameObject>();
        destructibleList = new List<GameObject>();
        if (mapContainer == null)
            CreateMapContainer();

        GameObject tPrefab;

		theme = serializedData.theme;
		Camera.main.GetComponent<CameraController>().scrollerSpeed = serializedData.scrollerSpeed;
		SetParallax();

		//Update the color scheme of all enemies.
		for(int i = 0; i < serializedData.colorScheme[0].Length; i++) {
			levelData.colorScheme[i][0] = serializedData.colorScheme[i][0].GetColor();
			levelData.colorScheme[i][1] = serializedData.colorScheme[i][1].GetColor();
		}
        //Fill the object list.
		for(int i = 0; i < serializedData.objectList.Count; i++) {
			for(int j = 0; j < serializedData.objectList[i].Count; j++) {
				for(int k = 0; k < serializedData.objectList[i][j].Count; k++) {
					//Return the matching prefab ( using current serialized object's type and id ).
					tileManager.GetTile(serializedData.objectList[i][j][k].type, serializedData.objectList[i][j][k].id, out tPrefab, serializedData.objectList[i][j][k].GetPosition());
					Vector3 tRot = tPrefab.transform.localEulerAngles;
					tRot.z = serializedData.objectList[i][j][k].rotation;
					tPrefab.transform.localEulerAngles = tRot;
					
					Vector3 tScale = tPrefab.transform.localScale;
					tScale.x = serializedData.objectList[i][j][k].horizontalMirror ? -1 : 1;
					tPrefab.transform.localScale = tScale;

					//Update the unique object list.
					_uniqueObjects.CheckUniqueObject(new int[2] { i, j }, tPrefab, UniqueObjects.Mode.Add, newCheckPointSet);

					//If the serialized tile type is extended ( has special attributes ), deserialize the extra attributes.
					if(serializedData.objectList[i][j][k].isExtended) {
						if(serializedData.objectList[i][j][k].GetType() == typeof(Tile_PassableGlitch)) {
							GameObject tObj2 = tPrefab;
							Tile_PassableGlitch tTile = serializedData.objectList[i][j][k] as Tile_PassableGlitch;
							tTile.Deserialize(ref tObj2, tileManager);
						}

						//Create a copy of the previously created gameobject ( needed to pass the object as a reference ).
						GameObject tObj = tPrefab;
						//Deserialize the object's extra attributes and modify the temporary gameobject.
						serializedData.objectList[i][j][k].Deserialize(ref tObj);
						//Replace the actual gameobject with the temporary one.
						tPrefab = tObj;
                    }

                    if (tPrefab.gameObject.name.Contains("Door"))
                        doorList.Add(tPrefab.gameObject);

                    if (tPrefab.tag != "Untagged") {
                        if (tPrefab.tag == "Enemies")
                            monsterList.Add(tPrefab.gameObject);
                        else if (tPrefab.tag == "Connectable")
                            _tileConnector.SetSprite(ref tPrefab);
                        else if (tPrefab.tag == "Destructible")
                            destructibleList.Add(tPrefab.gameObject);
                    }

					//Instantiate the prefab with the serialized object's position and links it to the map container.
					levelData.objectList[i][j].Add(tPrefab);
                    levelData.objectList[i][j][k].transform.parent = mapContainer.transform;
                    _tileConnector.RefreshZone(tPrefab.transform.position);
				}
			}
        }
    }


	//Fill the serialize level data list with the level data list's information.
	public void SerializeLevel() {
		serializedData.theme = theme;
		serializedData.scrollerSpeed = Camera.main.GetComponent<CameraController>().scrollerSpeed;

		for(int i = 0; i < serializedData.objectList.Count; i++) {
			for(int j = 0; j < serializedData.objectList[i].Count; j++) {
				for(int k = 0; k < serializedData.objectList[i][j].Count; k++) {
					if(serializedData.objectList[i][j][k].isExtended) {
						serializedData.objectList[i][j][k].Serialize(levelData.objectList[i][j][k]);
					}
				}
			}
		}
	}

	public void SaveLevelToDb (string p_levelName, int p_uid, SerializedLevelData p_serializedData, System.Action<LevelInfo> levelInfo) {
		StartCoroutine (_SaveLevelToDb(p_levelName, p_uid, p_serializedData, _levelInfo => {
			levelInfo(_levelInfo);
		}));
	}

	private IEnumerator _SaveLevelToDb (string p_levelName, int p_uid, SerializedLevelData p_serializedData, System.Action<LevelInfo> levelInfo) {
		SerializeLevel ();

		WWWForm form = new WWWForm();
		form.AddField("levelName", p_levelName);
		form.AddField("data", SerializeDataAsString(p_serializedData));
		form.AddField("uid", p_uid);

		WWW www = new WWW(Url.WEBSITE+Url.SAVE_LEVEL, form);

		yield return www;

		if (www.error == null) {
			try {
				levelInfo(JsonUtility.FromJson<LevelInfo>(www.text));
			} catch (System.Exception exception) {
				Debug.LogError("Failed to fetch login info from saved level: " + exception);
				levelInfo(null);
			}
		} else {
			Debug.LogError("Failed to save level to database: " + www.error);
			levelInfo(null);
		}
	}

	public void LoadLevelDataFromDb (string uniqueId, System.Action<SerializedLevelData> levelData) {
		Debug.Log ("LoadLevelDataFromDb");
		StartCoroutine (_LoadLevelDataFromDb(uniqueId, _levelData => {
			levelData(_levelData);
		}));
	}

	private IEnumerator _LoadLevelDataFromDb (string uniqueId, System.Action<SerializedLevelData> levelData) {
		WWWForm form = new WWWForm();
		form.AddField("uniqueId", uniqueId);

		WWW www = new WWW(Url.WEBSITE+Url.LOAD_LEVEL, form);

		yield return www;
		if (www.error == null) {
			levelData((SerializedLevelData)DeserializeDataFromString(www.text));
		} else {
			Debug.LogError(www.error);
			levelData(null);
		}
	}

	/// <summary>
	/// Fetch the level information with search term.
	/// </summary>
	/// <param name="sortOption">Sort option.</param>
	/// <param name="searchTerm">Search term.</param>
	/// <param name="limit">Number of items per page.</param>
	/// <param name="offset">Page number.</param>
	/// <param name="levelInfos">Returned level info.</param>
	public void FetchLevels (SortOption sortOption, string searchTerm, System.Action<LevelInfo[]> levelInfos) {
		StartCoroutine(_FetchLevels(sortOption, searchTerm, 10, 0, _levelInfos => {
			levelInfos(_levelInfos);
		}));
	}

	/// <summary>
	/// Fetch the level information.
	/// </summary>
	/// <param name="sortOption">Sort option.</param>
	/// <param name="limit">Number of items per page.</param>
	/// <param name="offset">Page number.</param>
	/// <param name="levelInfos">Returned level info.</param>
	public void FetchLevels (SortOption sortOption, System.Action<LevelInfo[]> levelInfos) {
		StartCoroutine(_FetchLevels(sortOption, "", 10, 0, _levelInfos => {
			levelInfos(_levelInfos);
		}));
	}

	private IEnumerator _FetchLevels (SortOption sortOption, string searchTerm, int limit, int offset, System.Action<LevelInfo[]> levelInfos) {
		string sort = sortOption.ToString();
		WWWForm form = new WWWForm();
		form.AddField("sortOption", sort);
		form.AddField ("limit", limit);
		form.AddField ("offset", offset);

		if (!string.IsNullOrEmpty(searchTerm))
			form.AddField("searchTerm", searchTerm);

		WWW www = new WWW(Url.WEBSITE+Url.FETCH_LEVELS, form);

		yield return www;

		if (www.error == null) {
			LevelInfoCollection levelInfoCollection = new LevelInfoCollection();
			try {
				levelInfoCollection = JsonUtility.FromJson<LevelInfoCollection>(www.text);
				levelInfos(levelInfoCollection.levelInfos);
			} catch (System.Exception exception) {
				Debug.LogError(exception);
				levelInfos(null);
			}
		} else {
			Debug.LogError(www.error);
			levelInfos(null);
		}
	}

	private string SerializeDataAsString (object o) {
		using (MemoryStream stream = new MemoryStream()) {
			new BinaryFormatter().Serialize(stream, o);
			return System.Convert.ToBase64String(stream.ToArray());
		}
	}

	private object DeserializeDataFromString (string s) {
		byte[] data = System.Convert.FromBase64String(s);
		using (MemoryStream stream = new MemoryStream(data)) {

			BinaryFormatter bf = new BinaryFormatter();
			stream.Seek(0, SeekOrigin.Begin);
			return bf.Deserialize(stream);
		}
	}

	public void SetParallax() {
		for(int i = 0; i < 7; i++) {
			if(i == theme)
				Camera.main.transform.GetChild(i + 1).gameObject.SetActive(true);
			else
				Camera.main.transform.GetChild(i + 1).gameObject.SetActive(false);
		}
	}


	/// <summary>
	/// Add an object of a given type and id and repositions it.
	/// </summary>
	/// <param name="tPos">Position of the object in world units (x,y).</param>
	/// <param name="tColRow">Sector in which the object is positionned.</param>
	/// <param name="type">Object's type.</param>
	/// <param name="id">Object's id in the given type list.</param>
	public void AddObject(float[] tPos, int[] tColRow, ushort type, ushort id) {
		GameObject tObj;
		//Return the object's tile script ( which contains serialized information ) and the selected gameobject prefab.
		Tile tTile = tileManager.GetTile(type, id, out tObj, new Vector3(tPos[0], tPos[1], 0.0f));
        
        if (tObj.tag == "Enemies")
            monsterList.Add(tObj.gameObject);
        if (tObj.gameObject.name.Contains("Door"))
            doorList.Add(tObj.gameObject);
        if (tObj.gameObject.name.Contains("Destructible"))
            destructibleList.Add(tObj.gameObject);

        //Verifie if the object is of unique type, and if so, if an object of the same type has already been placed in the level.
        if (_uniqueObjects.CheckUniqueObject(tColRow, tObj, UniqueObjects.Mode.Add, newCheckPointSet)) {
			if(tObj.tag == "Connectable")
				_tileConnector.SetSprite(ref tObj);

			//Update the tile script with the object's type, id and position ( used when loading the level ).
			tTile.type = type;
			tTile.id = id;
			tTile.SetPosition(new Vector2(tPos[0], tPos[1]));
			//Add the tile to the serialized level data list.
			serializedData.objectList[tColRow[0]][tColRow[1]].Add(tTile);
			//Add the prefab to the level and link it to the map container gameobject.
			tObj.transform.parent = mapContainer.transform;
			//Add the gameobject to the level data list.
			levelData.objectList[tColRow[0]][tColRow[1]].Add(tObj);

			_tileConnector.RefreshZone(tObj.transform.position);
		}
		else
			Destroy(tObj);
    }


	/// <summary>
	/// Delete a given object.
	/// </summary>
	/// <param name="tColRow">Sector in which the object is positionned.</param>
	/// <param name="index">Position of the object inside the sector's list of objects.</param>
	public void DeleteObject(int[] tColRow, int index) {
		if(levelData.objectList[tColRow[0]][tColRow[1]][index] != null) {
			Vector2 tPos = levelData.objectList[tColRow[0]][tColRow[1]][index].transform.position;

			//Update the unique objects list.
			_uniqueObjects.CheckUniqueObject(tColRow, levelData.objectList[tColRow[0]][tColRow[1]][index].gameObject, UniqueObjects.Mode.Delete, newCheckPointSet);
            //remove form the monster List
            if (levelData.objectList[tColRow[0]][tColRow[1]][index].transform.tag == "Enemies")
                for (int i = 0; i < monsterList.Count; i++)
                {
                    if (monsterList[i].gameObject.transform.position == levelData.objectList[tColRow[0]][tColRow[1]][index].gameObject.transform.position)
                        monsterList.RemoveAt(i);
                }
            else if (levelData.objectList[tColRow[0]][tColRow[1]][index].transform.tag == "Door")
                for (int i = 0; i < doorList.Count; i++)
                {
                    if (doorList[i].gameObject.transform.position == levelData.objectList[tColRow[0]][tColRow[1]][index].gameObject.transform.position)
                        doorList.RemoveAt(i);
                }
            else if (levelData.objectList[tColRow[0]][tColRow[1]][index].transform.tag == "Destructible")
                for (int i = 0; i < destructibleList.Count; i++)
                {
                    if (destructibleList[i].gameObject.transform.position == levelData.objectList[tColRow[0]][tColRow[1]][index].gameObject.transform.position)
                        destructibleList.RemoveAt(i);
                }
            //Destroy the gameobject.
            Destroy(levelData.objectList[tColRow[0]][tColRow[1]][index]);
			//Remove the object from the level data list.
			levelData.objectList[tColRow[0]][tColRow[1]].RemoveAt(index);
			//Remove the object from the serialized level data list.
			serializedData.objectList[tColRow[0]][tColRow[1]].RemoveAt(index);

			_tileConnector.RefreshZone(tPos);
        }
	}


	public void AddPassable(float[] tPos, int[] tColRow, ushort fakeType, ushort fakeId) {
		GameObject tObj;
		tileManager.GetTile(3, 7, out tObj, new Vector3(tPos[0], tPos[1], 0.0f));
		Tile_PassableGlitch tTile = new Tile_PassableGlitch();
		
		tTile.type = 3;
		tTile.id = 7;
		tTile.SetPosition(new Vector2(tPos[0], tPos[1]));
		tTile.fakeType = fakeType;
		tTile.fakeId = fakeId;
		serializedData.objectList[tColRow[0]][tColRow[1]].Add(tTile);
		tTile.Deserialize(ref tObj, tileManager);
		tObj.transform.parent = mapContainer.transform;
		levelData.objectList[tColRow[0]][tColRow[1]].Add(tObj);

		if(tObj.tag == "Connectable")
			_tileConnector.SetSprite(ref tObj);

		_tileConnector.RefreshZone(tObj.transform.position);
	}


	public void AddTeleport(GameObject prefab, float[] pos1, float[] pos2) {
		int[] tColRow = new int[2];
		tColRow[0] = (int)(pos1[0] / 10);
		tColRow[1] = (int)(pos1[1] / 10);

		GameObject tObj = prefab;
		Tile_TeleportGlitch tTile = new Tile_TeleportGlitch();
		tObj = Instantiate(tObj, new Vector3(pos1[0], pos1[1], 0.0f), Quaternion.identity);

		tTile.type = 3;
		tTile.id = 9;
		tTile.SetPosition(new Vector2(pos1[0], pos1[1]));
		tTile.isEntrance = true;
		tTile.destinationX = pos2[0];
		tTile.destinationY = pos2[1];
		serializedData.objectList[tColRow[0]][tColRow[1]].Add(tTile);
		tTile.Deserialize(ref tObj);
		tObj.transform.parent = mapContainer.transform;
		levelData.objectList[tColRow[0]][tColRow[1]].Add(tObj);

		tColRow[0] = (int)(pos2[0] / 10);
		tColRow[1] = (int)(pos2[1] / 10);

		tObj = prefab;
		Tile_TeleportGlitch tTile2 = new Tile_TeleportGlitch();
		tObj = Instantiate(tObj, new Vector3(pos2[0], pos2[1], 0.0f), Quaternion.identity);

		tTile2.type = 3;
		tTile2.id = 9;
		tTile2.SetPosition(new Vector2(pos2[0], pos2[1]));
		tTile2.isEntrance = false;
		tTile2.destinationX = pos1[0];
		tTile2.destinationY = pos1[1];
		serializedData.objectList[tColRow[0]][tColRow[1]].Add(tTile2);
		tTile2.Deserialize(ref tObj);
		tObj.transform.parent = mapContainer.transform;
		levelData.objectList[tColRow[0]][tColRow[1]].Add(tObj);
	}

    public void SetEnemiesDynamique(RigidbodyType2D bodyType)
    {
        for (int i = 0; i < monsterList.Count; i++)
        {
            monsterList[i].transform.GetComponent<Rigidbody2D>().bodyType = bodyType;
        }
    }

    public void CheckPoint()
    {
        ToggleCheckPoint(true);
        checkPointFlag.transform.position = spawnPoint = player.transform.position;
    }

	public bool NewCheckPointSet {
		get {
			return newCheckPointSet;
		}
	}

    public void ToggleCheckPoint(bool b)
    {
        newCheckPointSet = b;
    }

	private void Update() {
		if(isReloading) {
			if(_reloadFrame < 4)
				_reloadFrame += 1;
			else {
				isReloading = false;
				_reloadFrame = 0;
			}
		}
	}
    public IEnumerator LoadingScreen()
    {
        if (loadingScreen == null)
            loadingScreen = Instantiate(Resources.Load("LoadingScreen", typeof(GameObject)) as GameObject);
        loadingScreen.SetActive(true);
        if (!finishLoading)
            yield return new WaitForSeconds(.5f);
        finishLoading = false;
        loadingScreen.SetActive(false);
        float time = Time.time;
        while (Time.time - time < 0.5f)
            yield return new WaitForSeconds(.1f);
        player.GetComponent<PlayerController>().SetIsDead = false;
    }
    public void FinishLevelScreen() {
        if (finishScreen == null)
            finishScreen = Instantiate(Resources.Load("finishScreen", typeof(GameObject)) as GameObject);
		else
			finishScreen.SetActive(true);
    }
}