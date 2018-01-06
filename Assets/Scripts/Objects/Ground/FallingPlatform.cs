using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingPlatform : MonoBehaviour {
    bool falling;
    short speed;
    float timeBeforeDestruction;
    Transform parentTransform;

    private void Start()
    {
        parentTransform = transform.parent.transform;
        falling = false;
        speed = 2;
        timeBeforeDestruction = 8f;
    }

    private void FixedUpdate()
    {
        if (transform.childCount > 0 || falling)
        {
            parentTransform.Translate(new Vector3(0f, -speed * Time.fixedDeltaTime, 0f));
            if (!falling)
            {
                falling = true;
                Destroy(transform.parent.gameObject, timeBeforeDestruction);
            }
        }
    }
}
