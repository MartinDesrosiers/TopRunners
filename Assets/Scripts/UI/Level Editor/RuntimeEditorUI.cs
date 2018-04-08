using UnityEngine;
using System.Collections;

//This script is a Level Editor only script.
public class RuntimeEditorUI : MonoBehaviour {

	public GameObject levelEditorUI;
    public GameObject[] mobileButtons;

    private void Start() {
        //Remove the mobile buttons for Standalone PC/MAC/Linux versions
#if UNITY_EDITOR
        return;
#elif UNITY_STANDALONE
        mobileButtons[0].SetActive(false);
        mobileButtons[1].SetActive(false);
        mobileButtons[2].SetActive(false);
        mobileButtons[3].SetActive(false);
#endif
    }

    //Called when using the create button while in play mode.
    public void CreateButton() {
        LevelManager.Instance.player.GetComponent<PlayerController>().Restart();
		LevelManager.Instance.player.transform.SetParent(null);
        //Changes gamemode to level editor.
        LevelManager.Instance.ToggleCheckPoint(false);
		GameManager.Instance.currentState = GameManager.GameState.LevelEditor;
		//Reload the level to replace all the npcs and dynamic objects.
		LevelManager.Instance.ReloadLevel();
		//Makes sure the game is set to pause to prevent AIs and dynamic objects from moving when editing the level.
		LevelManager.Instance.IsPaused = true;

		LevelManager.Instance.player.transform.position = LevelManager.Instance.spawnPoint;
        LevelManager.Instance.player.transform.GetComponent<Rigidbody2D>().bodyType = RigidbodyType2D.Kinematic;
		//Centers camera on player position.
		Vector3 cameraPosition = LevelManager.Instance.player.transform.position;
		cameraPosition.z = Camera.main.transform.position.z;
		Camera.main.transform.position = cameraPosition;

		//Turns On the Level Editor UI.
		levelEditorUI.SetActive(true);

		//Turns off the Runtime UI and Runtime script.
		gameObject.SetActive(false);
    }
}