using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class NewPlayerController : NewPlayerMotor {
	public bool IsInvisible { get; set; }
	public bool IsInvincible { get; set; }
	public float SpeedBoost { get; set; }
	public float CurrentStamina { get; private set; }
	public bool IsRecovering { get { return Time.time - _recoveryTimer < _recoveryDelay; } }
	public float WalkingSpeed { get { return _stats.walkingSpeed; } }
	public float RunningSpeed { get { return _stats.runningSpeed * SpeedBoost; } }
	public float SprintingSpeed { get { return _stats.sprintingSpeed * SpeedBoost; } }

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
		Time.timeScale = 1.0f;
		_rigidBody.gravityScale = 1.0f;

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
		LevelManager.Instance.IsPaused = true;

		SetToDefault();
		
		RuntimeUI.GetStartTimer = false;

		_rigidBody.velocity = Vector3.zero;
		playerUI.ShowKeys(_key);
		playerUI.CheckHealth(_stats.health);
		playerUI.FillSprintBar(CurrentStamina / _stats.stamina);
		
		runtimeEditorUI.transform.GetComponent<RuntimeUI>().ResetTime();

		transform.SetParent(null);
		LevelManager.Instance.ReloadLevel();
		
		_grounds = new List<Collider2D>();
		_walls = new List<Collider2D>();

		transform.position = LevelManager.Instance.spawnPoint;

		LevelManager.Instance.IsPaused = false;
	}

	#region Stamina
	//Fills up the stamina bar and updates the stamina UI.
	public void FillStamina() {
		CurrentStamina = _stats.stamina;
		playerUI.FillSprintBar(CurrentStamina / _stats.stamina);
	}

	//Consumes stamina and updates the stamina UI.
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

	//Triggers stamina regeneration after the player stopped consuming it for a set amount of time.
	public IEnumerator RegenStaminaTimer() {
		yield return new WaitForSeconds(2f);
		_isStaminaRefilling = true;
	}

	//Regenerate stamina over time.
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
	//Changes the player colliders when jumping to better fit the animation and have smoother corner collisions.
	public void ToggleJumpingColliders(bool enable) {
		_colliders.mainEdgeCollider.points = enable ? _colliders.jumpingCollidersPoints : _colliders.normalCollidersPoints;
	}

	//Changes the player colliders when sliding to better fit the animation and have smoother sliding collisions.
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
	//Calculates and returns the player's jump velocity.
	public float Jump() {
		float jumpForce = _stats.jumpForce * _jumpBoost;
		//Set jumpBoost to default.
		_jumpBoost = 1f;
		return jumpForce;
	}

	//Force changes the player's state to Jump and modifies the jump boost ( if specified ).
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

				_stats.health -= damage;
				playerUI.CheckHealth(_stats.health);

				_recoveryTimer = Time.time;
			}
		}
	}

	public void Kill() {
		Restart();
		LevelManager.Instance.IsPaused = false;
	}
	#endregion

	#region Dash
	public void StartDash(bool isSprinting = false) {
		if(EnemyList.IsDashValid(transform.position, Direction))
			_dash = new NewPlayerDash(transform.position, EnemyList.GetDash(transform.position, Direction), isSprinting);
		else
			_dash = new NewPlayerDash(transform.position, Direction, isSprinting);
	}

	public bool LerpDash() {
		Vector2 target = _dash.enemyTarget ? (Vector2)_dash.enemyTarget.transform.position : _dash.dashTarget;
		float magnitude = ((Vector2)transform.position - target).magnitude;
		_dash.dashTimer += _dash.dashSpeed / magnitude * Time.deltaTime;
		transform.position = Vector2.Lerp(transform.position, target, _dash.dashTimer);

		if(_dash.dashTarget != null && (Vector2)transform.position == target)
			return true;

		return false;
	}

	public void ResetDash() {
		_dash = null;
	}
	#endregion

	#region Collectibles & Glitches
	//Used by the flying glitch class to toggle gravity on and off.
	public void ToggleGravity(bool isOn) {
		_rigidBody.gravityScale = isOn ? 1.0f : 0.0f;
	}

	//Removes a glitch from the list ( used by the glitch classes to remove themselves when they're over ).
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