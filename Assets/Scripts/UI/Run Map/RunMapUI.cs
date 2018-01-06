using UnityEngine;
using UnityEngine.SceneManagement;

public class RunMapUI : MonoBehaviour {

	public GameObject[] textsToTranslate;

	void Start(){
		updateLanguage();
	}

	void updateLanguage() {
		Localization.current = new Localization ();
		//string[] textIDs = new string[6] {"locLightWorld","locDarkWorld","locFireWorld","locScores","locGym","locStore"};
	}

	public GameObject overlay;

	public void onClick(string buttonClicked){
		switch(buttonClicked) {
		case "Back":
			{
				SceneManager.LoadScene ("MainMenu");
			}
			break;
		}
	}

	public void levelClick(string selectedLevel){
		selectedLevel = this.gameObject.name;
		activatePopUp(selectedLevel);
	}

	public void playLevel(){
		SceneManager.LoadScene("RuntimeDEMO");
	}

	void activatePopUp(string theLevel){
		overlay.SetActive(true);
	}

	void deactivatePopUp(string theLevel){
	}
}
