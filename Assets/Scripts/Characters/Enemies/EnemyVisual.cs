using UnityEngine;

public class EnemyVisual : MonoBehaviour {
	
	public Color primaryColor;
	public Color secondaryColor;

	public SpriteRenderer[] primarySprites;
	public SpriteRenderer[] secondarySprites;

	private void Start() {
		SetColor();
	}

	//Set the color of every sprites inside the arrays.
	public void SetColor() {
		for(int i = 0; i < primarySprites.Length; i++)
			primarySprites[i].color = primaryColor;
		for(int i = 0; i < secondarySprites.Length; i++)
			secondarySprites[i].color = secondaryColor;
	}
}
