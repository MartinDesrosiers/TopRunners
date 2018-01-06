using UnityEngine;

[System.Serializable]
public abstract class Tile {
	public abstract bool isExtended { get; }

	public ushort type;
	public ushort id;
	public float posX;
	public float posY;
	public float rotation;
	public bool horizontalMirror;

	public Vector2 GetPosition() {
		return new Vector2(posX, posY);
	}

	public void SetPosition(Vector2 tPos) {
		posX = tPos.x;
		posY = tPos.y;
	}

	public virtual void Serialize(GameObject tObj) { }
	public virtual void Deserialize(ref GameObject tObj) { }
}