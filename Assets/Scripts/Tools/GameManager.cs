using UnityEngine.SceneManagement;
//using System.Xml;
//using System.IO;
using System.Collections.Generic;
//using System;
//using UnityEngine;

/// <summary>
/// A singleton used to keep in memory certain values needed throughout the scenes.
/// currentState ( changes depending on which scene is active )
///		Used by certain script ( mostly leveleditor and runtime related ) to change behaviours according to the active scene.
///	currentLevel : The manager keeps in memory the last level used.
/// The Manager also initializes the level manager if the scene runtime or leveleditor is loaded.
/// </summary>
public class GameManager : Singleton<GameManager>
{

    //Needed to prevent non singleton constructor calls.
    protected GameManager() { }

    //Needed for scripts which are used in more than one scene but needs to behave differently according to which one is active.
    public enum GameState { RunTime, LevelEditor, Menu, Map };
    public GameState currentState;
    //File name of the selected level.
    public string currentLevel = "";
	// Unique id of the selected remote level.
	public string currentLevelUniqueId = "";
    public List<int> myList = new List<int>();

	public int myUid = 0;

    //Updates the game state and initializes the LevelManager.
    private void Awake() {
		CheckGameState();
		InitLevelManager();
    }

    void OnEnable() {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable() {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode) {
        CheckGameState();
        InitLevelManager();
    }

	private void InitLevelManager() {
		if(currentState == GameState.RunTime)
		    LevelManager.Instance.IsPaused = false;
		else if(currentState == GameState.LevelEditor)
		    LevelManager.Instance.IsPaused = true;
	}

    private void CheckGameState() {
        if (SceneManager.GetActiveScene().name == "RunTime")
            currentState = GameState.RunTime;
        else if (SceneManager.GetActiveScene().name == "LevelEditor") {
			currentState = GameState.LevelEditor;
			SpawnPoint.isFirstLoad = true;
		}
        else if (SceneManager.GetActiveScene().name == "Online" || SceneManager.GetActiveScene().name == "RunMap")
            currentState = GameState.Map;
        else
            currentState = GameState.Menu;
    }

    private void OnApplicationQuit() {
        LevelManager.Instance.ClearLevel();
        Destroy(LevelManager.Instance.gameObject);
        Destroy(this.gameObject);
    }

    /*void Save()
    {// Create the XmlDocument.
        XmlDocument doc = new XmlDocument();
        doc.LoadXml("<Stats><health>3</health></Stats>");

        // Add element.
        XmlElement newElem = doc.CreateElement("combo");
        newElem.InnerText = "20";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("strength");
        newElem.InnerText = "4";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("range");
        newElem.InnerText = "5";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("topSpeed");
        newElem.InnerText = "10";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("acceleration");
        newElem.InnerText = "6";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("stamina");
        newElem.InnerText = "10";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("recuperation");
        newElem.InnerText = "4";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("propulsion");
        newElem.InnerText = "8";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("dashSpeed");
        newElem.InnerText = "15";
        doc.DocumentElement.AppendChild(newElem);

        newElem = doc.CreateElement("dodge");
        newElem.InnerText = "4";
        doc.DocumentElement.AppendChild(newElem);

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Indent = true;
        // Save the document to a file and auto-indent the output.
        XmlWriter writer = XmlWriter.Create("data.xml", settings);
        doc.Save(writer);
    }
    public void LoadStats()
    {
        XmlDocument doc = new XmlDocument();
        doc.Load("data.xml");
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/health").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/combo").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/strength").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/range").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/topSpeed").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/acceleration").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/stamina").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/recuperation").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/propulsion").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/dashSpeed").InnerText));
        myList.Add(Convert.ToInt32(doc.SelectSingleNode("Stats/dodge").InnerText));
    }*/
}