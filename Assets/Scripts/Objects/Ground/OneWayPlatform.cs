using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayPlatform : MonoBehaviour {

	public BoxCollider2D boxCol;

	private bool isPassing;

	private void Update() {
		if(isPassing)
			boxCol.enabled = false;
		else
			boxCol.enabled = true;
	}

	private void OnTriggerStay2D(Collider2D col) {
		if(col.tag == "Player")
			isPassing = true;
	}

	private void OnTriggerExit2D(Collider2D col) {
		if(col.tag == "Player")
			isPassing = false;
	}
}
