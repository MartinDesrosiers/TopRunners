using UnityEngine;

public class EndPoint : MonoBehaviour {
    
	public void OnTriggerEnter2D(Collider2D col) {
		if(col.tag == "Player") {
            LevelManager.Instance.IsPaused = true;
			LevelManager.Instance.FinishLevelScreen();
			//col.transform.parent.GetComponentInParent<PlayerController>().Restart();
			//LevelManager.Instance.ToggleCheckPoint(false);
		}
	}
}