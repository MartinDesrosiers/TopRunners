using UnityEngine;

public class NewPlayerController : NewPlayerMotor {
	private float _recoveryTimer;
	private float _recoveryDelay = 0.4f;
	public bool IsRecovering { get { return Time.time - _recoveryTimer < _recoveryDelay; } }
	public bool IsInvisible { get; private set; }

	public RuntimeEditorUI runtimeEditorUI;
	public Animator animator;
	public PlayerUI playerUI;
	public float walkingSpeed, runningSpeed, wallJumpDelay, slideDelay;

	private GameObject _colliders;
	private GameObject _rollColliders;
	private EdgeCollider2D _mainEdgeCollider;
	private EdgeCollider2D _wallEdgeCollider;
	private Vector2[] _normalCollidersPoints, _jumpingCollidersPoints, _standingCollider, _slidingCollider;
	private ushort _health, _strength, _range, _key;
	private float _jumpForce, _jumpBoost;
	private bool _isInvincible;

	private void Awake() {
		CustomStart();
	}

	protected override void CustomStart() {
		_colliders = transform.GetChild(0).GetChild(0).gameObject;
		_rollColliders = transform.GetChild(0).GetChild(1).gameObject;
		_mainEdgeCollider = transform.GetChild(0).GetChild(0).GetComponent<EdgeCollider2D>();
		_wallEdgeCollider = transform.GetChild(0).GetChild(2).GetComponent<EdgeCollider2D>();
		_normalCollidersPoints = new Vector2[4];
		_jumpingCollidersPoints = new Vector2[4];

		for(int i = 0; i < _mainEdgeCollider.pointCount; i++) {
			_normalCollidersPoints[i] = _mainEdgeCollider.points[i];
			_jumpingCollidersPoints[i] = _normalCollidersPoints[i];
		}
		
		_jumpingCollidersPoints[1] = new Vector2(0.3f, -0.45f);
		_jumpingCollidersPoints[2] = new Vector2(-0.55f, -0.98f);
		_standingCollider = new Vector2[] { new Vector2(.33f, .7f), new Vector2(.33f, -1f) };
		_slidingCollider = new Vector2[] { new Vector2(.45f, -.6f), new Vector2(.45f, -1f) };

		base.CustomStart();
	}

	public void SetToDefault() {
		walkingSpeed = 2f;
		runningSpeed = 10f;
		wallJumpDelay = 0.2f;
		slideDelay = 0.6f;

		_health = 3;
		_strength = 4;
		_range = 5;
		_key = 0;
		_jumpForce = 6f;
		_jumpBoost = 1f;
		_isInvincible = false;
		_recoveryTimer = -_recoveryDelay;
	}

	public void Restart() {
		RuntimeUI.GetStartTimer = false;

		_rigidBody.velocity = Vector3.zero;
		playerUI.ShowKeys(_key);
		playerUI.CheckHealth(_health);
		transform.position = LevelManager.Instance.spawnPoint;

		LevelManager.Instance.ReloadLevel();
		runtimeEditorUI.transform.GetComponent<RuntimeUI>().ResetTime();
		LevelManager.Instance.finishLoading = true;

		SetToDefault();
	}

	public void ToggleJumpingColliders(bool enable) {
		_mainEdgeCollider.points = enable ? _jumpingCollidersPoints : _normalCollidersPoints;
	}

	public void ToggleSlidingColliders(bool enable) {
		_wallEdgeCollider.points = enable ? _slidingCollider : _standingCollider;
		_rollColliders.SetActive(enable);
		_colliders.SetActive(!enable);
	}

	public void ResetDash() {
		//inEnemiesRange = false;
		//ResetBool(true, BooleenStruct.ISJUMPING);
		//Animation("jump", movementState[BooleenStruct.ISJUMPING]);
		//_dashTarget = Vector2.zero;
		//ChangeJumpButtonSprite(0);
	}

	public void AddHealth(ushort hp = 1) {
		_health += hp;
		if(_health > 3)
			_health = 3;
		playerUI.CheckHealth(_health);
	}

	public void Kill() {
		_stateMachine.CurrentState = PlayerStates.Dead;
	}

	public float Jump() {
		float jumpForce = _jumpForce * _jumpBoost;
		_jumpBoost = 1f;
		return jumpForce;
	}

	public void ForceJump(float boost = 1f) {
		_jumpBoost = boost;
		_stateMachine.CurrentState = PlayerStates.Jump;
	}

	public void TakeDamage(ushort damage, bool overrideDash, bool hitOnLeftSide) {
		if(!_isInvincible && !IsRecovering && (CurrentState.CompareTo(PlayerStates.Dash) != 0 || overrideDash)) {
			if(damage >= _health) {
				_health = 0;
				playerUI.CheckHealth(_health);
				Kill();
			}
			else {
				Move(new Vector2(0, 0));

				//if(hitOnLeftSide)
				//	afterHit = new Vector2(10, 1);
				//else
				//	afterHit = new Vector2(-10, 1);
				//StartCoroutine(RecoverFromDamage());

				_health -= damage;
				playerUI.CheckHealth(_health);

				_recoveryTimer = Time.time;
			}
		}
	}
}