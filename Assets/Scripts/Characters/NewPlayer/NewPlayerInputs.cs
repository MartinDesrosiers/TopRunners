using UnityEngine;

public class NewPlayerInputs : MonoBehaviour {
	public float Direction { get; private set; }
	public bool Jump { get; private set; }
	public bool Slide { get; private set; }
	public bool Sprint { get; private set; }

	private void Update() {
		Direction = Input.GetAxisRaw("Horizontal");
		Jump = Input.GetKeyDown(KeyCode.W) | Input.GetKeyDown(KeyCode.Space) | Input.GetKeyDown(KeyCode.UpArrow);
		Slide = Input.GetKeyDown(KeyCode.S) | Input.GetKeyDown(KeyCode.DownArrow);
		Sprint = Input.GetKey(KeyCode.LeftShift);
	}
}