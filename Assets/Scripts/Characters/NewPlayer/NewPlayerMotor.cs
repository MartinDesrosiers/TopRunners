using UnityEngine;
using System;
using System.Collections.Generic;

public class NewPlayerMotor : MonoBehaviour {
	//Returns the player's rigidbody velocity.
	public Vector2 Velocity { get { return _rigidBody.velocity; } }
	//Returns the player state machine's current state.
	public Enum CurrentState { get { return _stateMachine.CurrentState; } }
	//Returns the player model's orientation.
	public float Direction { get { return transform.localScale.x; } }

	private float _destructionDelay = 1f;

	protected List<Collider2D> _grounds;
	protected List<Collider2D> _walls;
	protected Rigidbody2D _rigidBody;
	protected NewPlayerStateMachine _stateMachine;
	protected bool _isGrounded;
	protected bool _isWalled;

	protected virtual void CustomStart() {
		_rigidBody = GetComponent<Rigidbody2D>();
		_stateMachine = GetComponent<NewPlayerStateMachine>();
		_grounds = new List<Collider2D>();
		_walls = new List<Collider2D>();
	}

	private void FixedUpdate() {
		if(!LevelManager.Instance.IsPaused) {
			if(_grounds.Count > 0) {
				int subIndex = 0;
				for(int i = 0; i < _grounds.Count; i++) {
					if(_grounds[i] == null || !_grounds[i].gameObject.activeInHierarchy) {
						_grounds.RemoveAt(i - subIndex);
						subIndex++;
					}
				}

				if(_grounds.Count == 0)
					_isGrounded = false;
			}

			if(_walls.Count > 0) {
				int subIndex = 0;
				for(int i = 0; i < _walls.Count; i++) {
					if(_walls[i] == null || !_walls[i].gameObject.activeInHierarchy) {
						_walls.RemoveAt(i - subIndex);
						subIndex++;
					}
				}

				if(_walls.Count == 0)
					_isWalled = false;
			}

			if(_isGrounded) {
				RaycastHit2D hit = Physics2D.Raycast(transform.position, -transform.up, 2f, 1 << LayerMask.NameToLayer("Ground"));
				if(hit.collider != null) {
					transform.rotation = Quaternion.FromToRotation(transform.up, hit.normal) * transform.rotation;
					Debug.DrawRay(transform.position, -transform.up * 2f, Color.red, 0.5f);
					Debug.Log(Quaternion.FromToRotation(transform.up, hit.normal) + " , " + transform.rotation + " , " + hit.normal.ToString("F4"));
				}
				//RaycastHit2D[] hit2 = Physics2D.BoxCastAll(transform.position - transform.up, new Vector2(1f, 0.2f), 0f, -transform.up, 0.1f, 1 << LayerMask.NameToLayer("Ground"));
				//if(hit2.Length > 0) {
				//	float magnitude = 0;
				//	Quaternion rotation = transform.rotation;
				//	RaycastHit2D collision = hit2[0];
				//	foreach(RaycastHit2D rHit in hit2) {
				//		float tMagnitude = ((Vector2)transform.position - rHit.point).magnitude;
				//		Quaternion tRotation = Quaternion.FromToRotation(transform.up, collision.normal) * transform.rotation;
				//		float rotationDifference = Mathf.Abs(tRotation.eulerAngles.z - transform.rotation.eulerAngles.z);
				//
				//		Debug.Log(tMagnitude + " , " + transform.rotation.eulerAngles + " , " + tRotation.eulerAngles + " , " + rotationDifference);
				//
				//		if(tMagnitude < magnitude && rotationDifference < 45f) {
				//			collision = rHit;
				//			rotation = tRotation;
				//		}
				//	}
				//	
				//	transform.rotation = rotation;
				//	Debug.DrawRay(transform.position, -transform.up * 2f, Color.red, 0.5f);
				//	//Debug.Log(Quaternion.FromToRotation(transform.up, collision.normal) + " , " + transform.rotation + " , " + collision.normal.ToString("F4"));
				//}
			}
			else
				transform.rotation = Quaternion.identity;
		}
	}

	public void TriggerEnter2D(Collider2D col, string colliderType) {
		if(col.gameObject.layer == 8) {
			if(colliderType == "Ground") {
				if(_grounds.Count == 0) {
					if(transform.tag == "Player" && _stateMachine.CurrentState.CompareTo(PlayerStates.Dash) != 0) {
						if(_rigidBody.velocity.y < .5f)
							_isGrounded = true;
						if(col.gameObject.name.Contains("platform"))
							transform.SetParent(col.gameObject.transform);
					}
				}

				_grounds.Add(col);
			}
			else if(colliderType == "Wall") {
				if(col.tag != "Crossable" && col.tag != "Ramp") {
					if(_walls.Count == 0 && col.tag != "Trap") {
						if(transform.tag == "Player") {
							if(col.gameObject.name.Contains("InnerShadow") && col.gameObject.GetComponent<BoxCollider2D>().isTrigger)
								return;
							else if(col.gameObject.name.Contains("PipeCol") && _stateMachine.CurrentState.CompareTo(PlayerStates.Slide) == 0)
								return;
							else if(!col.gameObject.name.Contains("CrossableGround")) {
								if(_stateMachine.CurrentState.CompareTo(PlayerStates.Dash) == 0)
									transform.parent.GetComponentInParent<NewPlayerController>().ResetDash();

								_isWalled = true;
							}
						}
					}

					_walls.Add(col);
				}
			}
		}
	}

	public void TriggerExit2D(Collider2D col, string colliderType) {
		if(col.gameObject.layer == 8) {
			if(colliderType == "Ground") {
				ExitCollider(ref _grounds, ref _isGrounded, col);

				if(col.gameObject.name == "platform")
					transform.SetParent(null);
			}
			else if(colliderType == "Wall") {
				ExitCollider(ref _walls, ref _isWalled, col);
			}
		}
	}

	public void CancelVelocity(bool x, bool y) {
		_rigidBody.velocity = new Vector2(x ? 0 : _rigidBody.velocity.x, y ? 0 : _rigidBody.velocity.y);
	}

	public bool GetGround() {
		return _isGrounded;
	}

	public bool GetWall() {
		return _isWalled;
	}

	public void Move(Vector2 force) {
		_rigidBody.velocity = force;
	}

	public void FlipHorizontal() {
		Vector3 tScale = transform.localScale;
		tScale.x = -tScale.x;
		transform.localScale = tScale;
	}

	private void ExitCollider(ref List<Collider2D> colliders, ref bool collisionState, Collider2D col) {
		colliders.Remove(col);

		if(colliders.Count == 0)
			collisionState = false;
	}
}