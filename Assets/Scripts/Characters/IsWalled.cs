using UnityEngine;
using System.Collections.Generic;

public class IsWalled : Destructible
{
    private void Start()
    {
        others = new List<Collider2D>();
    }
    //Because the map is made with blocks, count every block the the player is touching at the same time
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8 && col.tag != "Crossable" && col.tag != "Ramp")
        {
            if (others.Count == 0 && col.tag != "Trap")
            {
                if (transform.tag == "Player")
                {
                    if (col.gameObject.name.Contains("InnerShadow") && col.gameObject.GetComponent<BoxCollider2D>().isTrigger)
                        return;
                    else if (col.gameObject.name.Contains("PipeCol") && transform.parent.GetComponentInParent<PlayerController>().GetMovementState[BooleenStruct.ISROLLING])
                        return;
                    else if (!col.gameObject.name.Contains("CrossableGround"))
                    {
                        if (transform.parent.GetComponentInParent<PlayerController>().GetMovementState[BooleenStruct.ISDASHING])// FIX dan 26fevrier
                            transform.parent.GetComponentInParent<PlayerController>().ResetDash();// FIX dan 26fevrier
                        transform.parent.GetComponentInParent<PlayerController>().IsWalled(true);
                    }
                }
                else if (gameObject.name == "WallCollider")
                    transform.parent.GetComponentInParent<EnemyAI>().IsWalled(true);
            }
            if (col.gameObject.name.Contains("Destructible") && transform.name != "Enemies")
            {
                SetDestroyTimer(col.gameObject);
            }
            others.Add(col);
        }
    }
    //same here exept it count all the block the player doesn't touch anymore and if the count is down to zero, it means that the player doesn't touch the wall anymore
    private void OnTriggerExit2D(Collider2D col)
    {
        if(col.gameObject.layer == 8)
        {
            if (others.Count > 0)
                others.RemoveAt(others.Count - 1);


            if (others.Count == 0)
            {
                if (transform.tag == "Player")
                {
                    transform.parent.GetComponentInParent<PlayerController>().IsWalled(false);
                }
                else if (gameObject.name == "WallCollider")
                    transform.parent.GetComponentInParent<EnemyAI>().IsWalled(false);
            }
        }
    }

    private void Update(){
        if (!LevelManager.Instance.IsPaused && others.Count > 0)
        {
            for (int i = 0; i < others.Count; i++)
            {
                if (!others[i].gameObject.activeInHierarchy)
                {
                    others.RemoveAt(i);
                }
            }
            if (others.Count == 0)
                transform.parent.GetComponentInParent<PlayerController>().IsWalled(false);
        }
    }
}
