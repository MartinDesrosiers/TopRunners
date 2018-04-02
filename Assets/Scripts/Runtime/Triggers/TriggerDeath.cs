using UnityEngine;

public class TriggerDeath : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.name == "NormalColliders" || col.gameObject.name == "RollCollider")
			col.transform.GetComponentInParent<NewPlayerController>().Kill();
	}
}