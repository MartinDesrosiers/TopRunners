using UnityEngine;

public class SpawnPoint : MonoBehaviour {
	public bool isFacingRight = true;

	public static bool isFirstLoad = true;

	private void Start() {
        if (!LevelManager.Instance.GetNewCheckPointSet) {
            //LevelManager.Instance.player.transform.position = transform.position;
            /*if (LevelManager.Instance.isGhostReplayActive)
                LevelManager.Instance.ghostPlayer.transform.position = transform.position;*/
        }

		Vector3 tScale = LevelManager.Instance.player.transform.localScale;
		tScale.z *= isFacingRight ? 1 : -1;
		LevelManager.Instance.player.transform.localScale = tScale;
		LevelManager.Instance.player.transform.position = transform.position;
		
		if(isFirstLoad == true) {
			Camera.main.GetComponent<CameraController>().SetInitialPosition();
			isFirstLoad = false;
		}
	}
}