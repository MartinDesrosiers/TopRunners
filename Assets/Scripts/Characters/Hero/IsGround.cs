using UnityEngine;
using System.Collections.Generic;

public class IsGround : Destructible {
    private void Start()
    {
        others = new List<Collider2D>();
    }
    //Because the map is made with blocks, count every block the the player is touching at the same time
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8)
        {
            if (others.Count == 0)
            {
                if (transform.tag == "Player" && !transform.parent.GetComponentInParent<PlayerController>().GetMovementState[BooleenStruct.ISDASHING])
                {
                    if(transform.parent.GetComponentInParent<Rigidbody2D>().velocity.y < .5f)
                        transform.parent.GetComponentInParent<PlayerController>().IsGrounded(true);
                    if (col.gameObject.name == "platform") {
                        transform.parent.parent.gameObject.transform.SetParent(col.gameObject.transform);
                    }
                }
                else if (transform.tag == "Enemies")
                {
                    transform.parent.GetComponentInParent<EnemyAI>().IsGrounded(true);
                }
            }
            if (col.gameObject.name.Contains("Destructible") && transform.tag != "Enemies") {
                SetDestroyTimer(col.gameObject);
            }
            others.Add(col);
        }
    }
    //same here exept it count all the block the player doesn't touch anymore and if the count is down to zero, it means that the player doesn't touch the ground anymore
    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.layer == 8)
        {
            if (others.Count > 0)
                others.RemoveAt(0);
            if (others.Count == 0)
            {
                if (transform.tag == "Player")
                {
                    transform.parent.GetComponentInParent<PlayerController>().IsGrounded(false);
                }
                else if (transform.tag == "Enemies")
                {
                    transform.parent.GetComponentInParent<EnemyAI>().IsGrounded(false);
                }
            }
            if (col.gameObject.name == "platform")
            {
                transform.parent.parent.gameObject.transform.SetParent(null);
            }
        }   
    }

    private void FixedUpdate() {
		if(!LevelManager.Instance.isPaused && others.Count > 0) {
            for (int i = 0; i < others.Count; i++)
            {
                if (others[i] == null)
                {
                    others.RemoveAt(0);
                }
            }
			if(others.Count == 0)
				transform.parent.GetComponentInParent<PlayerController>().IsGrounded(false);
		}
	}
}