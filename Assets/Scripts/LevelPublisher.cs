using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class LevelPublisher : MonoBehaviour {

	private LevelEditorUI levelEditorUI = null;

    public void PublishLevel () {
		if (GameManager.Instance.currentLevel == "Tutorial2.sld") {
			Debug.LogError ("Cannot publish tutorial level.");
			return;
		}

		if (levelEditorUI == null) {
			// Hacky way to access hidden LevelEditorUI object from scene, without returning prefab
			LevelEditorUI[] levelEditorUIs = Resources.FindObjectsOfTypeAll<LevelEditorUI>();
			foreach (LevelEditorUI l in levelEditorUIs) {
				bool isActive = l.gameObject.activeSelf;

				l.gameObject.SetActive(true);

				if (l.gameObject.activeInHierarchy) {
					levelEditorUI = l;
				}

				l.gameObject.SetActive(isActive);
			}
		}

		if (levelEditorUI == null) {
			Debug.LogError("No LevelEditorUI in scene. Level not published");
			return;
		}


		// Make sure we're logged in before saving
		if (!AccountManager.Instance.LoggedIn) {
			Debug.Log("You need to be logged in to publish levels.");
			AccountManager.Instance.ShowLoginDialog();
			return;
		}

		LevelManager.Instance.SerializeLevel();

		LevelManager.Instance.SaveLevelToDb (
			levelEditorUI.levelName.text,
			AccountManager.Instance.Uid,
			LevelManager.Instance.serializedData,
			levelInfo => {
			if (levelInfo != null) {
				if (string.IsNullOrEmpty(levelInfo.errorMessage)) {
					Debug.Log("Successfully saved level");

					// Remove the local level
					FileManager.DeleteLevel(GameManager.Instance.currentLevel);

					// Get rid of finish screen
					Destroy(this.gameObject);
				} else {
					Debug.LogError("Failed to save level");
				}
			} else {
				Debug.LogError("Failed to save level");
			}
		});
    }
}
