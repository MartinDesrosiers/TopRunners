using UnityEngine;

public class CameraController : MonoBehaviour {

    //Camera movement type.
    public enum CameraType { Follow, Scroller, Static }
    public CameraType cameraType = CameraType.Follow;

    public bool isControlActive = false;

    //Speed of the camera when in scroller mode ( in units per second ).
    public int scrollerSpeed = 0;

    //Material applied to the level editor grid.
    public Material gridMaterial;

    private Vector2 _mapBoundaries;

    private float _gameviewSize = 20f;

    //Thickness of the zone ( in units ) outside the map boundaries.
    //Used to modify the level editor camera boundaries and allow the player to place and remove blocks
    //	on the edge of the map where the UI would normally block the view.
    private float _editorUIDeadzone = 2f;
    //Camera's movement speed in the level editor.
    private float _translationSpeed = 20f;
    private float _zoomSensitivity = 4f;

    private Grid _grid;


    private void Awake() {
        Camera.main.orthographicSize = _gameviewSize / 2f * Screen.height / Screen.width;
    }


    private void Start() {
        _mapBoundaries = LevelManager.Instance.mapSize;

        //Initialize the grid.
        if (GameManager.Instance.currentState == GameManager.GameState.LevelEditor) {
            _grid = gameObject.AddComponent<Grid>();
            _grid.Initialize(gridMaterial, Camera.main.orthographicSize % 1);
            UpdateGrid();
        }

        if (scrollerSpeed != 0) {
            cameraType = CameraType.Scroller;
            transform.GetChild(0).gameObject.SetActive(true);
        }
        else
            transform.GetChild(0).gameObject.SetActive(false);
    }


    private void Update() {
        if (GameManager.Instance.currentState == GameManager.GameState.LevelEditor) {

            //Debug.Log("Control Is Active");
            if (isControlActive) {
                if (LevelEditorInputs.GetCameraUp())
                    Camera.main.transform.Translate(Vector2.up * _translationSpeed * Time.deltaTime);
                else if (LevelEditorInputs.GetCameraLeft())
                    Camera.main.transform.Translate(Vector2.left * _translationSpeed * Time.deltaTime);
                else if (LevelEditorInputs.GetCameraDown())
                    Camera.main.transform.Translate(Vector2.down * _translationSpeed * Time.deltaTime);
                else if (LevelEditorInputs.GetCameraRight())
                    Camera.main.transform.Translate(Vector2.right * _translationSpeed * Time.deltaTime);

                //Zoom in/out.
                if (LevelEditorInputs.GetZoomIn())
                    Camera.main.orthographicSize -= _zoomSensitivity * Time.deltaTime;
                else if (LevelEditorInputs.GetZoomOut())
                    Camera.main.orthographicSize += _zoomSensitivity * Time.deltaTime;
            }

            //Screen base size
            float baseSize = _gameviewSize / 2f * Screen.height / Screen.width;

            //Makes sure you can't zoome in / out too much.
            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, baseSize, baseSize * 3);

            //After moving the camera, checks if it's still inside the gameview.
            CheckBoundaries();

            UpdateGrid();
        }

        //Maps interfaces can't zoom. only level editor
        else if (GameManager.Instance.currentState == GameManager.GameState.Map) {

            //Camera movement.
            if (LevelEditorInputs.GetCameraLeft())
                transform.Translate(Vector2.left * _translationSpeed * Time.deltaTime);
            if (LevelEditorInputs.GetCameraRight())
                transform.Translate(Vector2.right * _translationSpeed * Time.deltaTime);

            //Screen base size
            float baseSize = _gameviewSize / 2f * Screen.height / Screen.width;

            Camera.main.orthographicSize = Mathf.Clamp(Camera.main.orthographicSize, baseSize, baseSize * 3);
            CheckBoundaries();
        }
    }


    private void LateUpdate() {
        if (!LevelManager.Instance.IsPaused) {
            switch (cameraType) {
                case CameraType.Follow:
                    //Sets the camera's position to the player's position.
                    SetPositionXY(LevelManager.Instance.player.transform.position);
                    break;

                case CameraType.Scroller:
                    Vector3 tPosition = transform.position;
                    //Increments the scroller's position.
                    tPosition.x += scrollerSpeed * Time.fixedDeltaTime;
                    tPosition.y = LevelManager.Instance.player.transform.position.y;
                    transform.position = tPosition;

                    break;

                case CameraType.Static:

                    break;

                default:
                    break;
            }
        }

        //If the level editor scene is active, changes the camera's boundaries to include the deadzone.
        if (GameManager.Instance.currentState == GameManager.GameState.LevelEditor)
            _editorUIDeadzone = 2f;
        else
            _editorUIDeadzone = 0f;

        CheckBoundaries();
    }

    //Only change the camera's position on the X axis.
    private void SetPositionX(float newPosition) {
        transform.position = new Vector3(newPosition, transform.position.y, transform.position.z);
    }
    //Only change the camera's position on the Y axis.
    private void SetPositionY(float newPosition) {
        transform.position = new Vector3(transform.position.x, newPosition, transform.position.z);
    }
    //Change the camera's position without affecting the Z axis.
    private void SetPositionXY(Vector2 newPosition) {
        transform.position = new Vector3(newPosition.x, newPosition.y, transform.position.z);
    }

    //Clamps the camera's position inside the map boundaries.
    private void CheckBoundaries() {
        //Calculates the width of the camera ( game view ) in world units.
        float horizonExtent = Camera.main.orthographicSize * Screen.width / Screen.height;
        //Calculates the height of the camera ( game view ) in world units.
        float verticalExtent = Camera.main.orthographicSize;

        //Verifies the horizontal boundaries ( including the additional space for the Level Editor UI ).
        if (transform.position.x < horizonExtent - _editorUIDeadzone)
            SetPositionX(horizonExtent - _editorUIDeadzone);
        else if (transform.position.x > _mapBoundaries.x - horizonExtent + _editorUIDeadzone)
            SetPositionX(_mapBoundaries.x - horizonExtent + _editorUIDeadzone);

        //Verifies the vertical boundaries ( including the additional space for the Level Editor UI ).
        if (transform.position.y < verticalExtent - _editorUIDeadzone)
            SetPositionY(verticalExtent - _editorUIDeadzone);
        else if (transform.position.y > _mapBoundaries.y - verticalExtent + _editorUIDeadzone)
            SetPositionY(_mapBoundaries.y - verticalExtent + _editorUIDeadzone);
    }


    //Update the grid size and position according to the camera's current orthographic size, position and the screen resolution.
    private void UpdateGrid() {
        _grid.ResizeGrid(new Vector2(Camera.main.orthographicSize * 2f / ((float)Screen.height / Screen.width), Camera.main.orthographicSize * 2));
        _grid.MoveGrid(new Vector2(transform.position.x % 1, transform.position.y % 1));
    }


    //Render the grid when the camera is done rendering everything else.
    private void OnPostRender() {
        if (GameManager.Instance.currentState == GameManager.GameState.LevelEditor)
            _grid.ShowGrid();
    }
}