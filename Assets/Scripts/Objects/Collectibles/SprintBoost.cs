﻿using UnityEngine;

public class SprintBoost : MonoBehaviour {

	public void OnTriggerEnter2D(Collider2D col) {
		if(col.tag == "Player") {
			col.transform.parent.gameObject.GetComponent<PlayerController>().FillStamina();
		}
	}
}