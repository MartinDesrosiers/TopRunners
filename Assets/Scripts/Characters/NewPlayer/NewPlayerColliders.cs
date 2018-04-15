using UnityEngine;

public class NewPlayercolliders {
	public GameObject colliders;
	public GameObject rollColliders;
	public EdgeCollider2D mainEdgeCollider;
	public EdgeCollider2D wallEdgeCollider;
	public Vector2[] normalCollidersPoints, jumpingCollidersPoints, standingCollider, slidingCollider;

	public NewPlayercolliders(Transform playerTransform) {
		colliders = playerTransform.GetChild(0).GetChild(0).gameObject;
		rollColliders = playerTransform.GetChild(0).GetChild(1).gameObject;
		mainEdgeCollider = playerTransform.GetChild(0).GetChild(0).GetComponent<EdgeCollider2D>();
		wallEdgeCollider = playerTransform.GetChild(0).GetChild(2).GetComponent<EdgeCollider2D>();
		normalCollidersPoints = new Vector2[4];
		jumpingCollidersPoints = new Vector2[4];

		for(int i = 0; i < mainEdgeCollider.pointCount; i++) {
			normalCollidersPoints[i] = mainEdgeCollider.points[i];
			jumpingCollidersPoints[i] = normalCollidersPoints[i];
		}

		jumpingCollidersPoints[1] = new Vector2(0.3f, -0.45f);
		jumpingCollidersPoints[2] = new Vector2(-0.55f, -0.98f);
		standingCollider = new Vector2[] { new Vector2(.33f, .7f), new Vector2(.33f, -1f) };
		slidingCollider = new Vector2[] { new Vector2(.45f, -.6f), new Vector2(.45f, -1f) };
	}
}