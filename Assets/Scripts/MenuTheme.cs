using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuTheme : MonoBehaviour {

    void Awake() {
       DontDestroyOnLoad(this.gameObject);
    }

    void Update() {
        if (SceneManager.GetActiveScene().name == "LevelEditor") {
            Destroy(this.gameObject);
        }
    }

}
