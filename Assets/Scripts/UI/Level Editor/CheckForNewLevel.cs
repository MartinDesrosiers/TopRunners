using UnityEngine;

public class CheckForNewLevel : MonoBehaviour {

	public GameObject saveMenu;
	public GameObject newLevelSaveMenu;

	public void OpenSaveMenu() {
		if(GameManager.Instance.currentLevel == "Tutorial2.sld")
			newLevelSaveMenu.SetActive(true);
		else
			saveMenu.SetActive(true);
	}
}