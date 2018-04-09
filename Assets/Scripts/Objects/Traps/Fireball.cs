using UnityEngine;

public class Fireball : MonoBehaviour {

	private bool _isOn = false;
	
	private void FixedUpdate() {
		if(!LevelManager.Instance.IsPaused) {
			if(LevelManager.Instance.isReloading)
				DestroyImmediate(gameObject);
			else {
				if(_isOn) {
					if((Camera.main.transform.position - transform.position).magnitude < 22f)
						transform.Translate(Vector2.up * 8f * Time.fixedDeltaTime);
					else
						Destroy(gameObject);
				}
				else if((Camera.main.transform.position - transform.position).magnitude < 22f)
					_isOn = true;
			}
		}
	}
}