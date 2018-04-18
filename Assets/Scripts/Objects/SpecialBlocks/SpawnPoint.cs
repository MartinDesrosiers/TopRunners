using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	public bool isFacingRight = true;

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

		if(LevelManager.Instance.isReloading)
			Camera.main.GetComponent<CameraController>().SetInitialPosition();
    }
}