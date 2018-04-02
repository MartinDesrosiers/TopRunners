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
	private EdgeCollider2D _edgeCollider;
	private Vector2[] _normalCollidersPoints, _jumpingCollidersPoints;
	private ushort _health, _strength, _range, _key;
	private float _jumpForce, _jumpBoost;
	private bool _isInvincible;

	private void Awake() {
		CustomStart();
	}

	protected override void CustomStart() {
		_edgeCollider = transform.GetChild(0).GetChild(0).GetComponent<EdgeCollider2D>();
		_normalCollidersPoints = new Vector2[4];
		_jumpingCollidersPoints = new Vector2[4];

		for(int i = 0; i < _edgeCollider.pointCount; i++) {
			_normalCollidersPoints[i] = _edgeCollider.points[i];
			_jumpingCollidersPoints[i] = _normalCollidersPoints[i];
		}

		_jumpingCollidersPoints[1] = new Vector2(0.3f, -0.45f);
		_jumpingCollidersPoints[2] = new Vector2(-0.55f, -0.98f);

		_colliders = transform.GetChild(0).gameObject;

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

	public void ResetDash() {
		//inEnemiesRange = false;
		//ResetBool(true, BooleenStruct.ISJUMPING);
		//Animation("jump", movementState[BooleenStruct.ISJUMPING]);
		//_dashTarget = Vector2.zero;
		//ChangeJumpButtonSprite(0);
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