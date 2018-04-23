using UnityEngine;

public class DoorScript : MonoBehaviour {
	private bool _inTheDoor = false;
	private bool _buttonIsPushed = false;
	private bool _hasBeenOpenWithKey = false;
	public bool GetInTheDoor { get { return _inTheDoor; } }
	public bool SetButtonIsPushed { set { _buttonIsPushed = value; } }

	private void OnTriggerEnter2D(Collider2D col) {
		_inTheDoor = true;
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
			_inTheDoor = false;
			CloseDoor();
		}
	}

	public void OpenDoor() {
		transform.GetComponent<BoxCollider2D>().isTrigger = true;
	}

	public void CloseDoor() {
		if(!_hasBeenOpenWithKey && !_inTheDoor && !_buttonIsPushed)
			transform.GetComponent<BoxCollider2D>().isTrigger = false;
	}
}