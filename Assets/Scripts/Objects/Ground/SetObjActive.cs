using UnityEngine;
using System.Collections;

public class SetObjActive : MonoBehaviour {

	public IEnumerator SetObjectActive(float destrucTime) {
		yield return new WaitForSeconds(destrucTime);
		gameObject.GetComponent<Animator>().SetBool("Destroy", false);
		gameObject.transform.localScale = new Vector2(1f, 1f);
		gameObject.SetActive(false);
	}
}
