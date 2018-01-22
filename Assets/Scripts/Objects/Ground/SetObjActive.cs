using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetObjActive : MonoBehaviour {

	// Use this for initialization
	public IEnumerator SetObjectActive(float destrucTime)
    {
        yield return new WaitForSeconds(destrucTime);
        gameObject.GetComponent<Animator>().SetBool("Destroy", false);
        gameObject.transform.localScale = new Vector2(1f, 1f);
        gameObject.SetActive(false);
    }
}
