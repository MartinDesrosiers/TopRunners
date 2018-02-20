using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroy : MonoBehaviour {

    //This new function is broken, it won't destroy objects when colliding
    protected void DestroyObj(GameObject obj, float time = 0.0f)
    {
        Destroy(obj, time);
    }

    //This function is still necessary to destroy Powerups and collectibles.
    private void OnTriggerEnter2D(Collider2D collision) {
        if (collision.tag == "Player")
            Destroy(gameObject);
    }
}
