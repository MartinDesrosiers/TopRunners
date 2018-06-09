using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour {
	private bool _isOn = true;

	private void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.name == "NormalColliders" || col.gameObject.name == "RollCollider" && _isOn) {
			NewPlayerController controller = col.transform.parent.GetComponentInParent<NewPlayerController>();
			if(controller.CurrentState.CompareTo(PlayerStates.Dash) != 0) {
				_isOn = false;
				controller.ForceJump(3f);

				StartCoroutine(JumpTimer());
			}
		}
	}

	private IEnumerator JumpTimer() {
		yield return new WaitForSeconds(1f);

		_isOn = true;
	}
}