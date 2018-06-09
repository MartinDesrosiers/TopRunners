using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class PlayerUI {
	public GameObject[] hearts;
	public GameObject keys;
	public GameObject sprintBar;
	public Image sprintBarImage;
	public GameObject jumpButton;

    public void ShowKeys(int t_key) {
        keys.GetComponent<Text>().text = t_key.ToString();
    }

    public void CheckHealth(int t_health) {
        for (int i = 0; i < 3; i++)
            hearts[i].SetActive(i < t_health);

		//if(t_health == 0)
		//	hearts[0].SetActive(false);
	}

    public void ChangeJumpButtonSprite(int sprite) {
        //jumpButton.GetComponent<Image>().sprite = t_jumpDash;
    }

    public void FillSprintBar(float filling) {
        if (sprintBar != null)
            sprintBarImage.fillAmount = filling;
    }
}