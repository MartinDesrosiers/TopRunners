﻿using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreUIController : MonoBehaviour {

	public void OnClick(string buttonSelected) {
		switch(buttonSelected) {

			case "Back":
				SceneManager.LoadScene("MainMenu");
			break;
		}
	}
}