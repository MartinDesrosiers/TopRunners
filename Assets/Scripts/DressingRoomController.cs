using UnityEngine;
using UnityEngine.SceneManagement;

public class DressingRoomController : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnClick(string buttonClicked){
		switch (buttonClicked) {
		case "Store": {
				SceneManager.LoadScene ("Store");
			}
			break;
		case "Gym":	{
				SceneManager.LoadScene ("Gym");
			}
			break;
		case "Back": {
				SceneManager.LoadScene ("MainMenu");
			}
			break;
		}
	}
}
