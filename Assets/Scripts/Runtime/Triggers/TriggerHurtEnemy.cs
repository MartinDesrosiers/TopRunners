using UnityEngine;

public class TriggerHurtEnemy : MonoBehaviour {
	public ushort damage;
	public bool overrideDash;
	
	private void OnTriggerEnter2D(Collider2D col) {
		if(col.gameObject.name == "NormalColliders" || col.gameObject.name == "RollCollider") {
            NewPlayerController controller = col.transform.parent.GetComponentInParent<NewPlayerController>();

			if(controller.CurrentState.CompareTo(PlayerStates.Dash) != 0 && !controller.IsRecovering || transform.tag == "Trap" || overrideDash) {
				bool b;
				
				if(transform.tag != "Trap")
					if (transform.parent.GetComponentInParent<EnemyAI>() != null)
						transform.parent.GetComponentInParent<EnemyAI>().SetDashState();

				if(transform.position.x < col.gameObject.transform.position.x)
					b = true;
				else
					b = false;

				controller.TakeDamage(damage, overrideDash, b);
			}
			else if(controller.CurrentState.CompareTo(PlayerStates.Dash) == 0) {
                transform.parent.parent.gameObject.GetComponent<EnemyAI>().TakeDamage();
				controller.ForceJump();
            }
		}
	}
}