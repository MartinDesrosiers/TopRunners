using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour {
    Transform platform;
    Transform[] wheels;
    short speed;
    short rotationSpeed;
    short direction;
    short wheelRotation;
    float railHeight;
    public float GetPlatformHeight { get { return transform.position.y; } }
	// Use this for initialization
	void Start () {
        platform = transform.GetChild(0);
        wheels = new Transform[platform.childCount];
        for (int i = 0; i < platform.childCount; i++)
        {
            wheels[i] = platform.GetChild(i);
        }
        railHeight = transform.GetChild(1).GetComponent<SpriteRenderer>().size.y;
        rotationSpeed = 360;
        speed = 2;
        direction = 1;
        wheelRotation = -1;
    }
	
	void FixedUpdate () {
        if (!LevelManager.Instance.isPaused)
        {
            platform.Translate(new Vector3(0f, direction * speed * Time.fixedDeltaTime, 0f));
            if (platform.localPosition.y + (platform.localScale.y / 2) > railHeight / 2 ||
                platform.localPosition.y - (platform.localScale.y / 2) < -railHeight / 2)
                direction *= -1;
            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].Rotate(new Vector3(0f, 0f, wheelRotation * direction * rotationSpeed * Time.deltaTime));
                wheelRotation *= -1;
            }
            if (transform.GetComponent<BoxCollider2D>().enabled == true)
                transform.GetComponent<BoxCollider2D>().enabled = false;
        }
        else if(GameManager.Instance.currentState == GameManager.GameState.LevelEditor)
        {
            platform.transform.localPosition = Vector3.zero;
            transform.GetComponent<BoxCollider2D>().enabled = true;
        }
	}
}
