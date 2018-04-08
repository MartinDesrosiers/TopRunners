using UnityEngine;

public class AB_Gaz : AB_Interface {
	private EnemyAI _parentAI;
	private Animator _anim;
	private float _dashRadius = 4f;
	private bool _isAbilityActive = false;
	private float _abilityDuration = 1.5f;
	private float _abilityCooldown = 1.5f;
	private float _abilityTimer = 0f;

	public AB_Gaz(EnemyAI parentAI) {
		_parentAI = parentAI;
		_anim = _parentAI.transform.GetChild(1).GetChild(0).GetComponent<Animator>();
	}

	public void FixUpdate(GameObject player, int behaveNumber, ref DashState dashState) {
		float magnitude = (player.transform.position - _parentAI.transform.position).magnitude;

		if(_isAbilityActive) {
			_abilityTimer += Time.fixedDeltaTime;
			if(_abilityTimer >= _abilityDuration) {
				_parentAI.transform.GetChild(0).GetChild(0).GetComponent<CircleCollider2D>().enabled = false;
			}

			if(_abilityTimer >= _abilityDuration + _abilityCooldown) {
				_isAbilityActive = false;
				_abilityTimer = 0;
			}
		}

		if(magnitude < _dashRadius * 2) {
			_parentAI.overrideMove = true;

			if(!_isAbilityActive) {
				_isAbilityActive = true;
				_parentAI.transform.GetChild(0).GetChild(0).GetComponent<CircleCollider2D>().enabled = true;
				_anim.SetTrigger("Attack");
			}
		}
		else if(!_isAbilityActive)
			_parentAI.overrideMove = false;
	}
}