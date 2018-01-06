using UnityEngine;public static class GlobalInputs {	private static float _minSwipeDistance = 20f;	private static float _fullSwipeDistance = 75f;	private static Vector2 _startPos = Vector2.zero;	private static Vector2 _lockedDirection = Vector2.zero;	private static bool _isSwipping = false;	private static float _tapTimer = 0f;	private static bool _tapTriggered = false;	public static Vector2 GetDirection() {		return _lockedDirection;	}	public static void ClearInputs() {		if(Input.touchCount == 1) {			if(Input.GetTouch(0).phase == TouchPhase.Ended) {				_isSwipping = false;				_lockedDirection = Vector2.zero;				_tapTriggered = false;
				_tapTimer = 0f;			}		}	}	public static bool GetRotate() {
		if(Input.touchCount == 1)
			if(Input.GetTouch(0).phase == TouchPhase.Began)
				return true;

		return false;
	}	public static bool GetTap() {		if(Input.touchCount == 1) {			Touch touch = Input.GetTouch(0);			switch(touch.phase) {
				case TouchPhase.Moved:
				case TouchPhase.Stationary:
				if(!_isSwipping && !_tapTriggered) {
					_tapTimer += 1 / Time.deltaTime;
					if(_tapTimer > 400) {
						_tapTriggered = true;
						return true;
					}
					else
						return false;
				}
				else
					return false;

				case TouchPhase.Ended:
				if(_isSwipping || _tapTriggered)
					return false;
				else
					return true;

				default:
				return false;
			}		}

		return false;	}	public static bool GetSwipe(Vector2 direction, bool isLocked) {		if(GetSwipeMotion(TouchPhase.Moved, direction, 1)) {			if(isLocked) {				if(_lockedDirection == Vector2.zero)					_lockedDirection = direction;				else if(direction != _lockedDirection)					return false;			}			_isSwipping = true;			return true;		}		return false;	}	public static bool GetSwipeFull(Vector2 direction) {		if(GetSwipeMotion(TouchPhase.Ended, direction, 1)) {			_isSwipping = true;			return true;		}		return false;	}


	public static bool GetPinchIn() {		if(GetPinchMotion() == 1)			return true;		return false;	}		public static bool GetPinchOut() {		if(GetPinchMotion() == -1)			return true;		return false;	}		//ToDo :	//Use velocity instead of end phase.	private static bool GetSwipeMotion(TouchPhase swipeType, Vector2 direction, int touchNumber) {		if(Input.touchCount == touchNumber) {			Touch touch = Input.GetTouch(0);			if(touch.phase == TouchPhase.Began) {				_startPos = touch.position;				return false;			}			else if(touch.phase == swipeType) {				float swipeDistance;				if(direction == Vector2.zero)					return true;				else if(direction == Vector2.left || direction == Vector2.right)					swipeDistance = touch.position.x - _startPos.x;				else					swipeDistance = touch.position.y - _startPos.y;				if(Mathf.Abs(swipeDistance) > ((swipeType == TouchPhase.Moved) ? _minSwipeDistance : _fullSwipeDistance)) {					if(direction != Vector2.zero) {						if(direction == Vector2.left || direction == Vector2.right) {							if(direction.x == Mathf.Sign(swipeDistance))								return true;						}						else {							if(direction.y == Mathf.Sign(swipeDistance))								return true;						}					}				}			}		}		return false;	}	private static int GetPinchMotion() {		if(Input.touchCount == 2) {			Touch touchZero = Input.GetTouch(0);			Touch touchOne = Input.GetTouch(1);			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;						if(deltaMagnitudeDiff < 0)				return -1;			else if(deltaMagnitudeDiff > 0)				return 1;		}		return 0;	}}