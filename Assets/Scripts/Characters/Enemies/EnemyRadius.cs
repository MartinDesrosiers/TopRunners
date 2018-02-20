using UnityEngine;

public class EnemyRadius : MonoBehaviour {

	[Range(0, 10)]
	public ushort radius;		//Radius in which the hero can dash to the enemy.
	public ushort priority;     //If the hero is inside the radius of multiple enemies, the priority dictates which enemy he should dash to.
	public SpriteRenderer area;
	public bool isInvincible = false;

	private void Start() {
		//Adds itself to the global enemy list.
		EnemyList.AddEnemy(this);
		area = gameObject.GetComponent<SpriteRenderer>();
	}

	/*private void OnDestroy() {
		//Removes itself from the global enemy list.
		EnemyList.DestroyEnemy(this);
	}*/

	public void SetAlpha(float alpha) {
		area.color = new Color(1f, 1f, 1f, alpha);
	}
}