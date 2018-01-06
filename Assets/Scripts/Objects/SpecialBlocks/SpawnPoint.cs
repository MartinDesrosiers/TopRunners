using UnityEngine;

public class SpawnPoint : MonoBehaviour {

	public bool isFacingRight = true;

	private void Start() {
        if (!LevelManager.Instance.GetNewCheckPointSet)
        {
            LevelManager.Instance.player.transform.position = transform.position;
            /*if (LevelManager.Instance.isGhostReplayActive)
                LevelManager.Instance.ghostPlayer.transform.position = transform.position;*/
        }
		Vector3 tScale = LevelManager.Instance.player.transform.localScale;
		tScale.z *= isFacingRight ? 1 : -1;
		LevelManager.Instance.player.transform.localScale = tScale;

        //Set the camera at spawn position, only required at first load. Hardcoded 5.62
        if (Camera.main.transform.position.y == 5.62f) {
            Vector3 cameraPosition = LevelManager.Instance.player.transform.position;
            cameraPosition.z = Camera.main.transform.position.z;
            Camera.main.transform.position = cameraPosition;
        }
    }
}