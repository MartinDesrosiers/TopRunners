using UnityEngine;

public class NewPlayerDash {
	public Vector2 startPosition;
	public GameObject dashTarget;
	public float dashSpeed;
	public float dashTimer;

	public NewPlayerDash(Vector2 start, GameObject target, bool isSprinting) {
		startPosition = start;
		dashTarget = target;
		dashSpeed = isSprinting ? 3f : 2f;
		dashTimer = 0f;
	}
}