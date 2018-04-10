using UnityEngine;

public class Grid : MonoBehaviour {

	private Material _mat;
	private Vector2 _screenRatio;
	private Vector2 _screenPos;
	private float _verticalMargin;

	public void Initialize(Material tMat, float tMargin) {
		_mat = tMat;
		_verticalMargin = tMargin;
	}

	public void ShowGrid() {
		GL.PushMatrix();
		_mat.SetPass(0);
		GL.LoadOrtho();

		Vector2 tRatio = new Vector2(_screenPos.x / _screenRatio.x, (_screenPos.y - _verticalMargin) / _screenRatio.y);

		for(int i = -1; i < Mathf.CeilToInt(_screenRatio.x) + 1; i++) {
			GL.Begin(GL.LINES);
				GL.Color(_mat.color);
				GL.Vertex(new Vector2(1 / _screenRatio.x * i - tRatio.x, -0.2f - tRatio.y));
				GL.Vertex(new Vector2(1 / _screenRatio.x * i - tRatio.x, 1.2f - tRatio.y));
			GL.End();
		}
		for(int i = -1; i < Mathf.CeilToInt(_screenRatio.y) + 1; i++) {
			GL.Begin(GL.LINES);
				GL.Color(_mat.color);
				GL.Vertex(new Vector2(-0.2f - tRatio.x, 1 / _screenRatio.y * i - tRatio.y));
				GL.Vertex(new Vector2(1.2f - tRatio.x, 1 / _screenRatio.y * i - tRatio.y));
			GL.End();
		}

		GL.PopMatrix();
	}

	public void ResizeGrid(Vector2 tRatio) {
		_screenRatio = tRatio;
	}

	public void MoveGrid(Vector2 tPos) {
		_screenPos = tPos;
	}
}