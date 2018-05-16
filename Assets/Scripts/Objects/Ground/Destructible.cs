using UnityEngine;
using System.Collections.Generic;

public class Destructible : MonoBehaviour {
	float destructionDelay = 1f;
    protected List<Collider2D> others;

    protected void SetDestroyTimer(GameObject obj) {
        if (!obj.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(obj.gameObject.GetComponent<Animator>().GetLayerIndex("Base Layer")).IsName("destroy-breaking") ||
            !obj.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(obj.gameObject.GetComponent<Animator>().GetLayerIndex("Base Layer")).IsName("glass-anim")) {
            obj.gameObject.GetComponent<Animator>().SetBool("Destroy", true);
        }
    }
}