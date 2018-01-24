using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : TriggerDestroy
{
    bool collected = false;
    public string CollectableName()
    {
        if (!collected)
        {
            char[] clone = { '(', 'C', 'l', 'o', 'n', 'e', ')' };
            string name = gameObject.name.TrimEnd(clone);
            DestroyObj(gameObject);
            collected = true;
            return name;
        }
        return "";
    }
}