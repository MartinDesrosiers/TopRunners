using UnityEngine;
using System.Collections;

public class JumpPlatform : MonoBehaviour {

	private bool _isOn = true;
	
	private void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.name == "NormalColliders" || col.gameObject.name == "RollCollider" && _isOn) {
			_isOn = false;
			col.transform.parent.GetComponentInParent<PlayerController>().CheckPropulsion(3f);

			StartCoroutine(JumpTimer());
		}
	}

	private IEnumerator JumpTimer() {
		yield return new WaitForSeconds(1f);

		_isOn = true;
	}
}