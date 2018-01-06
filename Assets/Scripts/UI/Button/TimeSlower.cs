using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSlower : MonoBehaviour {
    bool normalTime;
	// Use this for initialization
	void Start () {
        normalTime = true;
    }
	
	public void TimeScaler()
    {
        if (normalTime)
        {
            Time.timeScale = 0.6f;
        }
        else
        {
            Time.timeScale = 1f;
        }
        normalTime = !normalTime;
    }
}
