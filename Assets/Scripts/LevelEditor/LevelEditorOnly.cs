﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelEditorOnly : MonoBehaviour {
	private void Update() {
		if(SceneManager.GetActiveScene().name == "LevelEditor" && LevelManager.Instance.IsPaused || GameManager.Instance.currentLevel == "Tutoriel.sld")
			gameObject.GetComponent<SpriteRenderer>().enabled = true;
		else
			gameObject.GetComponent<SpriteRenderer>().enabled = false;
	}
}