using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class NewPlayerController : NewPlayerMotor {
	public bool IsRecovering { get { return Time.time - _recoveryTimer < _recoveryDelay; } }
	public bool IsInvisible { get; set; }
	public bool IsInvincible { get; set; }
	public float SpeedBoost { get; set; }
	public float WalkingSpeed { get { return _stats.walkingSpeed; } }
	public float RunningSpeed { get { return _stats.runningSpeed * SpeedBoost; } }
	public float SprintingSpeed { get { return _stats.sprintingSpeed * SpeedBoost; } }
	public float CurrentStamina { get; private set; }

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
	private bool _isStaminaRefilling;

	private void Awake() {
		CustomStart();
	}

	protected override void CustomStart() {
		base.CustomStart();

		_colliders = new NewPlayerColliders(transform);
		_recoveryDelay = 0.4f;

		SetToDefault();
	}

	public void SetToDefault() {
		IsInvisible = false;
		IsInvincible = false;
		SpeedBoost = 1.0f;

		_dash = null;
		_stats = new NewPlayerStatistics();
		_glitches = new List<NewPlayerGlitch>();
		wallJumpDelay = 0.2f;
		slideDelay = 0.6f;

		_range = 5;
		_key = 0;
		_jumpBoost = 1f;
		_recoveryTimer = -_recoveryDelay;
		CurrentStamina = _stats.stamina;
	}

	public void Restart() {
		SetToDefault();

		RuntimeUI.GetStartTimer = false;

		_rigidBody.velocity = Vector3.zero;
		playerUI.ShowKeys(_key);
		playerUI.CheckHealth(_stats.health);
		playerUI.FillSprintBar(CurrentStamina / _stats.stamina);
		transform.position = LevelManager.Instance.spawnPoint;

		LevelManager.Instance.ReloadLevel();
		runtimeEditorUI.transform.GetComponent<RuntimeUI>().ResetTime();
		LevelManager.Instance.finishLoading = true;
	}

	#region Stamina
	public void FillStamina() {
		CurrentStamina = _stats.stamina;
		playerUI.FillSprintBar(CurrentStamina / _stats.stamina);
	}

	public bool UseStamina() {
		if(CurrentStamina > 0) {
			_isStaminaRefilling = false;
			CurrentStamina -= _stats.staminaUseSpeed * Time.deltaTime;
			playerUI.FillSprintBar(CurrentStamina / _stats.stamina);
			return true;
		}
		else {
			CurrentStamina = 0;
			return false;
		}
	}

	public IEnumerator RegenStaminaTimer() {
		yield return new WaitForSeconds(2f);
		_isStaminaRefilling = true;
	}

	public void RegenStamina() {
		if(_isStaminaRefilling) {
			if(CurrentStamina + _stats.staminaUseSpeed * Time.deltaTime < _stats.stamina) {
				CurrentStamina += _stats.staminaUseSpeed * Time.fixedDeltaTime;
				playerUI.FillSprintBar(CurrentStamina / _stats.stamina);
			}
			else {
				CurrentStamina = _stats.stamina;
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
		if(!IsInvincible && !IsRecovering && (CurrentState.CompareTo(PlayerStates.Dash) != 0 || overrideDash)) {
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
	public void ToggleGravity(bool isOn) {
		_rigidBody.gravityScale = isOn ? 1.0f : 0.0f;
	}

	public void RemoveGlitch(NewPlayerGlitch glitch) {
		_glitches.Remove(glitch);
	}

	private void OnTriggerEnter2D(Collider2D col) {
		Transform colTransform = col.transform;
		TimedGlitch glitchContainer = colTransform.GetComponent<TimedGlitch>();

		if(glitchContainer != null)
			StartGlitch(glitchContainer);
		else if(colTransform.tag == "Collectable") {
			switch(colTransform.name.Substring(0, colTransform.name.Length - 7)) {
				case "health_power_ups":
					AddHealth();
					Destroy(colTransform.gameObject);
					break;
				case "sprint_power_ups":
					FillStamina();
					Destroy(colTransform.gameObject);
					break;
				case "Ruby":
					Destroy(colTransform.gameObject);
					break;
				case "Key":
					AddKey();
					Destroy(colTransform.gameObject);
					break;
				default:
					break;
			}
		}
	}

	private void StartGlitch(TimedGlitch glitchContainer) {
		string glitchName;
		float timer;
		glitchContainer.GlitchInfo(out glitchName, out timer);

		foreach(NewPlayerGlitch glitch in _glitches) {
			if(glitch.GetType().ToString().Contains(glitchName)) {
				glitch.ResetTimer();
				return;
			}
		}

		if(glitchName.Contains("DamageJump")) ;
		else if(glitchName == "Flying")
			_glitches.Add(new NewPlayerFlying(this, timer));
		else if(glitchName == "Invincible")
			_glitches.Add(new NewPlayerInvincible(this, timer));
		else if(glitchName == "Invisible")
			_glitches.Add(new NewPlayerInvisible(this, timer));
		else if(glitchName == "Lag")
			_glitches.Add(new NewPlayerLag(this, timer));
		else if(glitchName == "SpeedBoost")
			_glitches.Add(new NewPlayerSpeedBoost(this, timer));
	}
	#endregion
}