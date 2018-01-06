using UnityEngine;

public class TimedGlitch : MonoBehaviour {

	public string glitchType;
	public float timerDelay;

	public void OnTriggerEnter2D(Collider2D col) {
		if(col.tag == "Player") {
			col.transform.parent.gameObject.GetComponentInParent<PlayerController>().StartGlitchTimer(glitchType, timerDelay);
		}
	}
}