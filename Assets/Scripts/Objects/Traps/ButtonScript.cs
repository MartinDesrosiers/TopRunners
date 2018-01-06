using UnityEngine;

public class ButtonScript : MonoBehaviour {
	//Vector2 buttonThickness;
    float speed;
    float originalYPos;
    float timer;
    float delay;
    float startTimer;
    float rotation;
    bool hasBeenPressed;
    bool Unpressed;
    bool timerIsStart;

    public bool GetHasBeenPressed { get { return hasBeenPressed; } }

    private void Start() {
        //buttonThickness = new Vector2(transform.GetComponent<SpriteRenderer>().size.y, transform.GetComponent<SpriteRenderer>().size.y);
        timerIsStart = false;
        Unpressed = false;
        originalYPos = transform.localPosition.y;
        timer = 0f;
        delay = 15f;
        speed = 1f;
        hasBeenPressed = false;
    }

    private void OnCollisionEnter2D(Collision2D col) {
        if(col.transform.tag == "Player" && !hasBeenPressed) {
            if (!hasBeenPressed) {
                hasBeenPressed = true;
                timerIsStart = true;

                for(int i = 0; i < LevelManager.Instance.doorList.Count; i++) {
                    LevelManager.Instance.doorList[i].transform.GetChild(1).GetComponent<DoorScript>().OpenDoor();
                    LevelManager.Instance.doorList[i].transform.GetChild(1).GetComponent<DoorScript>().SetButtonIsPushed = hasBeenPressed;
                }
            }
        }
    }

    private void FixedUpdate() {
        if (hasBeenPressed) {
            if (transform.localPosition.y > 0)
                transform.Translate(new Vector3(0f, -speed * Time.deltaTime, 0f));
            
            if (timerIsStart) {
                timer = Time.time;
                timerIsStart = false;
            }

            if(Time.time - timer > delay) {
                hasBeenPressed = false;
                Unpressed = true;

                for (int i = 0; i < LevelManager.Instance.doorList.Count; i++) {
                    LevelManager.Instance.doorList[i].transform.GetChild(1).GetComponent<DoorScript>().SetButtonIsPushed = hasBeenPressed;
                    LevelManager.Instance.doorList[i].transform.GetChild(1).GetComponent<DoorScript>().CloseDoor();
                }
            }
        }
        else if(Unpressed) {
            if (transform.localPosition.y < originalYPos)
                transform.Translate(new Vector3(0f, speed * Time.fixedDeltaTime, 0f));
            else
                Unpressed = false;
        }
    }
}
