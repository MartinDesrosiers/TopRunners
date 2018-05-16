using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour {

    public AudioSource[] musicSources;

    public int currentLevelMusic = 0;

    // Use this for initialization
    void Start() {
        if (SceneManager.GetActiveScene().name == "LevelEditor" && LevelManager.Instance.IsPaused) {
            musicSources[7].Play();
        }
        else {
            musicSources[0].Play();
        }
    }

    public void EditorMusic(bool isLevelEditor) {
        if (isLevelEditor) {

            // Stop All Music
            for (int i = 0; i < musicSources.Length; i++) {
                musicSources[i].Stop();
            }

            // Play Level Editor Music
            musicSources[7].Play();
        }
        else {

            // Stop Level Editor Music
            musicSources[7].Stop();

            currentLevelMusic = LevelManager.Instance.theme;

            //Play Current Level Music
            musicSources[currentLevelMusic].Play();
        }
    }
}
