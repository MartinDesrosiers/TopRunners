using UnityEngine;

public class CheckForNewLevel : MonoBehaviour {

	public GameObject saveMenu;
	public GameObject newLevelSaveMenu;

	public void OpenSaveMenu() {
		if(GameManager.Instance.currentLevel == "Template.sld")
			newLevelSaveMenu.SetActive(true);
		else
			saveMenu.SetActive(true);
	}
}