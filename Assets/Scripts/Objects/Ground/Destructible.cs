using UnityEngine;
using System.Collections.Generic;

public class Destructible : MonoBehaviour {
	float destructionDelay = 1f;
    protected List<Collider2D> others;

    protected void SetDestroyTimer(GameObject obj) {
        obj.gameObject.GetComponent<Animator>().SetBool("Destroy", true);
		Destroy(obj.gameObject, destructionDelay);
	}
}