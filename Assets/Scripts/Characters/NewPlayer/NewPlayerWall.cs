using UnityEngine;

public class NewPlayerWall : MonoBehaviour {
	private NewPlayerController _controller;

	private void Start() {
		_controller = transform.parent.parent.GetComponent<NewPlayerController>();
	}

	private void OnTriggerEnter2D(Collider2D collision) {
		_controller.TriggerEnter2D(collision, "Wall");
	}

	private void OnTriggerExit2D(Collider2D collision) {
		_controller.TriggerExit2D(collision, "Wall");
	}
}