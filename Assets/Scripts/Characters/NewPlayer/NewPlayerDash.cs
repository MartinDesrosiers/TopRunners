using UnityEngine;

public class NewPlayerDash {
	public Vector2 startPosition;
	public GameObject enemyTarget;
	public Vector2 dashTarget;
	public float dashSpeed;
	public float dashTimer;

	public NewPlayerDash(Vector2 start, GameObject target, bool isSprinting) {
		Initialize(start, isSprinting);
		enemyTarget = target;
	}
	
	public NewPlayerDash(Vector2 start, float direction, bool isSprinting) {
		Initialize(start, isSprinting);
		dashTarget = new Vector2(start.x + (direction * dashSpeed * 2), start.y);

		RaycastHit2D hit = Physics2D.Linecast(start, dashTarget, 1 << LayerMask.NameToLayer("Ground"));
		if(hit.collider != null)
			dashTarget = hit.point;
	}

	private void Initialize(Vector2 start, bool isSprinting) {
		startPosition = start;
		dashSpeed = isSprinting ? 8f : 4f;
		dashTimer = 0f;
	}
}