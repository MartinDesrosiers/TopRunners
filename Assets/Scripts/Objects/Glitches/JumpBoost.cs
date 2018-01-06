using UnityEngine;

public class JumpBoost : MonoBehaviour {

	public float force;
	public bool isDamage;

	private void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.name == "NormalColliders") {
			if(isDamage) {
				bool b = (transform.position.x < col.gameObject.transform.position.x);
				col.transform.parent.parent.gameObject.GetComponent<PlayerController>().TakeDamage(1, true, b);
				Destroy(gameObject);
			}
			else
				col.transform.parent.parent.gameObject.GetComponent<PlayerController>().CheckPropulsion(3f);
		}
	}
}