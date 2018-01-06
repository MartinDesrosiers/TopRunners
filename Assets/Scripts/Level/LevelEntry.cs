using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class LevelEntry : MonoBehaviour {
	public string uniqueId;

	public Text NameTxt;
	public Text DateTxt;

	public void Init (LevelInfo levelInfo) {
		this.uniqueId = levelInfo.uniqueId;
		NameTxt.text = levelInfo.levelName;
		// Don't show the date at all if it's not set
		if (levelInfo.publishDate > 0)
			DateTxt.text = Utilities.UnixTimeStampToDateTime(levelInfo.publishDate).ToString(Utilities.DATE_FORMAT);

		GetComponent<Button>().onClick.AddListener(() => {
			GameManager.Instance.currentLevelUniqueId = levelInfo.uniqueId;
			SceneManager.LoadScene("RunTime");
		});
	}
}
