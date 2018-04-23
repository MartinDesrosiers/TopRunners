using UnityEngine;

public class Collectable : MonoBehaviour {
	bool collected = false;
	
	public string CollectableName() {
	    if (!collected) {
	        char[] clone = { '(', 'C', 'l', 'o', 'n', 'e', ')' };
	        string name = gameObject.name.TrimEnd(clone);
	        collected = true;
			Destroy(gameObject);
			Debug.Log(name);
	        return name;
	    }
	    return "";
	}
}