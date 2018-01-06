using UnityEngine;
using System.Collections;

class AB_RadiusBased : AB_Interface {
    private EnemyAI parentAI;
    Vector2 direction;

    private float baseMovementSpeed = 0.0f;
    private bool followY = false;

    private bool isAttackActive;
    private float fleeRadius = 0.0f;
    private float attackRadius = 0.0f;
    private float dashRadius = 0.0f;
    private float followRadius = 0.0f;
    float sqrMagnitude;


    private bool isJumpAttack = false;
    private float postAttackVelModifier = 6.0f;

    private Vector2 dashOrigin = Vector2.zero;
    private Vector2 dashTarget = Vector2.zero;
    private float dashSpeed = 0.5f;
    private float dashReturnSpeed = 1.0f;
    private float dashLerpPosition = 0.0f;
    //Constructor
    public AB_RadiusBased(EnemyAI tParentAI, float tSpeed) {
		parentAI = tParentAI;
		baseMovementSpeed = tSpeed;
	}

	//Used to set basic values ( needs to be changed / Optimised ).
	public void SetValues(uint index, float tRadius, bool tBool) {
		switch(index) {
			case 0:
				fleeRadius = tRadius;
			break;

			case 1:
				isAttackActive = true;
				attackRadius = tRadius / 2;
			break;

			case 2:
				dashRadius = tRadius / 2;
			break;

			case 3:
				followRadius = tRadius;
				followY = tBool;
			break;

			default:
				Debug.Log("Error, advanced behavior index is invalid.");
			break;
		}
	}

    public void FixUpdate(GameObject player, int behaveNumber, ref DashState dashState) {
        //Get hero's relative position from parentAI and the vector's magnitude.
        direction = player.transform.position - parentAI.transform.position;
        sqrMagnitude = direction.sqrMagnitude;
        //If Avoid behavior is used and within radius.
            switch (behaveNumber) {
                case 0:
                    if (sqrMagnitude < (fleeRadius * fleeRadius)) {
                        //Work as intended if attack behavior isn't used or if out of radius.
                        if ((!isAttackActive || sqrMagnitude > (attackRadius * attackRadius))) {
                            parentAI.SetEscapeModifier = 2f;
                            if (player.transform.position.x < parentAI.transform.position.x && parentAI.transform.localScale.x < 0) {
                                parentAI.Flip();
                                parentAI.SwitchDirection();
                            }
                            else if (player.transform.position.x > parentAI.transform.position.x && parentAI.transform.localScale.x > 0) {
                                parentAI.Flip();
                                parentAI.SwitchDirection();
                            }
                            parentAI.stickToWalls = true;
                        }
                        //Escape quickly after attacking.
                        else if (isJumpAttack && parentAI.GetIsGrounded) {
                            parentAI.movementSpeed = ((baseMovementSpeed > 0.0f) ? baseMovementSpeed : 1.0f) * postAttackVelModifier;
                        }
                        //Returns to base movement speed when out of radius.
                        else if (!isJumpAttack) {
                            parentAI.movementSpeed = baseMovementSpeed;
                        }
                    }
                    else {
                        parentAI.SetEscapeModifier = 0f;
                        parentAI.stickToWalls = false;
                    }
                    break;

                //If Attack behavior is used and within radius.
                case 1:
                        if (sqrMagnitude < (attackRadius * attackRadius)) {
                            if (!isJumpAttack) {
                                isJumpAttack = true;
                                parentAI.overrideMove = true;
                                parentAI.OverrideMoveFunction((direction.normalized + Vector2.up) / 2, 10.0f);

                                parentAI.StartCoroutine(JumpAttackDelay());
                            }
                        }
                    
                    break;
                //If Dash behavior is used and within radius.
                case 2:
                        if (sqrMagnitude < (dashRadius * dashRadius) && dashState == DashState.passive)
                        {
                            dashState = DashState.attacking;

                            dashOrigin = parentAI.transform.position;
                            dashTarget = LevelManager.Instance.player.transform.position;
                        }
                        else if (dashState == DashState.attacking)
                        {
                            parentAI.overrideMove = true;
                            parentAI.transform.position = Vector2.Lerp(dashOrigin, dashTarget, dashLerpPosition);

                            dashLerpPosition += Time.fixedDeltaTime / dashSpeed;
                            if (dashLerpPosition >= 1.0f)
                            {
                                //parentAI.Flip();
                                dashState = DashState.returning;
                                dashLerpPosition = 1.0f;
                            }
                        }
                        else if (dashState == DashState.returning)
                        {
                            parentAI.transform.position = Vector2.Lerp(dashOrigin, dashTarget, dashLerpPosition);
                            dashLerpPosition -= Time.fixedDeltaTime / dashReturnSpeed;
                            if (dashLerpPosition <= 0.0f)
                            {
                                dashState = DashState.passive;
                                dashLerpPosition = 0.0f;
                                parentAI.overrideMove = false;
                            }
                        }
                    
                    break;
                //If Follow behavior is used and within radius.
                case 3:
                        if (sqrMagnitude < (followRadius * followRadius) && Mathf.Abs(direction.y) < 3.5f)
                        {
                            parentAI.stickToWalls = true;
                            parentAI.SetEscapeModifier = 3f;
                            if (followY) {
                                parentAI.overrideMove = true;
                                //parentAI.OverrideMoveFunction(direction.normalized);
                            }

                            if (player.transform.position.x < parentAI.transform.position.x && parentAI.transform.localScale.x > 0 ||
                                player.transform.position.x > parentAI.transform.position.x && parentAI.transform.localScale.x < 0)
                            {
                                parentAI.Flip();
                                parentAI.SwitchDirection();
                            }

                        }
                        else
                        {
                            parentAI.stickToWalls = false;
                            parentAI.SetEscapeModifier = 1f;
                            parentAI.movementSpeed = baseMovementSpeed;
                        }
                    
                    break;
                default:
                    break;
            
        }
    }

    private IEnumerator JumpAttackDelay() {
		while(true) {
			yield return new WaitForSeconds(1.5f);

			if(parentAI.GetIsGrounded) {
				isJumpAttack = false;
				parentAI.overrideMove = false;
				break;
			}
		}
	}
}