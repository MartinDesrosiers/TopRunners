using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour {
    private bool _isFalling;
	private float _fallingSpeed;
	private float _maxFallingSpeed;
	private float _timeBeforeDestruction;
    Transform parentTransform;

    private void Start() {
        parentTransform = transform.parent.transform;
		_isFalling = false;
		_fallingSpeed = -2;
		_maxFallingSpeed = -16;
		_timeBeforeDestruction = 6f;
    }

    private void FixedUpdate() {
        if (transform.childCount > 0 || _isFalling) {
			_fallingSpeed += Physics2D.gravity.y * Time.fixedDeltaTime;
			if(_fallingSpeed < _maxFallingSpeed)
				_fallingSpeed = _maxFallingSpeed;

			parentTransform.Translate(new Vector3(0f, _fallingSpeed * Time.fixedDeltaTime, 0f));

            if (!_isFalling) {
				_isFalling = true;
				StartCoroutine(DestroyTimer());
            }
        }
    }

	private IEnumerator DestroyTimer() {
		yield return new WaitForSeconds(_timeBeforeDestruction);
		if(transform.childCount > 0)
			transform.GetChild(0).transform.SetParent(null);

		Destroy(transform.parent.gameObject);
	}
}