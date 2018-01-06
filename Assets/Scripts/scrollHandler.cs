using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class scrollHandler : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void TranslateLeft() {
        if (this.GetComponent<ScrollRect>().horizontalNormalizedPosition > 0) {
            this.GetComponent<ScrollRect>().horizontalNormalizedPosition -= 0.1f;
        }
    }

    public void TranslateRight() {
        if (this.GetComponent<ScrollRect>().horizontalNormalizedPosition < 1) {
            this.GetComponent<ScrollRect>().horizontalNormalizedPosition += 0.1f;
        }
    }
}
