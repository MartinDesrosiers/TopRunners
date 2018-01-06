using UnityEngine;

public class RevertGlitch : MonoBehaviour {
    bool already = false;

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.tag == "Player" && !already) {
            Destroy(gameObject);
            collision.transform.parent.GetComponent<PlayerController>().Flip();
            already = true;
        }    
    }
}
