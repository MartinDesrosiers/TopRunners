using UnityEngine;

public class EndPoint : MonoBehaviour {
	private void OnTriggerEnter2D(Collider2D col) {
		LevelManager.Instance.IsPaused = false;
		col.transform.parent.GetComponentInParent<NewPlayerController>().Restart();
		LevelManager.Instance.ToggleCheckPoint(false);
		LevelManager.Instance.FinishLevelScreen();
	}
}