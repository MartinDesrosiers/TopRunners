using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using System.Collections.Generic;
using System.IO;

public class LevelSelectionUI : MonoBehaviour {
	public LevelSelection levelSelection;
	public GameObject LocalInterface;
    public GameObject[] LocalInterfaceButtons;
    public GameObject LocalButton;

    public GameObject PublishedInterface;
    public GameObject[] PublishedInterfaceButtons;

    public GameObject LocalEntry;
    public GameObject PublishedEntry;

    public GameObject[] Panels;

	public Text levelName;

    private string currentFilter = "Local";

	// Use this for initialization
	void Start () {
        LocalButton.GetComponent<Button>().Select();
        
        //UPDATE WITH NEW SAVE LOAD LEVEL
        //SaveAndLoadData.LoadLevel ();
        LoadLevelsInterface(currentFilter);
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void LoadLevelsInterface(string listFilter) {
        currentFilter = listFilter;
        if (listFilter == "Local") {
            LocalInterface.SetActive(true);
            PublishedInterface.SetActive(false);
            foreach (GameObject buttons in LocalInterfaceButtons) {
                buttons.GetComponent<Button>().interactable = false;
            }
        }
        else if (listFilter == "Published") {
            LocalInterface.SetActive(false);
            PublishedInterface.SetActive(true);
            foreach (GameObject buttons in PublishedInterfaceButtons) {
                buttons.GetComponent<Button>().interactable = false;
            }
        }
        LoadList();
    }
    
    //Load the levels list
    //Filtered by local or published
    //ToDo Order by "Date" for Local
    //ToDo Order by "Runs" for Published
	void LoadList(){
        /*
		foreach (LevelData level in SaveAndLoadData.savedLevels) {
            if (currentFilter == "Local") {
                if (level.published == false) {
                    LoadLocalEntry(level);
                }
            }
            else if (currentFilter == "Published") {
                if (level.published == true) {
                    LoadPublishedEntry(level);
                }
            }
		}
        */
	}

    //Assign the values to the local button
    void LoadLocalEntry(LevelData level) {
		Instantiate(LocalEntry);
		//GameObject oneEntry = Instantiate(LocalEntry);

        //Level Name
        //Transform title = oneEntry.transform.GetChild(0);
        //title.GetComponent<Text>().text = level.name;

        //Level Date
        //Transform date = oneEntry.transform.GetChild(1);
        //date­.GetComponent<Text>().text = level.date+"";
    }

    //Assign the values to the published button
    void LoadPublishedEntry (LevelData level) {
		Instantiate(PublishedEntry);
		//GameObject oneEntry = Instantiate(PublishedEntry);

		//Level Name
		//Transform title = oneEntry.transform.GetChild(0);
		//title.GetComponent<Text>().text = level.name;

		//Level Likes
		//Transform likes = oneEntry.transform.GetChild(1);
		//ToDo add "Likes" in level data
		//likes.GetComponent<Text>().text = level.likes;

		//Level Runs
		//Transform runs = oneEntry.transform.GetChild(2);
		//ToDo add "Runs" in level data
		//runs.GetComponent<Text>().text = level.runs;
	}

    public void SelectLevel() {
        if (currentFilter == "Local")
            foreach (GameObject buttons in LocalInterfaceButtons)
                buttons.GetComponent<Button>().interactable = true;
        else if (currentFilter == "Published")
            foreach (GameObject buttons in PublishedInterfaceButtons)
                buttons.GetComponent<Button>().interactable = true;
    }

	public void CreateNewLevel()
    {
        StartCoroutine(LevelManager.Instance.LoadingScreen());
        GameManager.Instance.currentLevel = "";
		SceneManager.LoadScene("LevelEditor");
	}

	public void RenameLevel() {
		DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath + "/Levels");
		FileInfo[] file = directory.GetFiles(GameManager.Instance.currentLevel);
		Debug.Log(directory.FullName + " , " + directory.Root);
		 file[0].MoveTo(directory.FullName + "/" + levelName.text + ".sld");
		levelSelection.RefreshLevelList();
	}

	public void RemoveLevel() {
		DirectoryInfo directory = new DirectoryInfo(Application.persistentDataPath + "/Levels");
		FileInfo[] file = directory.GetFiles(GameManager.Instance.currentLevel);
		file[0].Delete();
		levelSelection.RefreshLevelList();
	}

	//TODO adapt to new save load function
	void loadLevel(int levelID){
		/*SettingsData.current = new SettingsData ();
		SettingsData.current.loadLevelId = levelID;
		SaveAndLoadData.SaveSettings ();
		Debug.Log("Loading level with ID :"+levelID);*/
		SceneManager.LoadScene("LevelEditor");
	}

    public void OnClick(string buttonSelected) {
        switch (buttonSelected) {
            case "Back": {
                    SceneManager.LoadScene("MainMenu");
                }
                break;
            case "Run": {
                    //SceneManager.LoadScene("Runtime");
                }
                break;
            case "Edit": {
                    SceneManager.LoadScene("LevelEditor");
                }
                break;
            case "Rules": {
                    //Open Pop-up
                }
                break;
            case "RulesSave": {
                    //Open Pop-up
                }
                break;
            case "RulesCancel": {
                    //Open Pop-up
                }
                break;
            case "Rename": {
                    //Open Pop-up
                }
                break;
            case "RenameSave": {
                    //Open Pop-up
                }
                break;
            case "RenameCancel": {
                    //Open Pop-up
                }
                break;
            case "Destroy": {
                    //Open Pop-up
                }
                break;
            case "DestroyConfirm": {
                    //Open Pop-up
                }
                break;
            case "DestroyCancel": {
                    //Open Pop-up
                }
                break;
            case "Stats": {
                    //Open Pop-up
                }
                break;
            case "StatsClose": {
                    //Open Pop-up
                }
                break;
            case "Scores": {
                    //Open Pop-up
                }
                break;
            case "ScoresClose": {
                    //Open Pop-up
                }
                break;
            case "Promote": {
                    //Open Pop-up
                }
                break;
            case "PromoteConfirm": {
                    //Open Pop-up
                }
                break;
            case "PromoteCancel": {
                    //Open Pop-up
                }
                break;
        }
    }
}
