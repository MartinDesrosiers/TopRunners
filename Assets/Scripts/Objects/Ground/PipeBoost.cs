using UnityEngine;

public class PipeBoost : MonoBehaviour {
	private bool playerOnTop = false;

	private void OnTriggerStay2D(Collider2D collision) {
		if(collision.gameObject.name == "isGroundCollider") {
			playerOnTop = true;
			return;
		}

		NewPlayerController player = collision.transform.parent.GetComponentInParent<NewPlayerController>();
		if(collision.gameObject.name == "isWalledCol") {
			if(player.CurrentState.CompareTo(PlayerStates.Slide) == 0 && !transform.GetComponent<BoxCollider2D>().isTrigger && !playerOnTop) {
				transform.GetComponent<Collider2D>().isTrigger = true;
				//ply.StartGlitchTimer("SpeedBoost", 1.5f);
			}
		}
	}

	private void OnTriggerExit2D(Collider2D collision) {
		if(collision.gameObject.name == "RollCollider") {
			transform.GetComponent<Collider2D>().isTrigger = false;
		}
		else if(collision.gameObject.name == "isGroundCollider")
			playerOnTop = false;
	}
}