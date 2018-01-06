using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class SaveInputVerification : MonoBehaviour {

	public InputField levelName;
	public Button confirmButton;
	public Text warningMessage;

	//List of all the .sld files in the Application.persistentDataPath/Levels directory.
	private List<string> _levelList = new List<string>();
	private string _warning = "Level name cannot contain the symbol";


	//Naming criteria.
	public void Verifiy() {
		if(levelName.text.Length < 4) {
			WarningMessage("Level name has to be 4 characters minimum.");
			return;
		}
		else if(levelName.text.Length > 20) {
			WarningMessage("Level name has to be 20 characters maximum.");
			return;
		}
		else if(levelName.text.Contains("\\")) {
			WarningMessage(_warning + " \\.");
			return;
		}
		else if(levelName.text.Contains("/")) {
			WarningMessage(_warning + " /.");
			return;
		}
		else if(levelName.text.Contains(":")) {
			WarningMessage(_warning + " :.");
			return;
		}
		else if(levelName.text.Contains("*")) {
			WarningMessage(_warning + " *.");
			return;
		}
		else if(levelName.text.Contains("?")) {
			WarningMessage(_warning + " ?.");
			return;
		}
		else if(levelName.text.Contains("\"")) {
			WarningMessage(_warning + " \".");
			return;
		}
		else if(levelName.text.Contains("<")) {
			WarningMessage(_warning + " <.");
			return;
		}
		else if(levelName.text.Contains(">")) {
			WarningMessage(_warning + " >.");
			return;
		}
		else if(levelName.text.Contains("|")) {
			WarningMessage(_warning + " |.");
			return;
		}
		else if(levelName.text.Contains(".")) {
			WarningMessage("Level name cannot contain a period.");
			return;
		}

		for(int i = 0; i < _levelList.Count; i++) {
			if(levelName.text.ToLower() == _levelList[i].ToLower() || levelName.text.ToLower() == "template") {
				WarningMessage("Level name not available.");
				return;
			}
		}

		warningMessage.text = "";

		confirmButton.enabled = true;
	}


	private void Awake() {
		if(GameManager.Instance.currentLevel == "Template.sld") {
			//Create an array of files containning all the files in the Application.persistentDataPath/Levels directory.
			FileInfo[] fileInfo = new DirectoryInfo(Application.persistentDataPath + "/Levels").GetFiles();

			foreach(FileInfo k in fileInfo) {
				//Parse all the files to keep only the levels ( .sld extension ).
				if(k.Extension == ".sld")
					_levelList.Add(k.Name.Replace(".sld", ""));
			}
		}
	}


	private void Start() {
		confirmButton.enabled = false;
	}


	//Message shown to the player when the level name doesn't fit the criteria.
	private void WarningMessage(string message) {
		confirmButton.enabled = false;
		warningMessage.text = message;
	}
}