using UnityEngine;

public class DoorScript : MonoBehaviour {
	public bool InTheDoor { get; private set; }
	public bool SetButtonIsPushed { set { _buttonIsPushed = value; } }

	private bool _buttonIsPushed = false;
	private bool _hasBeenOpenWithKey = false;

	private void OnTriggerEnter2D(Collider2D col) {
		InTheDoor = true;
		if(col.gameObject.tag == "Player") {
			if(!transform.GetComponent<BoxCollider2D>().isTrigger && LevelManager.Instance.player.GetComponent<NewPlayerController>().GetKey() > 0) {
				OpenDoor();
				LevelManager.Instance.player.GetComponent<NewPlayerController>().RemoveKey();
				_hasBeenOpenWithKey = true;
			}
		}
	}

	private void OnTriggerExit2D(Collider2D col) {
		if(col.gameObject.name.Contains("NormalColliders") || col.gameObject.name.Contains("RollCollider")) {
			InTheDoor = false;
			CloseDoor();
		}
	}

	public void OpenDoor() {
		transform.GetComponent<BoxCollider2D>().isTrigger = true;
	}

	public void CloseDoor() {
		if(!_hasBeenOpenWithKey && !InTheDoor && !_buttonIsPushed)
			transform.GetComponent<BoxCollider2D>().isTrigger = false;
	}
}