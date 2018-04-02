using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeBoost : MonoBehaviour
{
    bool playerOnTop = false;
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.name == "isGroundCollider")
        {
            playerOnTop = true;
            return;
        }
        PlayerController ply = collision.transform.parent.GetComponentInParent<PlayerController>();
        if (collision.gameObject.name == "isWalledCol")
        {
            if (ply.GetMovementState[BooleenStruct.ISROLLING] && !transform.GetComponent<BoxCollider2D>().isTrigger && !playerOnTop)
            {
                transform.GetComponent<Collider2D>().isTrigger = true;
                ply.StartGlitchTimer("SpeedBoost", 1.5f);
            }
        }
    }
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.name == "RollCollider")
        {
            transform.GetComponent<Collider2D>().isTrigger = false;
        }
        else if (collision.gameObject.name == "isGroundCollider")
            playerOnTop = false;
    }
}