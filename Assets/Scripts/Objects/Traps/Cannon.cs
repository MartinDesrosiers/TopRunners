using UnityEngine;

public class Cannon : MonoBehaviour {

	public GameObject fireball;

	private float _rateOfFire = 2f;
	private float _timerPosition = 0f;

	private void FixedUpdate() {
		if(!LevelManager.Instance.IsPaused) {
			if((Camera.main.transform.position - transform.position).magnitude < 22f) {
				if(Mathf.Abs(Camera.main.transform.position.y - transform.position.y) < 5.5f) {
					if(_timerPosition == 0f) {
						Instantiate(fireball, transform.GetChild(0).transform.position, Quaternion.Euler(0f, 0f, (transform.localScale.x < 0f) ? 90f : 270f));
						_timerPosition = 0.001f;
					}
					else
						_timerPosition += 1f / _rateOfFire * Time.fixedDeltaTime;
				}
			}

			if(_timerPosition > 0) {
				_timerPosition += 1f / _rateOfFire * Time.fixedDeltaTime;

				if(_timerPosition > _rateOfFire)
					_timerPosition = 0f;
			}
		}
	}
}