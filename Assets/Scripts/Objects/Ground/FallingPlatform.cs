using UnityEngine;

public class FallingPlatform : MonoBehaviour {
	private Transform _parentTransform;
	private bool _isFalling;
	private float _fallingSpeed;
	private float _maxFallingSpeed;
	private float _gravity;

    private void Start() {
		_parentTransform = transform.parent.transform;
		_isFalling = false;
		_fallingSpeed = -1.5f;
		_maxFallingSpeed = -12;
		_gravity = -2.5f;
	}

    private void FixedUpdate() {
        if (transform.childCount > 0 || _isFalling) {
			_fallingSpeed += _gravity * Time.fixedDeltaTime;
			
			if(transform.position.y < 0) {
				if(transform.childCount > 0)
					transform.GetChild(0).transform.SetParent(null);

				Destroy(transform.parent.gameObject);
			}
			else {
				if(_fallingSpeed < _maxFallingSpeed)
					_fallingSpeed = _maxFallingSpeed;

				_parentTransform.Translate(new Vector3(0f, _fallingSpeed * Time.fixedDeltaTime, 0f));
				
				_isFalling = true;
			}
        }
    }
}