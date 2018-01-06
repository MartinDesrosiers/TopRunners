using UnityEngine;

public class ConvoyerScript : MonoBehaviour {
    //float totalSpeed = 0;
    //float speedPenalty = 3f;

    private void OnTriggerStay2D(Collider2D col) {
        if(col.gameObject.name == "isGroundCollider")
            col.transform.parent.GetComponentInParent<PlayerController>().SetConvoyerBeltForce = 4;
    }

    private void OnTriggerExit2D(Collider2D col) {
        if (col.gameObject.name == "isGroundCollider")
            col.transform.parent.GetComponentInParent<PlayerController>().SetConvoyerBeltForce = 0;
    }
}
