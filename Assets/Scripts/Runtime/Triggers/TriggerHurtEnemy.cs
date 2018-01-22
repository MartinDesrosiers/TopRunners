using UnityEngine;

public class TriggerHurtEnemy : MonoBehaviour {
	public ushort damage;
	public bool overrideDash;

	//if the player is dashing he doesn't get hurt instead, it kills the enemie, reset the dash and makes the player jump after the kill. If not dashing, get hurt
	private void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.name == "NormalColliders" || col.gameObject.name == "RollCollider") {

            PlayerController controller = col.transform.parent.GetComponentInParent<PlayerController>();
			if(!controller.GetMovementState[BooleenStruct.ISDASHING] && !controller.GetHurt || transform.tag == "Trap") {
				bool b;
				if (transform.parent.GetComponentInParent<EnemyAI>())
				    transform.parent.GetComponentInParent<EnemyAI>().SetDashState();

				if(transform.position.x < col.gameObject.transform.position.x)
					b = true;
				else
					b = false;

				controller.TakeDamage(damage, overrideDash, b);
			}
			else if(controller.GetMovementState[BooleenStruct.ISDASHING]) {
                transform.parent.parent.gameObject.GetComponent<EnemyAI>().TakeDamage();
				controller.CheckPropulsion();
                controller.ResetDash(); 
            }
		}
	}
}