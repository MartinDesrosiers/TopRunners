using UnityEngine;
using System.Collections.Generic;

public enum DashState { passive, attacking, returning };

public class EnemyAI : CharacterMotor {
    //Is the character active ( used for LevelEditor ).
	NewPlayerController _player;
    GameObject _objPlayer;
    SpriteRenderer enemiesRender;
    public DashState dashState = DashState.passive;
    public Animator enemyAnimator;
    float maxDistance;

    public bool lookAtHero = false;
	public bool isFlying = false;
	public bool IsInvincible = false;

	public bool stickToWalls = false;

    public bool useAdvancedBehaviors;
	public bool[] abList = new bool[4];
	private List<AB_Interface> advancedBehaviors = new List<AB_Interface>();
	private AB_Gaz _gaz;

	public enum BasicBehavior { LeftRight, TopDown, WaveLR, FollowReturn, Motionless };
	public BasicBehavior basicBehavior;

	//Does the model flip vertically when changing vertical direction.
	public bool flipVertically = false;

	public bool ignoreWalls = false;
	public bool ignoreHoles = false;

    float escapeVelModifier = 1.0f;
    public float movementSpeed = 50.0f;
	public float angleSpeed = 0.0f;
	public float maxAngle = 0.0f;

	public float distance = 0.0f;
    public float GetdirX { get { return horiAxes; }  set { horiAxes = value; } }

    //Enemy's original position ( used to calculate distance traveled ).
    private Vector2 origin;

	public bool overrideMove = false;
	public bool isInit = false;

	public int health;

    #region GETSET
    public float SetEscapeModifier { set { escapeVelModifier = value; } }
    #endregion

	public void OnEnable() {
		if (origin == Vector2.zero)//pour prevenir au cas ou OnEnable is called first;
			return;

		transform.position = origin;
		GetComponent<Rigidbody2D>().velocity = Vector2.zero;
	}

	public void TakeDamage() {
		if(!IsInvincible) {
			health--;
			if(health < 1) {
				Kill();
				_player.ResetDash();
			}
		}
	}

	public void Kill() {
		gameObject.SetActive(false);
	}

	private void Start() {
		origin = transform.position;
		enemiesRender = transform.GetChild(1).GetChild(0).GetComponent<SpriteRenderer>();
		rg = transform.GetComponent<Rigidbody2D>();
		_objPlayer = GameObject.Find("PlayerTest").gameObject;
		_player = _objPlayer.GetComponent<NewPlayerController>();
		movementSpeed = 50.0f;
		maxDistance = 6f;
		
		//Changes certain basic variables depending on basic behavior type.
		switch (basicBehavior) {
			case BasicBehavior.LeftRight:
				horiAxes = 1f; vertAxis = 0f;
				break;
		
			case BasicBehavior.TopDown:
				horiAxes = 0f; vertAxis = 1f;
				ignoreHoles = true;
				_gaz = new AB_Gaz(this);
				break;
		
			case BasicBehavior.WaveLR:
				horiAxes = 1f; vertAxis = 0f;
				ignoreHoles = true;
				break;
		
			case BasicBehavior.Motionless:
				horiAxes = 0f;
				vertAxis = 0f;
				break;
		
			default:
				horiAxes = 1f;
				vertAxis = 0f;
				break;
		}

		//Initialise the list of Advanced Behaviors according to the ones chosen in inspector.
		if(abList.Length > 0) {
			for(int i = 0; i < abList.Length; i++) {
				if(abList[i]) {
					advancedBehaviors.Add(new AB_RadiusBased(this, movementSpeed));
					
					float radius;
					EnemyRadius enemyRadius = transform.GetChild(0).GetChild(1).GetComponent<EnemyRadius>();
					if(enemyRadius == null)
						Debug.Log("Error getting enemy's radius.");
					else {
						radius = enemyRadius.radius * 2;
						
						if(i == 0) {
							(advancedBehaviors[advancedBehaviors.Count - 1] as AB_RadiusBased).SetValues(0, radius, false);
							//stickToWalls = true;
						}
						else if(i == 1)
							(advancedBehaviors[advancedBehaviors.Count - 1] as AB_RadiusBased).SetValues(1, radius, false);
						else if(i == 2)
							(advancedBehaviors[advancedBehaviors.Count - 1] as AB_RadiusBased).SetValues(2, radius, false);
						else if(i == 3)
							(advancedBehaviors[advancedBehaviors.Count - 1] as AB_RadiusBased).SetValues(3, radius, false);
					}
				}
			}
		}
	}

	private void FixedUpdate() {
        //Movement(horiAxes, movementSpeed);
        if (!LevelManager.Instance.IsPaused && !LevelManager.Instance.isReloading) {
			if(!isInit) {
				if(enemiesRender.isVisible) {
					Vector3 scale = transform.localScale;
					if(scale.x == -1) {
						transform.localScale = new Vector3(scale.y, 1, scale.z);
						Flip();
						SwitchDirection();
					}
					isInit = true;
				}
			}

			if(isInit) {
				if(basicBehavior == BasicBehavior.FollowReturn)
					FollowReturn();
				else {
					if(basicBehavior != BasicBehavior.Motionless) {
						if(lookAtHero && !_player.IsInvisible) {
							float heroXPosition = _player.transform.position.x - transform.position.x;
							if(horiAxes > 0f && heroXPosition < 0f) {
								if(heroXPosition > -0.25f)
									movementSpeed = 0f;
								else {
									movementSpeed = 50.0f;
									Flip();
									SwitchDirection();
								}
							}
							else if(horiAxes < 0f && heroXPosition > 0f) {
								if(heroXPosition < 0.25f)
									movementSpeed = 0f;
								else {
									movementSpeed = 50.0f;
									Flip();
									SwitchDirection();
								}
							}
						}

						if(!_player.IsInvisible) {
							for(int i = 0, j = 0; i < abList.Length; i++) {
								if(abList[i] == true) {
									advancedBehaviors[j++].FixUpdate(_objPlayer, i, ref dashState);
								}
							}
							if(_gaz != null)
								_gaz.FixUpdate(_objPlayer, -1, ref dashState);
						}

						if(basicBehavior == BasicBehavior.LeftRight || basicBehavior == BasicBehavior.WaveLR) {
							if(isFlying) {
								if(_isWalled && !ignoreWalls || transform.position.x - origin.x > maxDistance || transform.position.x < origin.x) {
									if(!stickToWalls) {
										SwitchDirection();
										Flip();
										movementSpeed = 50;
									}
									else
										movementSpeed = 0;
								}
							}
							else {
								if(!_isGrounded || (_isWalled && !ignoreWalls)) {
									if(!ignoreHoles && !stickToWalls) {
										SwitchDirection();
										Flip();
										movementSpeed = 50;
									}
									else
										movementSpeed = 0;
								}
							}
						}
						//If ennemy uses Wave behavior, affect Y axis.
						if(basicBehavior == BasicBehavior.WaveLR && !overrideMove) {
							if(Mathf.Abs(vertAxis + angleSpeed * Time.fixedDeltaTime) > maxAngle)
								angleSpeed *= -1;

							vertAxis += angleSpeed * Time.fixedDeltaTime;
						}

						if(basicBehavior == BasicBehavior.TopDown) {
							RaycastHit2D rHit2D;
							rHit2D = Physics2D.Raycast(transform.position, Vector2.up * vertAxis, 0.55f, 1 << LayerMask.NameToLayer("Ground"));
							if(rHit2D.collider != null || transform.position.y > 59.5 || transform.position.y < 0.5)
								vertAxis *= -1;
						}

						if(isFlying) {
                            if (overrideMove)
                                rg.velocity = Vector2.zero;
                            else
                            {
                                rg.velocity = new Vector2(horiAxes * movementSpeed * escapeVelModifier * Time.fixedDeltaTime, vertAxis * movementSpeed * Time.fixedDeltaTime);
                            }
                        }
						else if(!overrideMove)
							Movement(horiAxes * movementSpeed * escapeVelModifier * Time.fixedDeltaTime, rg.velocity.y);
					}
					enemyAnimator.SetFloat("XSpeed", movementSpeed * escapeVelModifier);
				}
			}
        }
    }

    public void OverrideMoveFunction(Vector2 tDirection) {
        Movement(tDirection.x, rg.velocity.y);
    }

    public void OverrideMoveFunction(Vector2 tDirection, float tForce) {
		rg.AddForce(tDirection * tForce, ForceMode2D.Impulse);
    }
    
	public void IsWalled(bool iswall) {
		_isWalled = iswall;
	}

	public void IsGrounded(bool tof) {
		_isGrounded = tof;
		if(name.Contains("Cool") || name.Contains("Cat")) {
			Vector3 rDir = transform.position;
			if(horiAxes > 0)
				rDir.x += 0.51f;
			else
				rDir.x -= 0.51f;
			
			RaycastHit2D rHit2D = Physics2D.Raycast(rDir, Vector2.down, 2f, 1 << LayerMask.NameToLayer("Ground"));
			if(rHit2D.collider != null && rHit2D.collider.name.Contains("Ramp")) {
				_isGrounded = true;
			}
		}
		
		if (movementSpeed == 0f)
			movementSpeed = 50f;
	}

	public void SwitchDirection() {
		horiAxes = -horiAxes;
	}

	public void SetDashState() {
		dashState = DashState.returning;
	}

	private void FollowReturn() {
		Vector2 direction = _player.transform.position - transform.position;
		float radius = transform.GetChild(0).GetChild(1).GetComponent<EnemyRadius>().radius * 2;
		
		if(horiAxes > 0f && direction.x < 0f) {
			Flip();
			SwitchDirection();
		}
		else if(horiAxes < 0f && direction.x > 0f) {
			Flip();
			SwitchDirection();
		}

		if(direction.sqrMagnitude < radius * radius) {
			direction = direction.normalized;
			Movement(direction.x * movementSpeed * Time.fixedDeltaTime, direction.y * movementSpeed * Time.fixedDeltaTime);

			return;
		}

		direction = origin - (Vector2)transform.position;
		if(direction.sqrMagnitude > 0.005) {
			direction = direction.normalized;
			Movement(direction.x * movementSpeed * Time.fixedDeltaTime, direction.y * movementSpeed * Time.fixedDeltaTime);
		}
		else
			Movement(0f, 0f);
	}
}