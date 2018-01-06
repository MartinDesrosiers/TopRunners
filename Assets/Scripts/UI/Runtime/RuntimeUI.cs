using UnityEngine;
using UnityEngine.UI;

public class RuntimeUI : MonoBehaviour {

    public Text timer;
    static bool startTimer;
    static float minutes = 0f;
    static float seconds = 0f;
    static float miliseconds = 0f;
    
	private void Start () {
		if (!string.IsNullOrEmpty (GameManager.Instance.currentLevelUniqueId)) {
			LevelManager.Instance.LoadLevelDataFromDb (GameManager.Instance.currentLevelUniqueId, levelData => {
				Debug.Log ("Load the level at id " + GameManager.Instance.currentLevelUniqueId);

				LevelManager.Instance.InitializeLevel ();
				LevelManager.Instance.serializedData = levelData;
				LevelManager.Instance.DeserializeLevelData ();
			});
		}
	}

    public static bool GetStartTimer { get { return startTimer; } set { startTimer = value; } }
	//Pause or unpause the game ( Toggle ).
    public void PauseButton() {
        if (LevelManager.Instance.isPaused = LevelManager.Instance.isPaused ? false : true) {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(true);
        }
        else {
            transform.GetChild(1).gameObject.SetActive(false);
            transform.GetChild(0).gameObject.SetActive(true);
        }
	}

    public void ResetTime() {
        minutes = 0;
        seconds = 0;
        miliseconds = 0;
        PrintTime();
    }

    public void UpdateTime() {
        if (miliseconds >= 100) {
            if (seconds >= 59) {
                minutes++;
                seconds = 0;
            }
            else if (seconds <= 59) {
                seconds++;
            }

            miliseconds = 0;
        }

        miliseconds += Time.deltaTime * 100;
        PrintTime();
    }
    void PrintTime()
    {
        if (seconds <= 9)
        {
            timer.text = minutes + ":" + "0" + seconds + ":" + (int)miliseconds;
        }
        else
        {
            timer.text = minutes + ":" + seconds + ":" + (int)miliseconds;
        }
    }


    public void UpdateSprint(float tSlider) {
		//Gets the sprint gameobject and updates the slider position.
        transform.GetChild(6).GetChild(0).gameObject.GetComponent<Image>().fillAmount = tSlider;
	}


	private void Update() {
        if (!LevelManager.Instance.isPaused && startTimer)
        {
            UpdateTime();
        }
	}
}
