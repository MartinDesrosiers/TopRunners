using UnityEngine;

public class Water : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D col) {

		if(col.tag == "Player") {
			col.transform.parent.gameObject.GetComponent<PlayerController>();
		}
	}
}