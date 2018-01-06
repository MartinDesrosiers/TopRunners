using UnityEngine;

public class GlitchUI : MonoBehaviour {

	public GameObject teleportPrefab;
	[HideInInspector]
	public float[] entrancePos = new float[2];
	[HideInInspector]
	public float[] destinationPos = new float[2];
	[HideInInspector]
	public bool isActive = false;

	/*public void PassableUI() {
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).GetChild(0).gameObject.SetActive(true);
		isActive = true;
	}*/

	public void TeleportUI(float[] tPos) {
		transform.GetChild(0).gameObject.SetActive(false);
		transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
		isActive = true;
		entrancePos = tPos;
	}

	/*public void AddPassable() {

	}*/

	public void AddTeleport(float[] tPos) {
		destinationPos = tPos;

		LevelManager.Instance.AddTeleport(teleportPrefab, entrancePos, destinationPos);
		transform.GetChild(0).gameObject.SetActive(true);
		transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
		isActive = false;
	}
}