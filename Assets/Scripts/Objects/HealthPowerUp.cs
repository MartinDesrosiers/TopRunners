using UnityEngine;

public class HealthPowerUp : MonoBehaviour {

	private bool _isOn = true;

	private void OnTriggerEnter2D(Collider2D col) {

		if(_isOn) {
			if(col.tag == "Player") {
				_isOn = false;
				col.transform.parent.gameObject.GetComponentInParent<NewPlayerController>().AddHealth();
			}
		}
	}
}