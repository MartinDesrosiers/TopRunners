using UnityEngine;
using System.Collections.Generic;

public enum PlayerStates { Idle, Walk, Run, Sprint, Jump, Fall, Slide, Dash, Walled, Dead }

[RequireComponent(typeof(NewPlayerController))]
[RequireComponent(typeof(NewPlayerInputs))]
public class PlayerStateMachine : StateMachine {
	private NewPlayerController _controller;
	private NewPlayerInputs _inputs;
	private bool IsDirectionLocked { get { return lastState.CompareTo(PlayerStates.Walled) == 0 && Time.time - timeEnteredState < _controller.wallJumpDelay; } }
	private bool _wasWalled = false;

	private void Start() {
		_controller = GetComponent<NewPlayerController>();
		_inputs = GetComponent<NewPlayerInputs>();

		SetToDefault();
	}

	private void Update() {
		if(GameManager.Instance.currentState == GameManager.GameState.RunTime)
			CustomUpdate();
		else
			CurrentState = PlayerStates.Idle;
	}
	
	//Update loop called before the sate machine's main update loop.
	protected override void EarlyCustomUpdate() {
		if(_inputs.Direction != 0 && CheckDirection() && !IsDirectionLocked)
			_controller.FlipHorizontal();
	}
	
	//Update loop called after the sate machine's main update loop.
	protected override void LateCustomUpdate() {
		base.LateCustomUpdate();
	}
	
	//Resets all NewPlayerController variables to default value.
	private void SetToDefault() {
		CurrentState = PlayerStates.Idle;
		_controller.SetToDefault();
	}
	
	//Returns true if the player model is facing opposite way of it's direction.
	private bool CheckDirection() {
		if(_inputs.Direction < 0 && _controller.transform.localScale.x > 0)
			return true;
		
		if(_inputs.Direction > 0 && _controller.transform.localScale.x < 0)
			return true;

		return false;
	}

	private void ExitCurrentState() {
		if(_controller.GetGround()) {
			if(_inputs.Direction != 0) {
				if(!_inputs.Sprint)
					CurrentState = PlayerStates.Run;
				else
					CurrentState = PlayerStates.Sprint;
			}
			else
				CurrentState = PlayerStates.Idle;
		}
		else {
			if(_controller.GetWall())
				CurrentState = PlayerStates.Walled;
			else if(_controller.Velocity.y > 0)
				CurrentState = PlayerStates.Jump;
			else
				CurrentState = PlayerStates.Fall;
		}
	}

	private void RunUpdate() {
		if(!_controller.GetGround())
			CurrentState = PlayerStates.Fall;
		else if(_inputs.Jump)
			CurrentState = PlayerStates.Jump;
		else if(_inputs.Slide)
			CurrentState = PlayerStates.Slide;
		else if(_inputs.Direction == 0) {
			//if(_controller.Velocity.x == 0)			Decceleration
			CurrentState = PlayerStates.Idle;
		}
		else if(CurrentState.CompareTo(PlayerStates.Run) == 0 && _inputs.Sprint || CurrentState.CompareTo(PlayerStates.Sprint) == 0 && !_inputs.Sprint)
			CurrentState = CurrentState.CompareTo(PlayerStates.Run) == 0 ? PlayerStates.Sprint : PlayerStates.Run;
		else if(!_controller.GetWall())
			_controller.Move(new Vector2(_inputs.Direction * (CurrentState.CompareTo(PlayerStates.Run) == 0 ? _controller.RunningSpeed : _controller.SprintingSpeed), _controller.Velocity.y));
	}

	//All player states.

	//Player character is not moving.
	#region Idle
	private void Idle_EnterState() {
		_controller.animator.Play("idle");
		_controller.Move(new Vector2(0, 0));
	}

	private void Idle_CustomUpdate() {
		if(!_controller.GetGround())
			CurrentState = PlayerStates.Fall;
		else if(_inputs.Jump)
			CurrentState = PlayerStates.Jump;
		else if(_inputs.Direction != 0) {
			if(!_inputs.Sprint)
				CurrentState = PlayerStates.Run;
			else
				CurrentState = PlayerStates.Sprint;
		}
	}
	#endregion

	//Player character is walking.
	#region Walk
	private void Walk_EnterState() {
		_controller.animator.Play("walk");
	}

	private void Walk_CustomUpdate() {
		if(!_controller.GetGround())
			CurrentState = PlayerStates.Fall;
		else if(_inputs.Direction == 0) {
			//if(_controller.Velocity.x == 0)			Decceleration
			CurrentState = PlayerStates.Idle;
		}
		else if(_inputs.Sprint)
			CurrentState = PlayerStates.Run;
		else if(!_controller.GetWall())
			_controller.Move(new Vector2(_inputs.Direction * _controller.WalkingSpeed, _controller.Velocity.y));
	}
	#endregion

	//Player character is running.
	#region Run
	private void Run_EnterState() {
		_controller.animator.Play("run");
	}

	private void Run_CustomUpdate() {
		RunUpdate();
	}
	#endregion

	//Player character is sprinting.
	#region Sprint
	private void Sprint_EnterState() {
		_controller.animator.Play("run");
		_controller.animator.speed = 1.5f;
	}

	private void Sprint_CustomUpdate() {
		RunUpdate();
	}

	private void Sprint_ExitState() {
		_controller.animator.speed = 1f;
	}
	#endregion

	//Player character is jumping ( only while going upwards before reaching the jump's peak height ).
	#region Jump
	private void Jump_EnterState() {
		_controller.ToggleJumpingColliders(true);

		float movementSpeed = _controller.Velocity.x;
		if(lastState.CompareTo(PlayerStates.Walled) == 0) {
			_controller.FlipHorizontal();
			movementSpeed = _controller.RunningSpeed;
		}

		_controller.Move(new Vector2(_controller.Direction * movementSpeed, _controller.Jump()));
		_controller.animator.Play("jump");
	}

	private void Jump_CustomUpdate() {
		if(_controller.GetWall()) {
			CurrentState = PlayerStates.Walled;
			return;
		}
		else if(_inputs.Slide) {
			CurrentState = PlayerStates.Slide;
			return;
		}
		else if(_inputs.Jump && EnemyList.IsDashValid(_controller.transform.position, _controller.Direction)) {
			CurrentState = PlayerStates.Dash;
			return;
		}

		if(_controller.Velocity.y <= 0)
			CurrentState = PlayerStates.Fall;

		if(IsDirectionLocked)
			_controller.Move(new Vector2(_controller.Direction * (_inputs.Sprint ? _controller.SprintingSpeed : _controller.RunningSpeed), _controller.Velocity.y));
		else
			_controller.Move(new Vector2(_inputs.Direction * (_inputs.Sprint ? _controller.SprintingSpeed : _controller.RunningSpeed), _controller.Velocity.y));
	}

	private void Jump_ExitState() {
		_controller.ToggleJumpingColliders(false);
	}
	#endregion

	//Player character is falling ( after losing ground or after reaching the peak of a jump ).
	#region Fall
	private void Fall_EnterState() {
		_controller.animator.Play("fall");
	}

	private void Fall_CustomUpdate() {
		if(_controller.GetGround()) {
			ExitCurrentState();
			return;
		}
		else if(_controller.GetWall()) {
			CurrentState = PlayerStates.Walled;
			return;
		}
		else if(_inputs.Slide) {
			CurrentState = PlayerStates.Slide;
			return;
		}
		else if(_inputs.Jump && EnemyList.IsDashValid(_controller.transform.position, _controller.Direction)) {
			CurrentState = PlayerStates.Dash;
			return;
		}

		_controller.Move(new Vector2(_inputs.Direction * (_inputs.Sprint ? _controller.SprintingSpeed : _controller.RunningSpeed), _controller.Velocity.y));
	}
	#endregion

	//Player character is sliding.
	#region Slide
	private void Slide_EnterState() {
		_controller.ToggleSlidingColliders(true);

		_controller.animator.Play("slide");
	}

	private void Slide_CustomUpdate() {
		//if(_controller.GetWall()) {
		//	if(_wasWalled) {
		//		CurrentState = PlayerStates.Walled;
		//		_wasWalled = false;
		//		return;
		//	}
		//	else
		//		_wasWalled = true;
		//}
		//else
		//	_wasWalled = false;

		RaycastHit2D hit;
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2), Vector2.up, 1.5f, 1 << LayerMask.NameToLayer("Ground"));
		if(Time.time - timeEnteredState > _controller.slideDelay && hit.collider == null)
			ExitCurrentState();

		_controller.Move(new Vector2(_controller.Direction * (_inputs.Sprint ? _controller.SprintingSpeed : _controller.RunningSpeed), _controller.Velocity.y));
	}

	private void Slide_ExitState() {
		_controller.ToggleSlidingColliders(false);
	}
	#endregion

	//Player character is dashing.
	#region Dash
	private void Dash_EnterState() {
		_controller.animator.Play("dash");
		_controller.StartDash(_inputs.Sprint);
	}

	private void Dash_CustomUpdate() {
		_controller.LerpDash();
	}

	private void Dash_ExitState() {
		_controller.ResetDash();
	}
	#endregion

	//Player character is touching a wall and is not grounded.
	#region Wall
	private void Walled_EnterState() {
		_controller.Move(new Vector2(0, _controller.Velocity.y));
		_controller.animator.Play("wall");
	}

	private void Walled_CustomUpdate() {
		if(_controller.GetGround())
			ExitCurrentState();
		else if(!_controller.GetWall())
			CurrentState = PlayerStates.Fall;
		else if(_inputs.Jump)
			CurrentState = PlayerStates.Jump;
	}
	#endregion

	//Player character is dead.
	#region Dead
	private void Dead_EnterState() {
		//_controller.animator.Play("dead");
	}
	#endregion
}