using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour {
    bool keyCollect = false;
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!keyCollect && col.transform.tag == "Player")
        {
            keyCollect = true;
            col.transform.parent.parent.gameObject.GetComponent<PlayerController>().AddKey();
        }
    }
}
