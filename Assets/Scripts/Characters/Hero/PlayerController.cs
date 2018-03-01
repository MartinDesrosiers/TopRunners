using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public struct BooleenStruct
{
    public const int ISROLLING = 0, ISRUNNING = 1, ISDASHING = 2, ISJUMPING = 3,
                        ISIDLE = 4, WALLJUMPING = 5, WALLED = 6, ISIDLEAIR = 7, ISJUMPATTACK = 8, ISDASHIDLE = 9;
}
public class PlayerController : CharacterMotor
{
	#region Variables
	List<Coroutine> glitches = new List<Coroutine>();
    TimeSlower timeSlower;
	[SerializeField]
	public PlayerUI playerUI;
    GameObject colliders;
    GameObject body;
    GameObject ennemyObj;
    EdgeCollider2D edgeCollider;
    public GameObject jumpButton;
    InputScript inputScript;
    public Sprite[] jumpDash;
    public LayerMask touchInputMask;
	public bool isInvisible;
    bool inEnemiesRange;
    bool _flip;
    bool _gettingHurt;
    bool _isDead;
    bool _isRecovering;
	bool _refreshPlayer = true;
    List<SpriteRenderer> playerColor;

    //Collider2D hit;
    public ushort _health, _combo, _strength, _range;
    
    public RuntimeEditorUI RuntimeEditorUI;
    private Vector2 _dashTarget, afterHit;
    private Vector3 _ennemyPos;
    private Vector2[] standWallCollider = { new Vector2(.33f, .7f), new Vector2(.33f, -1f) };
    private Vector2[] rollWallCollider = { new Vector2(.45f, -.6f), new Vector2(.45f, -1f) };
    Vector2[] normalCollidersPoints;
    Vector2[] jumpingCollidersPoints;

    private short _key;
    bool[] movementState;
    bool jump, cantJump, roll, isSprintRefilling, isInvincible, playerControl;
    //Player statistics.
    float[] axisXY;
    float inAirSpeed; // FIX
    float _maxWallJumpDist;
    float originalXPos;
    float decelerationSpeed;
    float maxDashDistance;
    //float _dashRecovery;
    float _acceleration, _topSpeed, _speed, _stamina/*, _propulsion, _recuparation, _dashSpeed, _dodge*/;
    float _currentStamina, _staminaUseSpeed;
    float _jumpForce, _speedModifier, _maxJumpHeight, _playerHeight;
    float _acc;
    float _timer;
    float _convoyerBeltForce;
    string animName;
    private bool[] _timedGlitches = new bool[8];

    const int X = 0;
    const int Y = 1;

#endregion
    #region GET SET 
    public bool[] GetMovementState { get { return movementState; } }
    public short GetKey { get { return _key; } }
    public InputScript GetInputScript { get { return inputScript; } }
    public bool GetHurt { get { return _gettingHurt; } set { _gettingHurt = value; } }
    public bool GetInEnemiesRange { get { return inEnemiesRange; } set { inEnemiesRange = value; } }
    public bool GetIsWalled { get { return _isWalled; } }
    public bool SetIsDead { set { _isDead = value; } }
    public bool SetCantJump { set { cantJump = value; } }
    public float SetSpeed { get { return _speed; } set { _speed = value; } }
    public float SetConvoyerBeltForce { get { return _convoyerBeltForce; } set { _convoyerBeltForce = value; } }
    public PlayerUI GetPlayerUI { get { return playerUI; } }
    #endregion
    private void Awake() {
        Initialize(true);
    }

    public void Initialize(bool t_playerControl) {
        //GameManager.Instance.LoadStats();
        //Initialize edges points collider;
        edgeCollider = transform.GetChild(0).GetChild(0).GetComponent<EdgeCollider2D>();
        normalCollidersPoints = new Vector2[4];
        jumpingCollidersPoints = new Vector2[4];
        //Debug.Log();
        for (int i = 0; i < edgeCollider.pointCount; i++)
        {
            normalCollidersPoints[i] = edgeCollider.points[i];
            jumpingCollidersPoints[i] = normalCollidersPoints[i];
        }
        jumpingCollidersPoints[1] = new Vector2(0.3f,-0.45f);
        jumpingCollidersPoints[2] = new Vector2(-0.55f,-0.98f);
        playerControl = t_playerControl;
        colliders = transform.GetChild(0).gameObject;
        inputScript = new InputScript();
        axisXY = new float[2];
        movementState = new bool[10];
        inAirSpeed = 0.5f;  // FIX
        if (t_playerControl)
        {
            playerColor = new List<SpriteRenderer>();
            PlayerBodyColor();
            for (int i = 0; i < _timedGlitches.Length; i++)
            {
                _timedGlitches[i] = new bool();
                _timedGlitches[i] = false;
            }
        }
        ReInitialize(GameManager.Instance.myList);
    }

    //ushort health, ushort combo, ushort strength, ushort range,float topSpeed, float acceleration, float stamina, float recuperation, float propulsion, float dashSpeed, float dodge
    public void ReInitialize(List<int> list)
    {
        rg = transform.GetComponent<Rigidbody2D>();
        _convoyerBeltForce = 0;
        animator.Play("idle");
        for (int i = 0; i < movementState.Length; i++)
            movementState[i] = false;
        _gettingHurt = false;
        //Player Stats
        _health = 3; //(ushort)list[0];
        _strength = 4; //(ushort)list[2];
        _range = 5; //(ushort)list[3];
        _topSpeed = 10; //list[4];
        _acceleration = 2; //_acc = list[5];
        _stamina = 10; //list[6];
        _maxWallJumpDist = 3;
        decelerationSpeed = 2;
        _isRecovering = false;
        _jumpForce = 6f; //Down from 10f
        _maxJumpHeight = 2f; //Possibly redundant
        _speed = 1f;
        _speedModifier = 1.5f;
        _currentStamina = _stamina;
        _staminaUseSpeed = 5f;
        _key = 0;
        originalXPos = 0f;
        jump = roll = cantJump = isSprintRefilling = _isDead = isInvincible = false;
        axisXY[X] = axisXY[Y] = 0;
        _dashTarget = Vector2.zero;
        RuntimeUI.GetStartTimer = false;
    }
    public void PlayerBodyColor()
    {
        //for every part of the body the character has, it put that in a list to make all of them invisible when the glitch is taken
        GameObject body = transform.GetChild(1).GetChild(0).gameObject;
        playerColor.Add(body.transform.GetComponent<SpriteRenderer>());
        for (int i = 0; i < body.transform.childCount; i++)
        {
            playerColor.Add(body.transform.GetChild(i).GetComponent<SpriteRenderer>());
            for (int j = 0; j < body.transform.GetChild(i).childCount; j++)
            {
            playerColor.Add(body.transform.GetChild(i).GetChild(j).GetComponent<SpriteRenderer>());
                if (body.transform.GetChild(i).GetChild(j).childCount > 0)
                {
                    for (int k = 0; k < body.transform.GetChild(i).GetChild(j).childCount; k++)
                    {
                        playerColor.Add(body.transform.GetChild(i).GetChild(j).GetChild(k).GetComponent<SpriteRenderer>());
                        if (body.transform.GetChild(i).GetChild(j).GetChild(k).childCount > 0)
                        {
                            for (int l = 0; l < body.transform.GetChild(i).GetChild(j).GetChild(k).childCount; l++)
                                playerColor.Add(body.transform.GetChild(i).GetChild(j).GetChild(k).GetChild(l).GetComponent<SpriteRenderer>());
                        }
                    }
                }
            }
        }
    }

    public void PlayerUIInit() {
        playerUI.FillSprintBar(_currentStamina / _stamina);
        playerUI.CheckHealth(_health);
    }

    private void Update()
    {
        //TODO capter les controls dans l'update et appliquer leurs behaviors dans le fixed update
        if (!LevelManager.Instance.isPaused && GameManager.Instance.currentState == GameManager.GameState.RunTime && !_isDead)
        {
            EnemyList.UpdateAreaAlpha(transform.position);
            axisXY = inputScript.PlayerInput(true);
            if (!movementState[BooleenStruct.WALLJUMPING] && !movementState[BooleenStruct.ISDASHING] && !_gettingHurt)
            {
                if (axisXY[X] != 0)
                {
                    if (!movementState[BooleenStruct.ISRUNNING] && _isGrounded && !jump && !_isWalled && !movementState[BooleenStruct.ISROLLING])
                    {
                        ResetBool(true, BooleenStruct.ISRUNNING);
                        Animation("run", movementState[BooleenStruct.ISRUNNING]);
                        ChangeJumpButtonSprite(0);
                    }
                    if ((axisXY[X] < 0 && transform.localScale.x > 0) || (axisXY[X] > 0 && transform.localScale.x < 0))
                    {
                        Flip();
                        if (movementState[BooleenStruct.ISJUMPING]) // FIX
                            _speed = inAirSpeed; // FIX
                    }
                    if (!_isWalled)
                    {
                        if (!inputScript.GetSprint)
                        {
                            Movement(Acceleration(axisXY[X]) * _speed + _convoyerBeltForce, rg.velocity.y);
                        }
                        else
                        {
                            Movement(Acceleration(axisXY[X] * _speed * _speedModifier) + _convoyerBeltForce, rg.velocity.y);
                        }
                    }
                }
                else
                {
                    if (_isGrounded && !movementState[BooleenStruct.ISIDLE] && !jump && !movementState[BooleenStruct.ISROLLING] && !cantJump)
                    {
                        SetToIdle();
                        ChangeJumpButtonSprite(0);
                    }
                    else if (movementState[BooleenStruct.ISJUMPING]) // FIX
                        _speed = inAirSpeed; // FIX
                    Movement(rg.velocity.x - (rg.velocity.x * Time.deltaTime), rg.velocity.y);
                }
            }
            else if (_gettingHurt)
            {
                Movement(afterHit.x, afterHit.y);
            }
            else if (movementState[BooleenStruct.WALLJUMPING])
                PerformeWallFlip();
            else
            {
                if (!_gettingHurt)
                    Dash();
            }
            if (axisXY[Y] > 0 && !movementState[BooleenStruct.ISDASHING] && !movementState[BooleenStruct.WALLJUMPING] && !cantJump)
            {
                if (!transform.GetChild(0).GetChild(0).gameObject.activeInHierarchy)
                    ToggleColliders(false);
                if (_isGrounded && !movementState[BooleenStruct.ISJUMPING] && !movementState[BooleenStruct.WALLJUMPING] && !jump)
                {
                    SetJump("jump", BooleenStruct.ISJUMPING, true, transform.position.y);
                    if (inEnemiesRange)
                        ChangeJumpButtonSprite(1);
                }
                else if (_isWalled && !jump && !movementState[BooleenStruct.ISROLLING])
                {
                    SetJump("wallJump", BooleenStruct.WALLJUMPING, true, transform.position.y);
                    Movement(rg.velocity.x, Jump());
                    if (animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("Base Layer")).IsName("wall_jump"))
                        animator.Play("wall_jump", 0, 0);
                    originalXPos = transform.position.x;
                    WallFlip(transform.localScale.x * -1);
                }
                else if (inEnemiesRange && !movementState[BooleenStruct.ISDASHING] && !jump)
                {
                    RaycastHit2D hit;
                    _ennemyPos = ennemyObj.transform.position;
                    if (!DashAttemp(ennemyObj, out hit))
                        return;
                    SetJump("jumpAttack", BooleenStruct.ISDASHING, true, transform.position.y);
                    Normalized();
                    GetAngle();
                    if (_dashTarget.x < 0)
                        transform.localScale = new Vector2(-1, 1);
                }
                if (jump && !movementState[BooleenStruct.WALLJUMPING])
                {
                    if (transform.position.y - _playerHeight < _maxJumpHeight && rg.velocity.y >= 0)
                        Movement(rg.velocity.x, Jump());
                }
            }
            else if (axisXY[Y] < 0 && !roll)
            {
                Roll(true);
            }
            else if (axisXY[Y] == 0)
            {
                jump = false;
                roll = false;
            }
            if (!RuntimeUI.GetStartTimer)
                if (axisXY[0] != 0 || axisXY[1] != 0)
                    RuntimeUI.GetStartTimer = true;
        }
        else if (GameManager.Instance.currentState == GameManager.GameState.LevelEditor)
        {
            if (_refreshPlayer)
            {
                _refreshPlayer = false;
            }
        }
    }

    private void FixedUpdate() {
        if(!LevelManager.Instance.isPaused) {
            //Debug.Log(animator.GetCurrentAnimatorClipInfo(animator.GetLayerIndex("Base Layer")).Length);
            animator.speed = _speed;
            animator.SetFloat("VelY", rg.velocity.y);

            if (!_isGrounded && _isWalled && !movementState[BooleenStruct.WALLED]) {
                IsWalled(true);
            }
            else if (rg.velocity.y < 0 && !_isWalled && !movementState[BooleenStruct.ISIDLEAIR] && !movementState[BooleenStruct.ISROLLING] && !movementState[BooleenStruct.ISDASHING]) {
                ResetBool(true, BooleenStruct.ISIDLEAIR);
                Animation("idleAir", movementState[BooleenStruct.ISIDLEAIR]);
                edgeCollider.points = normalCollidersPoints;
            }

            if (inputScript.GetSprint) {
                if (_currentStamina > 0) {
                    _currentStamina -= _staminaUseSpeed * Time.fixedDeltaTime;
                    playerUI.FillSprintBar(_currentStamina / _stamina);
                }
                else {
                    inputScript.GetSprint = false;
                    StartCoroutine(SprintRegen());
                    _currentStamina = 0;
                }
            }
            else if(_currentStamina < _stamina && !isSprintRefilling)
                StartCoroutine(SprintRegen());
        else if(isSprintRefilling) {
                if (_currentStamina + _staminaUseSpeed * Time.fixedDeltaTime < _stamina) {
                    _currentStamina += _staminaUseSpeed * Time.fixedDeltaTime;
                    playerUI.FillSprintBar(_currentStamina / _stamina);
                }
                else {
                    _currentStamina = _stamina;
                    isSprintRefilling = false;
                }
            }
            if (_gettingHurt) {
                if (Time.time - _timer > .4f)
                    _gettingHurt = false;
            }
		}
    }
    //Set the jump variable. Avoid repetitive line
    void SetJump(string AnimName, int moveState, bool tJump, float height)
    {
        ResetBool(tJump, moveState);
        Animation(AnimName, movementState[moveState]);
        _playerHeight = height;
        jump = tJump;
        edgeCollider.points = jumpingCollidersPoints;
    }

    void SetToIdle() {
        Movement(0, rg.velocity.y);
        _acc = _acceleration;
        ResetBool(true, BooleenStruct.ISIDLE);
        Animation("idle", movementState[BooleenStruct.ISIDLE]);
        _acc = _acceleration;
    }

    float Acceleration(float dir) {
        float result;
        if (_acc > 1)
            _acc -= Time.fixedDeltaTime;
        else
            _acc = 1;
        result = dir * _topSpeed / _acc;
        return result;
    }

    //set every bool to false and to true the one passed in parameter
    void ResetBool(bool b, int j) {
        for (int i = 0; i < movementState.Length; i++)
            movementState[i] = false;
        movementState[j] = b;
    }

    //Set Animation 
    public void Animation(string name, bool trueOrFalse) {
        SetAnimation(name, trueOrFalse);
    }

    //normalized the distance betwen the player and the enemies. use when dashing
    void Normalized() {
        _dashTarget = Vector3.Normalize(_ennemyPos - transform.position);
    }

    //calculate the angle between the player and the enemies. use when dashing
    float GetAngle() {
        float angle = Mathf.Atan2(transform.position.x - _ennemyPos.x, transform.position.y - _ennemyPos.y) * Mathf.Rad2Deg;
        return (90 - Mathf.Abs(angle)) * transform.localScale.x;// * -1;
    }
    bool DashAttemp(GameObject enemies, out RaycastHit2D hit)
    {
        Vector2 start = transform.position;
        Vector2 end = enemies.transform.position;
        colliders.SetActive(false);
        enemies.transform.parent.gameObject.SetActive(false);
        hit = Physics2D.Linecast(start, end);
        colliders.SetActive(true);
        enemies.transform.parent.gameObject.SetActive(true);
        if (hit.transform == null)
            return true;
        return false;
    }

    public void FillStamina() {
		_currentStamina = _stamina;
	}

	public IEnumerator SprintRegen() {
		yield return new WaitForSeconds(2f);
        if (!inputScript.GetSprint)
        {
            isSprintRefilling = true;
        }
	}
    //Roll for a determinate time or as long as there is something blocking the player the get back up
    public IEnumerator RollTimer()
    {
        float elapsedTime = 0f;

        while (elapsedTime < 1f || cantJump)
        {
            RaycastHit2D hit;
            hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2), Vector2.up, 1.5f, 1 << LayerMask.NameToLayer("Ground"));
           
            if (hit.transform != null)
            {
                if (!cantJump)
                    cantJump = true;
            }
            else
            {
                if (cantJump)
                    cantJump = false;
            }
            elapsedTime += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        Roll(false);
    }
    void ToggleColliders(bool state)
    {
        transform.GetChild(0).GetChild(0).gameObject.SetActive(!state);
        transform.GetChild(0).GetChild(1).gameObject.SetActive(state);
    }
    //Apply jump depending on the speed of the player
    public float Jump(float boost = 1f) {
        //Triggers the jump animation.
        jump = true;
        return _jumpForce * boost;
    }
    //start the coroutine of roll
    public void Roll(bool state)
    {
        ResetBool(state, BooleenStruct.ISROLLING);
        roll = true;
        if (state)
        {
            transform.GetChild(0).GetChild(2).GetComponent<EdgeCollider2D>().points = rollWallCollider;
            StartCoroutine(RollTimer());
        }
        else
            transform.GetChild(0).GetChild(2).GetComponent<EdgeCollider2D>().points = standWallCollider;
        ToggleColliders(state);
        Animation("roll", state);
    }
	private void WallFlip(float direction)
    {
        Flip();
	}
    //if jumping while walled, apply a velocity in the opposite direction of the wall
    void PerformeWallFlip()
    {
        int i = 1;
        if (transform.localScale.x < 0)
            i = -1;
        if (Mathf.Abs(Mathf.Abs(transform.position.x) - Mathf.Abs(originalXPos)) < _maxWallJumpDist && !_isWalled)
        {
            Movement(_topSpeed * (i - Time.deltaTime) * _speedModifier, Jump(1f));
        }
        else
        {
            SetBoolState(BooleenStruct.WALLJUMPING, false);
        }
    }

    public void Dash()
    {
        Movement(_dashTarget.x * 20f, _dashTarget.y * 20f);
        if(!animator.GetBool("jumpAttack"))
            Animation("jumpAttack", true);
    }

	public void ResetDash() {
        //Resets all dashing values to zero.
        inEnemiesRange = false;
        ResetBool(true, BooleenStruct.ISJUMPING);
        Animation("jump", movementState[BooleenStruct.ISJUMPING]);
		_dashTarget = Vector2.zero;
        ChangeJumpButtonSprite(0);
	}

	public void TakeDamage(ushort damage, bool overrideDash, bool hitOnLeftSide) {
		//If invincible glitch is active, ignore damage.
		if(!isInvincible) {
			if((!movementState[BooleenStruct.ISDASHING] || overrideDash) &&
                !_isRecovering) {
                if (damage >= _health) {
                    Animation("death", true);
					_health = 0;
					playerUI.CheckHealth(_health);
					Kill();
                }
                else {
                    Movement(0, 0);
                    _gettingHurt = true;
                    if (hitOnLeftSide)
                        afterHit = new Vector2(10, 1);
                    else
                        afterHit = new Vector2(-10, 1);
                    StartCoroutine(RecoverFromDamage());
                    _timer = Time.time;
                    _health -= damage;
                    playerUI.CheckHealth(_health);
                }
			}
		}
	}

    private IEnumerator RecoverFromDamage() {
        _isRecovering = true;
		float timer = 0f;
		//Debug.Log("DAMAGE FRAMES START - TIMER : " + timer);
        animator.SetBool("damage", true);
		while(timer < 2f * Time.timeScale) {
			//Delay is modified by timescale to work with Lag glitch.
			yield return new WaitForSeconds(2f * Time.timeScale * Time.deltaTime);
			timer += 2f * Time.timeScale * Time.deltaTime;
		}
        animator.SetBool("damage", false);
        _isRecovering = false;
	}

	//Start a coroutine for a given timed glitch.
	public void StartGlitchTimer(string glitchType, float delay) {
		glitches.Add(StartCoroutine(GlitchTimer(glitchType, delay)));
	}

	//Glitch Timer.
	private IEnumerator GlitchTimer(string glitchType, float delay) {
		if(!UpdateGlitch(glitchType, true))
			yield break;
		//Delay is modified by timescale to work with Lag glitch.
		yield return new WaitForSeconds(delay * Time.timeScale);

		UpdateGlitch(glitchType, false);
	}

	//Updates the state of a specified glitch.
	private bool UpdateGlitch(string glitchType, bool isFirstPass) {
		switch(glitchType) {
			case "Flying": {
				if(!_timedGlitches[1]) {
					_timedGlitches[1] = true;
					GetComponent<Rigidbody2D>().gravityScale = 0f;
				}
				else {
					if(isFirstPass)
						return false;

					_timedGlitches[1] = false;
					GetComponent<Rigidbody2D>().gravityScale = 1f;
				}
			}
			break;
			case "Invincible": {
				if(!_timedGlitches[2]) {
					_timedGlitches[2] = true;
					isInvincible = true;
				}
				else {
					if(isFirstPass)
						return false;

					_timedGlitches[2] = false;
					isInvincible = false;
				}
			}
			break;
			case "Invisible": {
                    if (!_timedGlitches[3])
                    {
                        _timedGlitches[3] = true;
                        Invisible(.2f);
                    }
                    else
                    {
                        if (isFirstPass)
                        {
                            return false;
                        }
                        _timedGlitches[3] = false;
                        Invisible(1f);
                    }
				isInvisible = isInvisible ? false : true;
			}
			break;
			case "Lag": {
				if(!_timedGlitches[4]) {
					_timedGlitches[4] = true;
					Time.timeScale = 0.25f;
				}
				else {
					if(isFirstPass)
						return false;

					_timedGlitches[4] = false;
					Time.timeScale = 1f;
				}
			}
			break;
			case "SpeedBoost": {
				if(!_timedGlitches[5])
					_timedGlitches[5] = true;
				else {
					if(isFirstPass)
						return false;
					_timedGlitches[5] = false;
				}
                    _speed = _timedGlitches[5] ? 1.5f : 1f;
                }
			break;
			case "Reach": {
				if(!_timedGlitches[5])
					_timedGlitches[5] = true;
				else {
					if(isFirstPass)
						return false;
					_timedGlitches[5] = false;
				}
				//_propulsion = _timedGlitches[5] ? 14f : 8f;
			}
			break;
			case "JumpBoost": {
				if(!_timedGlitches[7])
					_timedGlitches[7] = true;
				else {
					if(isFirstPass)
						return false;
					_timedGlitches[7] = false;
				}
				_jumpForce = _timedGlitches[7] ? 12f : 6f;
			}
			break;
			default:
				return false;
		}
		return true;
	}

    void Invisible(float alpha) {
        for (int i = 0; i < playerColor.Count; i++) {
			Color temp = Color.white;
            temp.a = alpha;
            playerColor[i].material.color = temp;
        }
    }

	public void Kill() {
        _isDead = true;
		StartCoroutine(Die());
    }

    public void Restart()
    {
        RuntimeUI.GetStartTimer = false;
        //LevelManager.Instance.isGhostReplayActive = true;
        //Stop all glitch coroutines and clear the coroutine list.
        for (int i = 0; i < glitches.Count; i++)
        {
            if (glitches[i] != null)
                StopCoroutine(glitches[i]);
        }
        glitches.Clear();
        _health = 3;
        _key = 0;
        rg.velocity = Vector3.zero;
        playerUI.ShowKeys(_key);
        playerUI.CheckHealth(_health);
        transform.position = LevelManager.Instance.spawnPoint;
        LevelManager.Instance.ReloadLevel();
        ReInitialize(GameManager.Instance.myList);
        RuntimeEditorUI.transform.GetComponent<RuntimeUI>().ResetTime();
        LevelManager.Instance.finishLoading = true;
    }

    private IEnumerator Die() {
        float timer = 0f;
        animator.Play("death");
        while (timer < 2f * Time.timeScale) {
            yield return new WaitForSeconds(2f * Time.timeScale * Time.deltaTime);
            timer += 2f * Time.timeScale * Time.deltaTime;
        }
        StartCoroutine(LevelManager.Instance.LoadingScreen());
        rg.velocity = Vector2.zero;
        Restart();
    }

    //use after dash to make the player jump after hit
    public void CheckPropulsion(float boost = 1f)
    {
        SetBoolState(BooleenStruct.ISJUMPING, true);  //FIX Dan 26 fevrier
        Animation("jump", movementState[BooleenStruct.ISJUMPING]);  //FIX Dan 26 fevrier
        jump = true;
        Movement(rg.velocity.x, Jump(boost));
        transform.rotation = new Quaternion(0f, 0f, 0f, 0f);
	}

    //name Speak by itself :p
	public void AddHealth() {
		if(_health < 3) {
			_health++;
			playerUI.CheckHealth(_health);
		}
	}

    public void AddKey() {
        _key++;
        playerUI.ShowKeys(_key);
    }

    public void RemoveKey() {
        _key--;
        playerUI.ShowKeys(_key);
    }

    public void IsWalled(bool iswalled, bool b = false) {
        _isWalled = iswalled;
        _speed = 1f; // FIX
        if (!_isGrounded && !movementState[BooleenStruct.ISROLLING] && b) {
            Animation("wallIdle", _isWalled);
            movementState[BooleenStruct.WALLED] = _isWalled;
        }
    }

	public void IsGrounded(bool tof) {
        _speed = 1f; // FIX
        _isGrounded = tof;
        jump = !_isGrounded;
        if (_isGrounded && !movementState[BooleenStruct.ISROLLING])
            Animation("idle", _isGrounded);
    }

    public bool GetBoolState(int i) {
        return movementState[i];
    }

    public void SetBoolState(int i, bool b) {
        movementState[i] = b;
    }

    public void ChangeJumpButtonSprite(int sprite)
    {
        if (playerControl)
            playerUI.jumpButton.GetComponent<UnityEngine.UI.Image>().sprite = jumpDash[sprite];
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.tag == "Glitch")
        {
            string glitch = "";
            float timer = 0.0f;
            col.transform.GetComponent<TimedGlitch>().GlitchInfo(ref glitch, ref timer);
            StartCoroutine(GlitchTimer(glitch, timer));
        }
        else if (col.transform.tag == "Collectable")
        {
            switch (col.transform.GetComponent<Collectable>().CollectableName())
            {
                case "health_power_ups":
                    AddHealth();
                    break;
                case "sprint_power_ups":
                    FillStamina();
                    break;
                case "Ruby": //TODO Add a function for the ruby
                    break;
                case "Key":
                    AddKey();
                    break;
                default:
                    Debug.Log(col.transform.GetComponent<Collectable>().CollectableName());
                    break;
            }
        }
        if (col.gameObject.name == "DashArea")
        {
            if (!_isGrounded && playerControl)
                ChangeJumpButtonSprite(1);
            inEnemiesRange = true;
            ennemyObj = col.gameObject;
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.name == "DashArea" && !movementState[BooleenStruct.ISDASHING])
        {
            inEnemiesRange = false;
            if (playerControl)
                ChangeJumpButtonSprite(0);
            ResetDash();
        }
    }
}