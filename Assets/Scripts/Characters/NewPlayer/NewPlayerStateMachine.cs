using UnityEngine;

public enum PlayerStates { Idle, Walk, Run, Jump, Fall, Slide, Dash, Walled, Dead }

[RequireComponent(typeof(NewPlayerController))]
[RequireComponent(typeof(NewPlayerInputs))]
public class NewPlayerStateMachine : StateMachine {
	private bool _sprint = false;
	private bool Sprint {
		get { return _sprint; }
		set {
			if(_sprint != value) {
				_controller.animator.speed = value ? 1.5f : 1.0f;
				if(!value)
					StartCoroutine(_controller.RegenStaminaTimer());
			}
			_sprint = value;
		}
	}
	private NewPlayerController _controller;
	private NewPlayerInputs _inputs;
	private bool IsDirectionLocked { get { return (lastState.CompareTo(PlayerStates.Walled) == 0 && Time.time - timeEnteredState < _controller.wallJumpDelay) || CurrentState.CompareTo(PlayerStates.Slide) == 0; } }
	private float MovementSpeed { get { return _sprint ? _controller.SprintingSpeed : _controller.RunningSpeed; } }

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

		UpdateSprint();
	}

	//Update loop called after the sate machine's main update loop.
	protected override void LateCustomUpdate() {
		if(_sprint)
			_controller.UseStamina();
		else
			_controller.RegenStamina();
	}

	//Resets all NewPlayerController variables to default value.
	private void SetToDefault() {
		_controller.SetToDefault();
		CurrentState = PlayerStates.Idle;
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
			if(_inputs.Direction != 0)
				CurrentState = PlayerStates.Run;
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
		if(_controller.GetWall())
			CurrentState = PlayerStates.Idle;
		else if(!_controller.GetGround())
			CurrentState = PlayerStates.Fall;
		else if(_inputs.Jump)
			CurrentState = PlayerStates.Jump;
		else if(_inputs.Slide)
			CurrentState = PlayerStates.Slide;
		else if(_inputs.Direction == 0) {
			CurrentState = PlayerStates.Idle;
		}
		else
			_controller.Move((Vector2)transform.right * MovementSpeed * _inputs.Direction);
	}

	//If the player state is not Run, Jump, Fall or Slide, stop sprinting even if the player is still using the sprint input.
	private void UpdateSprint() {
		if(_inputs.Sprint) {
			int enumIndex = CurrentState.CompareTo(PlayerStates.Run);

			if(enumIndex >= 0 && enumIndex <= 3 && _controller.CurrentStamina > 0)
				Sprint = true;
			else
				Sprint = false;
		}
		else
			Sprint = false;
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
		else if(_inputs.Direction != 0 && !_controller.GetWall())
			CurrentState = PlayerStates.Run;
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
			CurrentState = PlayerStates.Idle;
		}
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
		Vector2 velocity = _controller.Velocity;
		float magnitude = velocity.magnitude;
		if(magnitude != 10 && velocity != Vector2.zero) {
			//Debug.Log(velocity.magnitude + " , " + velocity * (1f / (magnitude / 10f)) + " , " + (velocity * (1f / (magnitude / 10f))).magnitude);
			_controller.SetVelocity(velocity * (1f / (velocity.magnitude / 10f)));
		}

		Debug.Log(_controller.Velocity.magnitude);

		RunUpdate();
	}

	private void Run_ExitState() {
		UpdateSprint();
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
		else if(_inputs.Jump) {
			CurrentState = PlayerStates.Dash;
			return;
		}

		if(_controller.Velocity.y <= 0)
			CurrentState = PlayerStates.Fall;

		if(IsDirectionLocked)
			_controller.Move(new Vector2(_controller.Direction * MovementSpeed, _controller.Velocity.y));
		else
			_controller.Move(new Vector2(_inputs.Direction * MovementSpeed, _controller.Velocity.y));
	}

	private void Jump_ExitState() {
		UpdateSprint();
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

		_controller.Move(new Vector2(_inputs.Direction * MovementSpeed, _controller.Velocity.y));
	}

	private void Fall_ExitState() {
		UpdateSprint();
	}
	#endregion

	//Player character is sliding.
	#region Slide
	private void Slide_EnterState() {
		_controller.ToggleSlidingColliders(true);

		_controller.animator.Play("slide");
	}

	private void Slide_CustomUpdate() {
		RaycastHit2D hit;
		hit = Physics2D.Raycast(new Vector2(transform.position.x, transform.position.y - transform.localScale.y / 2), Vector2.up, 1.5f, 1 << LayerMask.NameToLayer("Ground"));
		if(Time.time - timeEnteredState > _controller.slideDelay && hit.collider == null)
			ExitCurrentState();

		_controller.Move(new Vector2(_controller.Direction * MovementSpeed, _controller.Velocity.y));
	}

	private void Slide_ExitState() {
		UpdateSprint();
		_controller.ToggleSlidingColliders(false);
	}
	#endregion

	//Player character is dashing.
	#region Dash
	private void Dash_EnterState() {
		_controller.ToggleGravity(false);
		_controller.CancelVelocity(false, true);
		_controller.animator.Play("dash");
		_controller.StartDash(_sprint);
	}

	private void Dash_CustomUpdate() {
		if(_controller.LerpDash())
			ExitCurrentState();
	}

	private void Dash_ExitState() {
		_controller.ResetDash();
		_controller.ToggleGravity(true);
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
		SetToDefault();
	}
	#endregion
}