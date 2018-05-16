using UnityEngine;

public class LevelEditorTutorial : MonoBehaviour {
	public void Awake() {
		if(!LevelEditor.firstLoad)
			gameObject.SetActive(false);
		else
			LevelEditor.firstLoad = false;
	}

	public void Exit() {
		gameObject.SetActive(false);
	}
}