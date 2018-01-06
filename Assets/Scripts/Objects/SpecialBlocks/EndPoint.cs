using UnityEngine;

public class EndPoint : MonoBehaviour {

	private bool _isOn = true;

	public void OnTriggerEnter2D(Collider2D col) {
		if(col.tag == "Player" && _isOn) {
			_isOn = false;
            LevelManager.Instance.ToggleCheckPoint(false);
            LevelManager.Instance.FinishLevelScreen();
            col.transform.parent.GetComponentInParent<PlayerController>().Restart();
		}
	}
}