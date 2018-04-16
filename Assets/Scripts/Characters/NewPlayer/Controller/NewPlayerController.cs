using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewPlayerController : NewPlayerMotor {
	public bool IsRecovering { get { return Time.time - _recoveryTimer < _recoveryDelay; } }
	public bool IsInvisible { get; private set; }
	public float WalkingSpeed { get { return _stats.walkingSpeed; } }
	public float RunningSpeed { get { return _stats.runningSpeed; } }
	public float SprintingSpeed { get { return _stats.sprintingSpeed; } }

	public RuntimeEditorUI runtimeEditorUI;
	public Animator animator;
	public PlayerUI playerUI;
	public float wallJumpDelay;
	public float slideDelay;

	private NewPlayerStatistics _stats;
	private NewPlayerColliders _colliders;
	private NewPlayerDash _dash;
	private List<NewPlayerGlitch> _glitches;
	private ushort _range;
	private ushort _key;
	private float _jumpBoost;
	private float _recoveryTimer;
	private float _recoveryDelay;
	private float _currentStamina;
	private bool _isInvincible;
	private bool _isStaminaRefilling;

	private void Awake() {
		CustomStart();
	}

	protected override void CustomStart() {
		_colliders = new NewPlayerColliders(transform);
		_recoveryDelay = 0.4f;

		base.CustomStart();
	}

	public void SetToDefault() {
		_dash = null;
		_stats = new NewPlayerStatistics();
		_glitches = new List<NewPlayerGlitch>();
		wallJumpDelay = 0.2f;
		slideDelay = 0.6f;
		
		_range = 5;
		_key = 0;
		_jumpBoost = 1f;
		_isInvincible = false;
		_recoveryTimer = -_recoveryDelay;
		_currentStamina = _stats.stamina;
	}

	public void Restart() {
		SetToDefault();

		RuntimeUI.GetStartTimer = false;

		_rigidBody.velocity = Vector3.zero;
		playerUI.ShowKeys(_key);
		playerUI.CheckHealth(_stats.health);
		playerUI.FillSprintBar(_currentStamina / _stats.stamina);
		transform.position = LevelManager.Instance.spawnPoint;

		LevelManager.Instance.ReloadLevel();
		runtimeEditorUI.transform.GetComponent<RuntimeUI>().ResetTime();
		LevelManager.Instance.finishLoading = true;
	}
	
	#region Stamina
	public void FillStamina() {
		_currentStamina = _stats.stamina;
		playerUI.FillSprintBar(_currentStamina / _stats.stamina);
	}

	public bool UseStamina() {
		if(_currentStamina > 0) {
			_isStaminaRefilling = false;
			_currentStamina -= _stats.staminaUseSpeed * Time.deltaTime;
			playerUI.FillSprintBar(_currentStamina / _stats.stamina);
			return true;
		}
		else {
			_currentStamina = 0;
			return false;
		}
	}

	public IEnumerator RegenStaminaTimer() {
		yield return new WaitForSeconds(2f);
		_isStaminaRefilling = true;
	}

	public void RegenStamina() {
		if(_isStaminaRefilling) {
			if(_currentStamina + _stats.staminaUseSpeed * Time.deltaTime < _stats.stamina) {
				_currentStamina += _stats.staminaUseSpeed * Time.fixedDeltaTime;
				playerUI.FillSprintBar(_currentStamina / _stats.stamina);
			}
			else {
				_currentStamina = _stats.stamina;
				_isStaminaRefilling = false;
			}
		}
	}
	#endregion

	#region Colliders
	public void ToggleJumpingColliders(bool enable) {
		_colliders.mainEdgeCollider.points = enable ? _colliders.jumpingCollidersPoints : _colliders.normalCollidersPoints;
	}

	public void ToggleSlidingColliders(bool enable) {
		_colliders.wallEdgeCollider.points = enable ? _colliders.slidingCollider : _colliders.standingCollider;
		_colliders.rollColliders.SetActive(enable);
		_colliders.colliders.SetActive(!enable);
	}
	#endregion

	#region Keys
	public void AddKey() {
		_key++;
		playerUI.ShowKeys(_key);
	}

	public void RemoveKey() {
		_key--;
		playerUI.ShowKeys(_key);
	}

	public ushort GetKey() {
		return _key;
	}
	#endregion

	#region Jump
	public float Jump() {
		float jumpForce = _stats.jumpForce * _jumpBoost;
		_jumpBoost = 1f;
		return jumpForce;
	}

	public void ForceJump(float boost = 1f) {
		_jumpBoost = boost;
		_stateMachine.CurrentState = PlayerStates.Jump;
	}
	#endregion

	#region Health
	public void AddHealth(ushort hp = 1) {
		_stats.health += hp;
		if(_stats.health > 3)
			_stats.health = 3;
		playerUI.CheckHealth(_stats.health);
	}

	public void TakeDamage(ushort damage, bool overrideDash, bool hitOnLeftSide) {
		if(!_isInvincible && !IsRecovering && (CurrentState.CompareTo(PlayerStates.Dash) != 0 || overrideDash)) {
			if(damage >= _stats.health) {
				_stats.health = 0;
				playerUI.CheckHealth(_stats.health);
				Kill();
			}
			else {
				Move(new Vector2(0, 0));

				//if(hitOnLeftSide)
				//	afterHit = new Vector2(10, 1);
				//else
				//	afterHit = new Vector2(-10, 1);
				//StartCoroutine(RecoverFromDamage());

				_stats.health -= damage;
				playerUI.CheckHealth(_stats.health);

				_recoveryTimer = Time.time;
			}
		}
	}

	public void Kill() {
		_stateMachine.CurrentState = PlayerStates.Dead;
	}
	#endregion

	#region Dash
	public void StartDash(bool isSprinting = false) {
		_dash = new NewPlayerDash(transform.position, EnemyList.GetDash(transform.position, Direction), isSprinting);
	}

	public void LerpDash() {
		float magnitude = (transform.position - _dash.dashTarget.transform.position).magnitude;
		_dash.dashTimer += _dash.dashSpeed / magnitude * Time.deltaTime;
		transform.position = Vector2.Lerp(transform.position, _dash.dashTarget.transform.position, _dash.dashTimer);
	}

	public void ResetDash() {
		_dash = null;
		//inEnemiesRange = false;
		//ResetBool(true, BooleenStruct.ISJUMPING);
		//Animation("jump", movementState[BooleenStruct.ISJUMPING]);
		//_dashTarget = Vector2.zero;
		//ChangeJumpButtonSprite(0);
	}
	#endregion

	#region Collectibles & Glitches
	private void OnTriggerEnter2D(Collider2D col) {
		Transform colTransform = col.transform;
		TimedGlitch glitchContainer = colTransform.GetComponent<TimedGlitch>();

		if(colTransform.tag == "Glitch")
			StartGlitch(glitchContainer);
		else if(colTransform.tag == "Collectable") {
			switch(colTransform.GetComponent<Collectable>().CollectableName()) {
				case "health_power_ups":
					AddHealth();
					break;
				case "sprint_power_ups":
					FillStamina();
					break;
				case "speed_power_ups":
					StartGlitch(glitchContainer);
					break;
				case "jump_power_ups":
					StartGlitch(glitchContainer);
					break;
				case "defense_power_ups":
					StartGlitch(glitchContainer);
					break;
				case "Ruby":
					break;
				case "Key":
					AddKey();
					break;
				default:
					break;
			}
		}
	}

	private void StartGlitch(TimedGlitch glitchContainer) {
		string glitch;
		float timer;
		glitchContainer.GlitchInfo(out glitch, out timer);
	}
	#endregion
}