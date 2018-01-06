﻿using UnityEngine;

public class ReboundBlock : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.name == "isGroundCollider") {
			PlayerController player = col.transform.parent.parent.gameObject.GetComponent<PlayerController>();
            Rigidbody2D playerBody = col.transform.parent.parent.gameObject.GetComponent<Rigidbody2D>();

			playerBody.velocity = new Vector2(playerBody.velocity.x, player.Jump());
        }
	}
}