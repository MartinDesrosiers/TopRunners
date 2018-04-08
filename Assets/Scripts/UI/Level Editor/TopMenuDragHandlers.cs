using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
///	This script prevents the top menu scrollview in LevelEditorUI from moving unless the player swipes longer than a set distance.
///	If the distance requirement is not met, a ray is cast to detect the button below the player's finger and the button's OnClick event is called.
///	This is needed to prevent the scrollview from sliding when the player is trying to hit the buttons inside of it.
/// This script also prevents game objects from being added to the level when the player's finger exits the scrollview while dragging it.
/// </summary>
public class TopMenuDragHandlers : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	//Image used as a visual aid when dragging an object from the toggle menu down into the gameview.
	public Image tempImg;
    private GameObject content;
    private LevelEditorUI _editorUI;
    private LevelEditor levelEditor;
	private ScrollRect _scrollRect;
	private Toggle _currentToggle;
	//System event starting position.
	private Vector2 _startPos;
	//Is the player dragging an object.
	private bool _objectDrag;


	public void OnBeginDrag(PointerEventData data) {
		//Lock the level editor to prevent objects being added when the player is dragging the menu.
		_editorUI.levelEditor.isLocked = true;

		//Detect and save the toggle below the initial finger position.
		if(data.pointerPressRaycast.gameObject.name == "Image")
			_currentToggle = data.pointerPressRaycast.gameObject.transform.parent.GetComponent<Toggle>();
		else if(data.pointerPressRaycast.gameObject.name == "Highlight")
			_currentToggle = data.pointerPressRaycast.gameObject.transform.parent.transform.parent.GetComponent<Toggle>();
		else
			return;

		//Lock the menu's scrolling.
		_scrollRect.horizontal = false;
		
		tempImg.sprite = GetTempImage();

		//Set the starting position of the finger drag.
		_startPos = data.position;

        _editorUI.UpdateObjType();
        int i;
        int.TryParse(_currentToggle.gameObject.name, out i);
        _editorUI.ChangeObjId(i);
    }

	
	public void OnDrag(PointerEventData data) {

        //Calculate the distance between the current finger position and the starting position of the drag event.
        float distance = Vector2.Distance(_startPos, data.position);

		//Unlock the scrollview if the distance is big enough.
		if(distance > 40f) {
			if(!_objectDrag) {
				if(!_scrollRect.horizontal) {
					//If the player's finger goes below the scrollview.
					if((_startPos - data.position).y > 20f) {
						tempImg.transform.position = data.position;
						tempImg.gameObject.SetActive(true);

						//Enable dragging.
						_objectDrag = true;
					}
					else
						//Enable the menu scrolling.
						_scrollRect.horizontal = true;
				}
			}
			else {
				tempImg.transform.position = data.position;
			}
		}
	}

	
	public void OnEndDrag(PointerEventData data) {
		//Unlock the level editor.
		_editorUI.levelEditor.isLocked = false;
		
		if(_objectDrag) {
			tempImg.gameObject.SetActive(false);
			_objectDrag = false;

			//Get the toggle's id by parsing its name ( the toggles are named in a increasing numerical order ).
			ushort tId;
			ushort.TryParse(_currentToggle.gameObject.name, out tId);
            _editorUI.ChangeObjId(tId);
            _editorUI.levelEditor.DragObject();
		}
		//If the player never dragged far enough to unlock the scrollview.
		else if(!_scrollRect.horizontal) {
			//Manually call the toggle's OnClick event.
			_currentToggle.OnPointerClick(data);
			//Unlock the scrollview.
			_scrollRect.horizontal = true;
		}

		_currentToggle = null;
	}

    Sprite GetTempImage() {
        int i;
        if (int.TryParse(_currentToggle.gameObject.name, out i))
            return content.transform.GetChild(i).GetChild(1).GetComponent<Image>().sprite;

        return content.transform.GetChild(0).GetChild(1).GetComponent<Image>().sprite;
    }

    private void Start() {
		_editorUI = transform.parent.transform.parent.GetComponent<LevelEditorUI>();
        levelEditor = _editorUI.GetLevelEditor;
        _scrollRect = gameObject.GetComponent<ScrollRect>();
		_scrollRect.horizontal = false;
        content = GameObject.Find("Content");
    }
}