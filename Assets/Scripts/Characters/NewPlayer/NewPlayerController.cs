using UnityEngine;

public class NewPlayerController : NewPlayerMotor {
	public bool IsRecovering { get { return Time.time - _recoveryTimer < _recoveryDelay; } }
	public bool IsInvisible { get; private set; }
	public float WalkingSpeed { get { return _stats.walkingSpeed; } }
	public float RunningSpeed { get { return _stats.runningSpeed; } }
	public float SprintingSpeed { get { return _stats.sprintingSpeed; } }

	public RuntimeEditorUI runtimeEditorUI;
	public Animator animator;
	public PlayerUI playerUI;
	public float wallJumpDelay, slideDelay;

	private NewPlayerStatistics _stats;
	private NewPlayercolliders _colliders;
	private NewPlayerDash _dash;
	private ushort _range, _key;
	private float _jumpBoost, _recoveryTimer, _recoveryDelay;
	private bool _isInvincible;

	private void Awake() {
		CustomStart();
	}

	protected override void CustomStart() {
		_colliders = new NewPlayercolliders(transform);
		_recoveryDelay = 0.4f;

		base.CustomStart();
	}

	public void SetToDefault() {
		_dash = null;
		_stats = new NewPlayerStatistics();
		wallJumpDelay = 0.2f;
		slideDelay = 0.6f;
		
		_range = 5;
		_key = 0;
		_jumpBoost = 1f;
		_isInvincible = false;
		_recoveryTimer = -_recoveryDelay;
	}

	public void Restart() {
		RuntimeUI.GetStartTimer = false;

		_rigidBody.velocity = Vector3.zero;
		playerUI.ShowKeys(_key);
		playerUI.CheckHealth(_stats.health);
		transform.position = LevelManager.Instance.spawnPoint;

		LevelManager.Instance.ReloadLevel();
		runtimeEditorUI.transform.GetComponent<RuntimeUI>().ResetTime();
		LevelManager.Instance.finishLoading = true;

		SetToDefault();
	}

	public void ToggleJumpingColliders(bool enable) {
		_colliders.mainEdgeCollider.points = enable ? _colliders.jumpingCollidersPoints : _colliders.normalCollidersPoints;
	}

	public void ToggleSlidingColliders(bool enable) {
		_colliders.wallEdgeCollider.points = enable ? _colliders.slidingCollider : _colliders.standingCollider;
		_colliders.rollColliders.SetActive(enable);
		_colliders.colliders.SetActive(!enable);
	}

	public void ResetDash() {
		_dash = null;
		//inEnemiesRange = false;
		//ResetBool(true, BooleenStruct.ISJUMPING);
		//Animation("jump", movementState[BooleenStruct.ISJUMPING]);
		//_dashTarget = Vector2.zero;
		//ChangeJumpButtonSprite(0);
	}

	public void AddHealth(ushort hp = 1) {
		_stats.health += hp;
		if(_stats.health > 3)
			_stats.health = 3;
		playerUI.CheckHealth(_stats.health);
	}

	public void AddKey() {
		_key++;
		playerUI.ShowKeys(_key);
	}

	public void RemoveKey() {
		_key--;
		playerUI.ShowKeys(_key);
	}

	public void Kill() {
		_stateMachine.CurrentState = PlayerStates.Dead;
	}

	public float Jump() {
		float jumpForce = _stats.jumpForce * _jumpBoost;
		_jumpBoost = 1f;
		return jumpForce;
	}

	public void ForceJump(float boost = 1f) {
		_jumpBoost = boost;
		_stateMachine.CurrentState = PlayerStates.Jump;
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

	public void StartDash(bool isSprinting = false) {
		_dash = new NewPlayerDash(transform.position, EnemyList.GetDash(transform.position, Direction), isSprinting);
	}

	public void LerpDash() {
		float magnitude = (transform.position - _dash.dashTarget.transform.position).magnitude;
		_dash.dashTimer += _dash.dashSpeed / magnitude * Time.deltaTime;
		transform.position = Vector2.Lerp(transform.position, _dash.dashTarget.transform.position, _dash.dashTimer);
	}
}