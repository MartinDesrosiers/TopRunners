using UnityEngine;

public class TileConnector : MonoBehaviour {

	[System.Serializable]
	public class SpriteTheme {
		public Sprite[] mainBlock;
		public Color mainColor;
        public Color secondaryColor;
	}
	public SpriteTheme[] spriteTheme;
	
	private bool _top, _tleft, _left, _dleft, _down, _dright, _right, _tright, _nonSolidException, _stop;

    public void SetSprite(ref GameObject obj) {
		_stop = false;
		_nonSolidException = false;

		GameObject tObj;
		int spriteIndex = 0;

		if(!IsSolid(obj.name))
			_nonSolidException = true;

		if(obj.name.Contains("Passable"))
			tObj = obj.transform.GetChild(0).gameObject;
		else
			tObj = obj;

		int col = (int)tObj.transform.position.x;
		int row = (int)tObj.transform.position.y;
		Vector3 newPos = tObj.transform.position;
        //Top
        if (row + 1 < 60)
        {
            if(obj.name.Contains("Passable"))
                _top = SetSpriteLoop(col, row + 1, new Vector2(newPos.x, newPos.y + 1));
            else
                _top = SetSpriteLoop(col, row + 1, new Vector2(newPos.x, newPos.y + 1));
        }
        else
            _top = true;

		//TopLeft
		if(row + 1 < 60 && col - 1 > -1)
			_tleft = SetSpriteLoop(col - 1, row + 1, new Vector2(newPos.x - 1, newPos.y + 1));
		else
			_tleft = true;

		//Left
		if(col - 1 > -1)
			_left = SetSpriteLoop(col - 1, row, new Vector2(newPos.x - 1, newPos.y));
		else
			_left = true;

		//DownLeft
		if(row - 1 > -1 && col - 1 > -1)
			_dleft = SetSpriteLoop(col - 1, row - 1, new Vector2(newPos.x - 1, newPos.y - 1));
		else
			_dleft = true;

        //Down
        if (row - 1 > -1)
        {
            if (obj.name.Contains("Passable"))
                _down = SetSpriteLoop(col, row - 1, new Vector2(newPos.x, newPos.y - 1));
            else
                _down = SetSpriteLoop(col, row - 1, new Vector2(newPos.x, newPos.y - 1));
        }
        else
            _down = true;

		//DownRight
		if(row - 1 > -1 && col + 1 < 200)
			_dright = SetSpriteLoop(col + 1, row - 1, new Vector2(newPos.x + 1, newPos.y - 1));
		else
			_dright = true;

		//Right
		if(col + 1 < 200)
			_right = SetSpriteLoop(col + 1, row, new Vector2(newPos.x + 1, newPos.y));
		else
			_right = true;
		
		//TopRight
		if(row + 1 < 60 && col + 1 < 200)
			_tright = SetSpriteLoop(col + 1, row + 1, new Vector2(newPos.x + 1, newPos.y + 1));
		else
			_tright = true;

		//Calculate which sprite to use according to nearby blocs.
		if(_down)
			spriteIndex += 4;
        if (_left && !_right)
            spriteIndex += 1;
        else if (!_left && _right)
            spriteIndex += 2;
        else if (_left && _right)
            spriteIndex += 3;
		//--------------------------------------------------------

		//Reset corner shadows.
		//tObj.transform.GetChild(2).gameObject.SetActive(false);
		//tObj.transform.GetChild(3).gameObject.SetActive(false);
		//tObj.transform.GetChild(4).gameObject.SetActive(false);
		//tObj.transform.GetChild(5).gameObject.SetActive(false);

        //Add grass ( if there's no bloc above the current one ).
        if (!_top){
            tObj.transform.GetChild(1).gameObject.SetActive(true);
        }
        else{
            //Remove grass if there is.
            tObj.transform.GetChild(1).gameObject.SetActive(false);
			
            ////Add corner shadows depending on which sprite is used and nearby blocs.
            ////Top corners.
            //if (spriteIndex == 1)
            //{
            //    if (!_tleft)
            //        tObj.transform.GetChild(2).gameObject.SetActive(true);
            //}
            //else if (spriteIndex == 2)
            //{
            //    if (!_tright)
            //        tObj.transform.GetChild(5).gameObject.SetActive(true);
            //}
            //else if (spriteIndex == 3)
            //{
            //    if (!_tleft)
            //        tObj.transform.GetChild(2).gameObject.SetActive(true);
			//
            //    if (!_tright)
            //        tObj.transform.GetChild(5).gameObject.SetActive(true);
            //}
            //else if (spriteIndex == 5)
            //{
            //    if (!_tleft)
            //        tObj.transform.GetChild(2).gameObject.SetActive(true);
            //}
            //else if (spriteIndex == 6)
            //{
            //    if (!_tright)
            //        tObj.transform.GetChild(5).gameObject.SetActive(true);
            //}
            //else if (spriteIndex == 7)
            //{
            //    if (!_tleft)
            //        tObj.transform.GetChild(2).gameObject.SetActive(true);
			//
            //    if (!_tright)
            //        tObj.transform.GetChild(5).gameObject.SetActive(true);
            //}
        }

		//Add corner shadows depending on which sprite is used and nearby blocs.
		//Bottom corners.
		//if(spriteIndex == 5) {
		//	if(!_dleft)
		//		tObj.transform.GetChild(3).gameObject.SetActive(true);
		//}
		//else if(spriteIndex == 6) {
		//	if(!_dright)
		//		tObj.transform.GetChild(4).gameObject.SetActive(true);
		//}
		//else if(spriteIndex == 7) {
		//	if(!_dleft)
		//		tObj.transform.GetChild(3).gameObject.SetActive(true);
		//
		//	if(!_dright)
		//		tObj.transform.GetChild(4).gameObject.SetActive(true);
		//}
		
		if(obj.name.Contains("Passable")) {
			for(int i = 0; i < obj.transform.childCount; i++)
				obj.transform.GetChild(0).GetChild(i).gameObject.SetActive(tObj.transform.GetChild(i).gameObject.activeSelf);

			obj.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().sprite = spriteTheme[LevelManager.Instance.theme].mainBlock[spriteIndex];
			obj.transform.GetChild(0).GetChild(0).GetComponent<SpriteRenderer>().color = spriteTheme[LevelManager.Instance.theme].mainColor;
            //if(LevelManager.Instance.levelData.objectList[col][row + 1].)
        }
		else {
			//Set the final sprite ( affected by nearby blocs and chosen theme ).
			tObj.transform.GetChild(0).GetComponent<SpriteRenderer>().sprite = spriteTheme[LevelManager.Instance.theme].mainBlock[spriteIndex];
            tObj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = spriteTheme[LevelManager.Instance.theme].mainColor;

            //Color the grass
            tObj.transform.GetChild(1).GetComponent<SpriteRenderer>().color = spriteTheme[LevelManager.Instance.theme].secondaryColor;

            if (!_nonSolidException) {
				if(!_right || !_left || !_down || !_top || _stop) {
					if(tObj.gameObject.GetComponent<BoxCollider2D>() == null) {
						AddComponent(ref tObj);
					}
				}
				else {
					if(tObj.gameObject.GetComponent<BoxCollider2D>() != null) {
						RemoveComponent(ref tObj);
					}
				}
			}

            //tObj.transform.GetChild(0).GetComponent<SpriteRenderer>().color = spriteTheme[LevelManager.Instance.theme].mainColor;
            obj = tObj;
		}
	}

	//Refresh all nearby blocs' sprites when adding or removing a bloc.
	public void RefreshZone(Vector2 centerPosition) {
		int col = (int)centerPosition.x;
		int row = (int)centerPosition.y;

		//Top
		if(row + 1 < 60)
			RefreshLoop(col, row + 1, new Vector2(centerPosition.x, centerPosition.y + 1));

		//TopLeft
		if(row + 1 < 60 && col - 1 > -1)
			RefreshLoop(col - 1, row + 1, new Vector2(centerPosition.x - 1, centerPosition.y + 1));

		//Left
		if(col - 1 > -1)
			RefreshLoop(col - 1, row, new Vector2(centerPosition.x - 1, centerPosition.y));

		//DownLeft
		if(row - 1 > -1 && col - 1 > -1)
			RefreshLoop(col - 1, row - 1, new Vector2(centerPosition.x - 1, centerPosition.y - 1));

		//Down
		if(row - 1 > -1)
			RefreshLoop(col, row - 1, new Vector2(centerPosition.x, centerPosition.y - 1));

		//DownRight
		if(row - 1 > -1 && col + 1 < 200)
			RefreshLoop(col + 1, row - 1, new Vector2(centerPosition.x + 1, centerPosition.y - 1));

		//Right
		if(col + 1 < 200)
			RefreshLoop(col + 1, row, new Vector2(centerPosition.x + 1, centerPosition.y));

		//TopRight
		if(row + 1 < 60 && col + 1 < 200)
			RefreshLoop(col + 1, row + 1, new Vector2(centerPosition.x + 1, centerPosition.y + 1));
	}

	private void RefreshLoop(int colMod, int rowMod, Vector2 position) {
		for(int i = 0; i < LevelManager.Instance.levelData.objectList[colMod / 10][rowMod / 10].Count; i++) {
			if(new Vector2(position.x, position.y) == (Vector2)LevelManager.Instance.levelData.objectList[colMod / 10][rowMod / 10][i].transform.position) {
				GameObject tObj = LevelManager.Instance.levelData.objectList[colMod / 10][rowMod / 10][i];

				if(tObj.tag == "Connectable") {
					SetSprite(ref tObj);
					LevelManager.Instance.levelData.objectList[colMod / 10][rowMod / 10][i] = tObj;
				}
			}
		}
	}
	private bool SetSpriteLoop(int colMod, int rowMod, Vector2 position) {
        for (int i = 0; i < LevelManager.Instance.levelData.objectList[colMod / 10][rowMod / 10].Count; i++)
        {
            GameObject tObj = LevelManager.Instance.levelData.objectList[colMod / 10][rowMod / 10][i];

            if (tObj.tag == "Connectable" || tObj.tag == "Connector")
            {
                if ((Vector2)tObj.transform.position == position)
                {
                    if(!IsSolid(tObj.name))
						_stop = true;

					return true;
                }
            }
        }
		return false;
	}

	private bool IsSolid(string obj) {
		if(obj.Contains("Passable") || obj.Contains("Water") || obj.Contains("Destructible"))
			return false;

		return true;
	}

    void AddComponent(ref GameObject obj)
    {
        obj.gameObject.AddComponent<BoxCollider2D>();
    }

    void RemoveComponent(ref GameObject obj)
    {
        Destroy(obj.gameObject.GetComponent<BoxCollider2D>());
    }
}