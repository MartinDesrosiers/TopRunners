using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishScreenCloseButton : MonoBehaviour {

	public void CloseWindow()
    {
        LevelManager.Instance.IsPaused = false; //FIX
    }
}
