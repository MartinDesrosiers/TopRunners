using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    bool collected = false;
    public string CollectableName()
    {
        if (!collected)
        {
            char[] clone = { '(', 'C', 'l', 'o', 'n', 'e', ')' };
            string name = gameObject.name.TrimEnd(clone);
            gameObject.SetActive(false);
            collected = true;
            return name;
        }
        return "";
    }
    private void OnEnable()
    {
        collected = false;
    }
}