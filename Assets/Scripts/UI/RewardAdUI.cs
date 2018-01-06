using UnityEngine;
//using UnityEngine.Advertisements;

public class RewardAdUI : MonoBehaviour {

	public GameObject bonusPopUp;
	private string selectedButton;

	// Use this for initialization
	void Start () {

	}

	public void onClick (){
		selectedButton = this.gameObject.name;
		Debug.Log (selectedButton+" was clicked");
		switch (selectedButton) {
		case "video":
			//Advertisement.Show ();
			break;
		case "facebook":
			Application.OpenURL ("https://www.facebook.com/");
			break;
		case "twitch":
			Application.OpenURL ("https://www.twitch.tv/");
			break;
		case "youtube":
			Application.OpenURL ("https://gaming.youtube.com/");
			break;
		}
		bonusPopUp.SetActive (false);
	}
}
