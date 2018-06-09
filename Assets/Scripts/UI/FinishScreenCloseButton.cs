using UnityEngine;

public class FinishScreenCloseButton : MonoBehaviour {
	public void CloseWindow() {
		LevelManager.Instance.ToggleCheckPoint(false);
		LevelManager.Instance.ReloadLevel();

		LevelManager.Instance.player.transform.SetParent(null);
		LevelManager.Instance.player.GetComponent<NewPlayerController>().Restart();
		LevelManager.Instance.player.transform.position = LevelManager.Instance.spawnPoint;

		Vector3 cameraPosition = LevelManager.Instance.player.transform.position;
		cameraPosition.z = Camera.main.transform.position.z;
		Camera.main.transform.position = cameraPosition;
		
		LevelManager.Instance.IsPaused = false;
	}
}
