using UnityEngine;
using System.Collections.Generic;

public class Parallax : MonoBehaviour {

	[System.Serializable]
	public class ParallaxLevels {
		public GameObject level;
		public float relativeSpeed;
		public float boundaries;
		public bool affectYAxis;

		[HideInInspector]
		public List<Transform> elements;

		public void Init() {
			elements = new List<Transform>();

			for(int i = 0; i < level.transform.childCount; i++) {
				elements.Add(level.transform.GetChild(i));
			}
		}
	}
	public List<ParallaxLevels> parallaxLevels = new List<ParallaxLevels>();

	private GameObject mainCamera;

	private Vector3 lastPosition;

	private void Start() {
		mainCamera = Camera.main.gameObject;
		lastPosition = mainCamera.transform.position;

		for(int i = 0; i < parallaxLevels.Count; i++) {
			parallaxLevels[i].Init();
		}
	}

	private void Update() {
		if(!LevelManager.Instance.IsPaused) {
			Vector2 distanceTraveled = mainCamera.transform.position - lastPosition;

			for(int i = 0; i < parallaxLevels.Count; i++) {
				float tSpeed = parallaxLevels[i].relativeSpeed;
				float tBoundaries = parallaxLevels[i].boundaries;
				bool tAffectYAxis = parallaxLevels[i].affectYAxis;

				for(int o = 0; o < parallaxLevels[i].elements.Count; o++) {
					parallaxLevels[i].elements[o].transform.Translate(new Vector2(distanceTraveled.x * -tSpeed, tAffectYAxis ? distanceTraveled.y * -tSpeed : 0.0f));

					Vector2 tPos = parallaxLevels[i].elements[o].transform.position;
					if(tPos.x < mainCamera.transform.position.x - tBoundaries) {
						tPos.x += tBoundaries * 2;
						parallaxLevels[i].elements[o].transform.position = tPos;
					}
					else if(tPos.x > mainCamera.transform.position.x + tBoundaries) {
						tPos.x -= tBoundaries * 2;
						parallaxLevels[i].elements[o].transform.position = tPos;
					}
				}
			}

			lastPosition = mainCamera.transform.position;
		}
	}

	//Called during the cameraController's update to counter the Y movement.
	//Has to be called by the camera in order to cancel the sprites jittering ( delay between updates ).
	public void SmoothBackground() {
		if(!LevelManager.Instance.IsPaused) {
			Vector2 distanceTraveled = mainCamera.transform.position - lastPosition;

			transform.GetChild(0).transform.Translate(new Vector2(0.0f, -distanceTraveled.y));
		}
	}
}