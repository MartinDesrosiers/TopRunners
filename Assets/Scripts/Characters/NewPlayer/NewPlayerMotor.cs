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

	private List<Collider2D> _grounds;
	private List<Collider2D> _walls;
	private float _destructionDelay = 1f;

	protected Rigidbody2D _rigidBody;
	protected PlayerStateMachine _stateMachine;
	protected bool _isGrounded;
	protected bool _isWalled;

	protected virtual void CustomStart() {
		_rigidBody = GetComponent<Rigidbody2D>();
		_stateMachine = GetComponent<PlayerStateMachine>();
		_grounds = new List<Collider2D>();
		_walls = new List<Collider2D>();
	}

	private void FixedUpdate() {
		if(!LevelManager.Instance.IsPaused) {
			if(_grounds.Count > 0) {
				for(int i = 0; i < _grounds.Count; i++) {
					if(!_grounds[i].gameObject.activeInHierarchy)
						_grounds.RemoveAt(0);
				}

				if(_grounds.Count == 0)
					_isGrounded = false;
			}

			if(_walls.Count > 0) {
				for(int i = 0; i < _walls.Count; i++) {
					if(!_walls[i].gameObject.activeInHierarchy) {
						_walls.RemoveAt(i);
					}
				}

				if(_walls.Count == 0)
					_isWalled = false;
			}
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

				if(col.gameObject.name.Contains("Destructible") && transform.tag != "Enemies")
					SetDestroyTimer(col.gameObject);

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

					if(col.gameObject.name.Contains("Destructible") && transform.name != "Enemies")
						SetDestroyTimer(col.gameObject);

					_walls.Add(col);
				}
			}
		}
	}

	public void TriggerExit2D(Collider2D col, string colliderType) {
		if(col.gameObject.layer == 8) {
			if(colliderType == "Ground") {
				ExitCollider(ref _grounds, ref _isGrounded);

				if(col.gameObject.name == "platform")
					transform.SetParent(null);
			}
			else if(colliderType == "Wall") {
				ExitCollider(ref _walls, ref _isWalled);
			}
		}
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

	private void ExitCollider(ref List<Collider2D> colliders, ref bool collisionState) {
		if(colliders.Count > 0)
			colliders.RemoveAt(0);

		if(colliders.Count == 0) {
			if(transform.tag == "Player")
				collisionState = false;
		}
	}

	private void SetDestroyTimer(GameObject obj) {
		if(!obj.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(obj.gameObject.GetComponent<Animator>().GetLayerIndex("Base Layer")).IsName("destroy-breaking") ||
			!obj.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(obj.gameObject.GetComponent<Animator>().GetLayerIndex("Base Layer")).IsName("glass-anim")) {
			obj.gameObject.GetComponent<Animator>().SetBool("Destroy", true);
			StartCoroutine(obj.GetComponent<SetObjActive>().SetObjectActive(obj.gameObject.GetComponent<Animator>().GetCurrentAnimatorStateInfo(obj.gameObject.GetComponent<Animator>().GetLayerIndex("Base Layer")).length + 1.0f));
		}
	}
}