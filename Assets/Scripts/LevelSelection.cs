﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;

/// <summary>
/// This Script Loads all files in the "Application.persistentDataPath/Levels" directory and adds a button to the
///		level list scrollfield for each .sld file inside the folder.
///	Each button is renamed to it's according file name ( the level name ).
///	Each button gets an onClick event listener.
///	Each event does 2 things.
///		- Change the current level in GameManager to button's level name.
///		- Load the level editor scene.
/// </summary>
public class LevelSelection : MonoBehaviour {
	public GameObject localContent;		    //Level list scrollField.
	public GameObject publishedContent;		   	
	public GameObject modelButton;                  //Prefab of the load level button.
	public LevelSelectionUI levelSelectionUI;

	private int i = 0;

	private void Start() {
		RefreshLevelList();
	}

	public void RefreshLevelList() {
		i = 0;
		if(localContent.transform.childCount > 0) {
			for(int j = 0; j < localContent.transform.childCount; j++)
				Destroy(localContent.transform.GetChild(j).gameObject);
		}

		RefreshLevelList(true);
		RefreshLevelList(false);
	}

	private void RefreshLevelList(bool basicLevels) {
		string directory;
		Color buttonColor = Color.white;

		if(basicLevels) {
			directory = Application.streamingAssetsPath;
			buttonColor = new Color(255f, 0f, 195f);
		}
		else
			directory = Application.persistentDataPath + "/Levels";

		//Check if the "Levels" folder exist, if not create it to avoid errors.
		if(!Directory.Exists(directory))
			Directory.CreateDirectory(directory);

		//Directory containning all the levels.
		DirectoryInfo directoryInfo = new DirectoryInfo(directory);
		//Create an array of fileInfo containning all the files inside "/Levels".
		FileInfo[] fileInfo = directoryInfo.GetFiles();
		
		foreach(FileInfo k in fileInfo) {
			//If the file is a level.
			if(k.Extension == ".sld") {
				string levelName = k.Name.Replace(".sld", "");
				if(levelName == "Template")
					continue;

				//Create a new load level button.
				GameObject tObj = Instantiate(modelButton, Vector3.zero, Quaternion.identity, localContent.transform);
				//Replace the button's text with the name of the level.
				tObj.transform.GetChild(0).GetComponent<Text>().text = levelName;
				tObj.GetComponent<Image>().color = buttonColor;

				//Get the date of the level's creation.
				string tString = File.GetCreationTime(directory + "/" + k.Name).ToString();
				//Get the position of the space between the date and time of creation.
				int tIndex = tString.IndexOf(" ");
				//Replace the button's date of creation text with the level's date of creation ( excluding the time of creation ).
				tObj.transform.GetChild(1).GetComponent<Text>().text = tString.Substring(0, tIndex);

				//Add an onClick event to load the specified level.
				tObj.GetComponent<Button>().onClick.AddListener(() => { GameManager.Instance.currentLevel = k.Name; levelSelectionUI.SelectLevel(); });

				//Attach the button to the container.
				//tObj.transform.SetParent(container.transform, false);
				//Set the button's position according to the number of buttons before it.
				tObj.GetComponent<RectTransform>().localPosition = new Vector2(92, -i * 40 - 8);

				i++;    //Increments the number of files.

				//Resize the container according to the amount of levels.
				localContent.GetComponent<RectTransform>().sizeDelta = new Vector2(localContent.GetComponent<RectTransform>().sizeDelta.x, i * 40 + 16);
			}
		}

		PopulateRemoteLevels();
	}

	private void PopulateRemoteLevels () {
		LevelManager.Instance.FetchLevels (LevelManager.SortOption.Uid, levelInfos => {
			for (int i = 0; i < levelInfos.Length; i++) {
				//Create a new load level button.
				GameObject tObj = Instantiate(modelButton, Vector3.zero, Quaternion.identity, publishedContent.transform);
				//Replace the button's text with the name of the level.
				tObj.transform.GetChild(0).GetComponent<Text>().text = levelInfos[i].levelName;

				//Get the date of the level's creation.
				string publishedDate = Utilities.UnixTimeStampToDateTime(levelInfos[i].publishDate).ToString(Utilities.DATE_FORMAT);
				tObj.transform.GetChild(1).GetComponent<Text>().text = publishedDate;

				string uniqueId = levelInfos[i].uniqueId;
				tObj.GetComponent<Button>().onClick.AddListener(() => {
					levelSelectionUI.SelectLevel();
					GameManager.Instance.currentLevelUniqueId = uniqueId;
				});

				tObj.GetComponent<RectTransform>().localPosition = new Vector2(92, -i * 40 - 8);

				//Resize the container according to the amount of levels.
				publishedContent.GetComponent<RectTransform>().sizeDelta = new Vector2(publishedContent.GetComponent<RectTransform>().sizeDelta.x, i * 40 + 16);
			}
		});
	}
}