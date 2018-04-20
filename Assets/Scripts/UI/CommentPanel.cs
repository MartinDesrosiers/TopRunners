using UnityEngine;
using UnityEngine.UI;

public class CommentPanel : MonoBehaviour {
	public Button popUpButton;
	public Button confirmButton;
	public InputField inputField;
	public float radius;

	private GameObject _panel;
	private bool _isPopUp = false;

	private void Awake() {
		_panel = transform.GetChild(0).gameObject;
		if(GameManager.Instance.currentState == GameManager.GameState.RunTime) {
			popUpButton.gameObject.SetActive(false);
			confirmButton.gameObject.SetActive(false);
			inputField.interactable = false;
		}
	}

	private void FixedUpdate() {
		if(!LevelManager.Instance.IsPaused) {
			if(_isPopUp) {
				if((LevelManager.Instance.player.transform.position - transform.position).magnitude < radius) {
					LevelManager.Instance.IsPaused = true;
					_isPopUp = false;
					_panel.SetActive(true);
				}
			}
			else {
				if((LevelManager.Instance.player.transform.position - transform.position).magnitude > radius)
					_isPopUp = true;
			}
		}
	}

	public void TogglePopUp() {
		_isPopUp = !_isPopUp;
		popUpButton.GetComponent<Image>().color = _isPopUp ? new Color(125f / 255f, 220f / 255f, 125f / 255f) : new Color(220f / 255f, 125f / 255f, 125f / 255f);
	}

	public void Confirm() {
		LevelManager.Instance.SerializeCommentPanel(gameObject);
		Exit();
	}

	public void Exit() {
		if(GameManager.Instance.currentState == GameManager.GameState.RunTime)
			LevelManager.Instance.IsPaused = false;

		_panel.SetActive(false);
	}
}