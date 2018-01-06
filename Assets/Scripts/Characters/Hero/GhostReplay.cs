using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostReplay : MonoBehaviour {
    public List<int> hori;
    public List<int> verti;
    public List<float> horiTime;
    public List<float> vertiTime;

    /*public void InitGhostList()
    {
        hori = new List<int>(LevelManager.Instance.player.GetComponent<PlayerController>().inputScript.horiPressed);
        verti = new List<int>(LevelManager.Instance.player.GetComponent<PlayerController>().inputScript.vertiPressed);
        horiTime = new List<float>(LevelManager.Instance.player.GetComponent<PlayerController>().inputScript.horiTimePressed);
        vertiTime = new List<float>(LevelManager.Instance.player.GetComponent<PlayerController>().inputScript.vertiTimePressed);
    }*/
}
