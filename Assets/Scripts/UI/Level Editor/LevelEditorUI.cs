using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//This script is a Level Editor only script.
public class LevelEditorUI : MonoBehaviour {

    // If set to true in inspector, unlocks all objects from Level Editor
    public bool devMode;

    //Reference of the level editor script.
    public LevelEditor levelEditor;
    //Object that contains every object id buttons in the level editor ui.
    public GameObject idContainer;

    public Button categoryButton;
    public Image topMenuImage;
    public Color[] topMenuColors;

    public Sprite[] enemiesSprites;
    public Sprite[] collectSprites;
    public Sprite[] groundsSprites;
    public Sprite[] glitchesSprites;
    public Sprite[] objectsSprites;
    public Sprite lockedSprite;

    // This is where we order the level editor items according to the Level Editor custom order, not the Level Manager ID Order
    // It remaps every ID
    // Note: We also need to reorder the sprites on the Level Editor UI in Unity Inspector
    // See also Tile Manager, as it also handles the actual number of items and if they are unlocked
    // Objects that are cut from the game are simply put at the end of each array, and not in the lenght of the Tile Manager

    // Enemies
    // Basic Order  --> | 0: Bird | 1: Gaz | 2: Ghost | 3: Cat | 4: Cool | 5: Spike | 6: Fish | 7: Slime |
    // Reordered    --> | 0: Bird | 1: Cool | 2: Spike | 3: Gaz | 4: Cat | 5: Ghost |
    public ushort[] customEnemiesOrder;

    // Collectibles
    // Basic Order  --> | 0: Ruby | 1: Random+ | 2: Attack+ | 3: Health+ | 4: Reach+ | 5: Speed+ | 6: Sprint+ | 7: Jump+ | 8: Defense+ |
    // Reordered    --> | 0: Ruby | 1: Health+ | 2: Sprint+ | 3: Defense+ | 4: Speed+ | 5: Jump+ | 6: Random+ |
    public ushort[] customCollectOrder;

    //Grounds
    // Basic Order  --> | 0: Spawn | 1: End | 2: Tile | 3: Bridge | 4: Ramp 2x1 | 5: Ramp | 6: Cheese Block | 7: Conveyor | 8: Broken Glass | 9: Rocks | 10: Moving Platform | 11: Falling Platform | 12: Platform | 13: Water | 14: Lava |
    // Reordered    --> | 0: Spawn | 1: End | 2: Tile | 3: Bridge | 4: Ramp 2x1 | 5: Platform | 6: Moving Platform | 7: Falling Platform | 8: Conveyor | 9: Cheese Block | 10: Rocks | 11: Broken Glass | 12: Water | 13: Lava |
    public ushort[] customGroundsOrder;

    //Glitches
    // Basic Order  --> | 0: Auto-kill | 1: Damage Jump | 2: Speed Boost | 3: Invincible | 4: Jump Boost | 5: Lag | 6: Object Spawn | 7: Passable | 8: Rebound | 9: Teleport | 10: Invisible | 11: Fly |
    // Reordered    --> | 0: Passable | 1: Teleport | 2: Lag | 3: Speed Boost | 4: Jump Boost | 5: Damage Jump | 6: Object Spawn | 7: Invisible | 8: Invincible | 9: Fly |
    public ushort[] customGlitchesOrder;

    //Objects
    public ushort[] customObjectsOrder;
    // Basic Order  --> | 0: Boost Pad | 1: Pipe | 2: Arrow Guide | 3: Button | 4: Canon | 5: Door | 6: Fire Ball | 7: Floating Spikes | 8: Ground Spike | 9: Jump | 10: Key |
    // Reordered    --> | 0: Boost Pad | 1: Button | 2: Door | 3: Jump | 4: Ground Spike | 5: Floating Spikes | 6: Fire Ball | 7: Canon | 8: Pipe | 9: Arrow Guide | 10: Key |

    //Levels
    public ushort[] customLevelsOrder;
    // No custom order for now
    // Only useful if we add or remove a level

    public TileManager tileManager;
	public GameObject runtimeEditorUI;

	public Text levelName;
	public Text scrollerSpeed;

    public GameObject leftArrow;
    public GameObject rightArrow;

    public bool activeLevelCategory;
    public GameObject themeOptionsCanvas;
	public int currentObjType;
    bool cr_Running;
    public LevelEditor GetLevelEditor { get { return levelEditor; } }

	private void Start() {
        //Safety update to make sure items show up on the top menu.

        cr_Running = false;
        currentObjType = levelEditor.objType;
        UpdateIDRow();
    }

	public void SetTheme(int themeNumber) {
		LevelManager.Instance.theme = themeNumber;
		LevelManager.Instance.serializedData.theme = themeNumber;
		LevelManager.Instance.ReloadLevel();
	}

	public void SetSideScroller(int number) {
		CameraController tCam = Camera.main.GetComponent<CameraController>();
		tCam.scrollerSpeed = number * 2;
		tCam.scrollerSpeed = Mathf.Clamp(tCam.scrollerSpeed, -4, 4);
		LevelManager.Instance.serializedData.scrollerSpeed = tCam.scrollerSpeed;
		//scrollerSpeed.text = tCam.scrollerSpeed.ToString();

		if(tCam.scrollerSpeed != 0) {
			tCam.cameraType = CameraController.CameraType.Scroller;
			Camera.main.transform.GetChild(0).gameObject.SetActive(true);
		}
		else {
			tCam.cameraType = CameraController.CameraType.Follow;
			Camera.main.transform.GetChild(0).gameObject.SetActive(false);
		}
	}

	//Turns all the objects in LevelData to serializable objects ( SerializedData ) and save the level using the FileManager script.
	//Called when using the save button in the home menu.
	public void SaveLevel() {
		if(GameManager.Instance.currentLevel == "")
			GameManager.Instance.currentLevel = levelName.text + ".sld";

		LevelManager.Instance.SerializeLevel();

		//if(GameManager.Instance.currentLevel == "Template.sld")
		//	GameManager.Instance.currentLevel = ((int)DateTime.Now.Ticks).ToString() + ".sld";

		FileManager.SaveLevel(GameManager.Instance.currentLevel, LevelManager.Instance.serializedData);
	}

	//Used to restart from scratch.
	//Called when using the bomb button.
	public void DestroyLevel() {
        Debug.Log("DestroyLevel");

        levelEditor.ClearLevel();
		LevelManager.Instance.ReloadLevel();
	}

	//Changes the chosen type of object.
	public void ChangeObjType(int tType) {
        currentObjType = tType;
		DisableThemeCanvas();
		UpdateIDRow();
	}

	public void UpdateObjType() {
        levelEditor.objType = (ushort)currentObjType;
		DisableThemeCanvas();
		//UpdateIDRow(true);
	}

	//Changes the chosen object.
	public void ChangeObjId(int tId) {
        string newName = "";
        switch (currentObjType) {
            case 0:
                levelEditor.objId = customEnemiesOrder[tId];
                newName = enemiesSprites[tId].name;
                break;
            case 1:
                levelEditor.objId = customCollectOrder[tId];
                newName = collectSprites[tId].name;
                break;
            case 2:
                levelEditor.objId = customGroundsOrder[tId];
                newName = groundsSprites[tId].name;
                break;
            case 3:
                levelEditor.objId = customGlitchesOrder[tId];
                newName = glitchesSprites[tId].name;
                break;
            case 4:
                levelEditor.objId = customObjectsOrder[tId];
                newName = objectsSprites[tId].name;
                break;
            case 5:
                levelEditor.objId = customLevelsOrder[tId];
                newName = "";
                break;
            default:
                levelEditor.objId = customGroundsOrder[tId];
                newName = "";
                break;
        }

        categoryButton.transform.GetChild(1).GetComponent<Text>().text = newName;
    }

	public void SelectCursor() {
		levelEditor.SetCursor(!levelEditor.cursor);
	}

	public void SelectEraser() {
        levelEditor.eraser = !levelEditor.eraser;
    }

    public void EnableThemeCanvas() {
        activeLevelCategory = true;
        themeOptionsCanvas.SetActive(true);
        idContainer.SetActive(false);
        leftArrow.SetActive(false);
        rightArrow.SetActive(false);
        UpdateIDRow();
    }

    public void DisableThemeCanvas() {
        activeLevelCategory = false;
        themeOptionsCanvas.SetActive(false);
        idContainer.SetActive(true);
        leftArrow.SetActive(true);
        rightArrow.SetActive(true);
    }

	public void UpdateIDRow(bool updateId = false) {
        Sprite[] currentCategorySprites;
        string categoryText;
        Color categoryColor;

        int categorySelector = currentObjType;

        if (activeLevelCategory) {
            categorySelector = 5;
        }

        switch (categorySelector) {
            case 0: {
                    currentCategorySprites = enemiesSprites;
                    categoryText = "Enemies";
                    categoryColor = topMenuColors[0];
                }
                break;
            case 1: {
                    currentCategorySprites = collectSprites;
                    categoryText = "Collect";
                    categoryColor = topMenuColors[1];
                }
                break;
            case 2: {
                    currentCategorySprites = groundsSprites;
                    categoryText = "Blocks";
                    categoryColor = topMenuColors[3];
                }
                break;
            case 3: {
                    currentCategorySprites = glitchesSprites;
                    categoryText = "Glitches";
                    categoryColor = topMenuColors[2];
                }
                break;
            case 4: {
                    currentCategorySprites = objectsSprites;
                    categoryText = "Objects";
                    categoryColor = topMenuColors[4];
                }
                break;
            case 5: {
                    currentCategorySprites = groundsSprites;
                    categoryText = "Level";
                    categoryColor = topMenuColors[5];
                }
                break;
            default:  {
                    currentCategorySprites = groundsSprites;
                    categoryText = "Blocks";
                    categoryColor = topMenuColors[2];
                }
                break;
            }
            //Update the Category Button
            categoryButton.transform.GetChild(0).GetComponent<Text>().text = categoryText;
            topMenuImage.color = categoryColor;

            //Update the toggles

            bool[] availableObject = tileManager.GetUnlocksList((ushort)currentObjType);

            //Non existent 
            for (int i = 0; i < idContainer.transform.childCount; i++) {
                if(i < availableObject.Length)
                idContainer.transform.GetChild(i).gameObject.SetActive(true);
                else
                idContainer.transform.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < availableObject.Length; i++) {
            //Locked
            if (availableObject[i] == true && devMode == false) {
                idContainer.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                idContainer.transform.GetChild(i).GetComponent<Toggle>().interactable = false;
                idContainer.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = lockedSprite;
            }
            //Unlocked
            else {
                idContainer.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                idContainer.transform.GetChild(i).GetComponent<Toggle>().interactable = true;
                idContainer.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite = currentCategorySprites[i];
                }
            }
        UpdateViewportSize(availableObject.Length);
	}

    //Update the width of the content's wiewport according to number of item displayed in current row
    public void UpdateViewportSize(int itemsInViewport) {
        idContainer.GetComponent<RectTransform>().sizeDelta = new Vector2(itemsInViewport * 160, idContainer.GetComponent<RectTransform>().sizeDelta.y);
    }

    //Called when using the play button in editor mode.
    public void PlayButton() {
        if (!LevelManager.Instance.GetUniqueObject.EndPoint.isUsed)
        {
            if (!cr_Running)
                StartCoroutine(NoExit());
            return;
        }
		GameManager.Instance.currentState = GameManager.GameState.RunTime;
        LevelManager.Instance.SetEnemiesDynamique(RigidbodyType2D.Dynamic);
        LevelManager.Instance.player.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;
        /*if (LevelManager.Instance.isGhostReplayActive)
            LevelManager.Instance.ghostPlayer.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Dynamic;*/
        LevelManager.Instance.isPaused = false;
		runtimeEditorUI.SetActive(true);
		gameObject.SetActive(false);
		//if(LevelManager.Instance.player.GetComponent<NewPlayerController>().PlayerUI == null)
		//	LevelManager.Instance.player.GetComponent<NewPlayerController>().PlayerUIInit();
    }

	public void ToggleUI(GameObject tObj) {
		tObj.SetActive(tObj.activeSelf ? false : true);
        idContainer.SetActive(idContainer.activeSelf ? false : true);
        leftArrow.SetActive(leftArrow.activeSelf ? false : true);
        rightArrow.SetActive(rightArrow.activeSelf ? false : true);
        activeLevelCategory = activeLevelCategory ? false : true;
        UpdateIDRow();
    }

	//Used to load a specidifed scene using it's build index.
	//Called when using the Level Selection and Main menu buttons in the home menu.
	public void LoadScene(int sceneIndex) {
		SceneManager.LoadScene(sceneIndex);
	}

    System.Collections.IEnumerator NoExit()
    {
        float timer = 0f;
        GameObject temp = GameObject.Find("Play Button").gameObject;
        temp.transform.GetChild(1).gameObject.SetActive(true);
        cr_Running = true;
        while (timer < 1.5f * Time.timeScale)
        {
            //Delay is modified by timescale to work with Lag glitch.
            yield return new WaitForSeconds(2f * Time.timeScale * Time.deltaTime);
            timer += 2f * Time.timeScale * Time.deltaTime;
        }
        temp.transform.GetChild(1).gameObject.SetActive(false);
        cr_Running = false;
    }
}