using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : MonoBehaviour
{
    bool inTheDoor = false;
    bool buttonIsPushed = false;
    bool hasBeenOpenWithKey = false;
    public bool SetButtonIsPushed { set { buttonIsPushed = value; } }
    public bool GetInTheDoor { get { return inTheDoor; } }
    private void OnTriggerEnter2D(Collider2D col)
    {
        inTheDoor = true;
        if (col.gameObject.tag == "Player")
        {
            if (!transform.GetComponent<BoxCollider2D>().isTrigger && LevelManager.Instance.player.GetComponent<PlayerController>().GetKey > 0)
            {
                OpenDoor();
                LevelManager.Instance.player.GetComponent<PlayerController>().RemoveKey();
                hasBeenOpenWithKey = true;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.name.Contains("NormalColliders") || col.gameObject.name.Contains("RollCollider"))
        {
            inTheDoor = false;
            CloseDoor();
        }
    }
    public void OpenDoor()
    {
        transform.GetComponent<BoxCollider2D>().isTrigger = true;
    }
    public void CloseDoor()
    {
        if(!hasBeenOpenWithKey && !inTheDoor && !buttonIsPushed)
            transform.GetComponent<BoxCollider2D>().isTrigger = false;
    }
}
