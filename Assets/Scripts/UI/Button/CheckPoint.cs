using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPoint : MonoBehaviour {

	public void SetCheckPoint()
    {
        LevelManager.Instance.CheckPoint();
    }
}
