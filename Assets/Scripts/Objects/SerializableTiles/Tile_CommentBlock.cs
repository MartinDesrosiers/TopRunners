using UnityEngine;

public class Tile_CommentBlock : Tile {
	public override bool isExtended { get { return true; } }
	
	private string _text;

	public override void Serialize(GameObject tObj) {
		Debug.Log("test2.0 :: , " + _text);
		_text = tObj.GetComponent<CommentPanel>().inputField.text;
		Debug.Log("test2.1 :: , " + _text);
	}

	public override void Deserialize(ref GameObject tObj) {
		Debug.Log("test3.0 :: , " + _text);
		tObj.GetComponent<CommentPanel>().inputField.text = _text;
		Debug.Log("test3.1 :: , " + _text);
	}
}