using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDestroy : MonoBehaviour {

    protected void DestroyObj(GameObject obj, float time = 0.0f)
    {
        Destroy(obj, time);
    }
}
