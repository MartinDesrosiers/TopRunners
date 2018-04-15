using UnityEngine;

public class NewPlayerDash {
	public Vector2 startPosition;
	public GameObject dashTarget;

	public NewPlayerDash(Vector2 start, GameObject target) {
		startPosition = start;
		dashTarget = target;
	}
}