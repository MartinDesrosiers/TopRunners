using UnityEngine;

[System.Serializable]
public class Tile_CommentBlock : Tile {
	public override bool isExtended { get { return true; } }
	
	private string _text;

	public override void Serialize(GameObject tObj) {
		_text = tObj.GetComponent<CommentPanel>().inputField.text;
	}

	public override void Deserialize(ref GameObject tObj) {
		tObj.GetComponent<CommentPanel>().inputField.text = _text;
	}
}