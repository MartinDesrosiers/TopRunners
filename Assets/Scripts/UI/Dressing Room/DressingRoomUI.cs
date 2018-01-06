using UnityEngine;
using UnityEngine.UI;

public class DressingRoomUI : MonoBehaviour {

	public GameObject heroImporter;
	public GameObject bottomMenu;
	public GameObject categoryBTN;

	public Color[] menuColors;

	public GameObject[] ObjectToggles;
	public Sprite lockSprite;
	public GameObject ethiquette;
	public GameObject buyBTN;

	public GameObject money;

	public Text playerName;
	public Text level;
	public Image[] health;
	public Sprite heart;
	public Text maxSpeed;
	public Image stamina;
	public Image strenght;
	public Image acceleration;
	public Image jump;

	public GameObject[] hatObjects;
	public GameObject[] shirtObjects;
	public GameObject[] pantsObjects;
	public GameObject[] shoesObjects;
	public GameObject[] weaponObjects;
	public GameObject[] characterObjects;

	// Use this for initialization
	void Start() {
		ethiquette.SetActive(false);
		buyBTN.SetActive(false);
		CategoryChanged("Hats");


        //TO DO MERGE WITH NEW HERO DATA
		/*HeroData tHeroData = HeroData.current;
		name.text = tHeroData.name.ToString();
		level.text = "Level " + tHeroData.level.ToString();
		SetHP((ushort)(tHeroData.health / 100));
		maxSpeed.text = tHeroData.maxSpeed.ToString() + " MPH!";
		stamina.fillAmount = Mathf.Clamp(tHeroData.stamina / 500, 0.0f, 1.0f);
		strenght.fillAmount = Mathf.Clamp(tHeroData.strenght / 500, 0.0f, 1.0f);
		acceleration.fillAmount = Mathf.Clamp(tHeroData.acceleration / 1000, 0.0f, 1.0f);
		jump.fillAmount = Mathf.Clamp(tHeroData.jumpForce / 3000, 0.0f, 1.0f);*/
	}

	// Update is called once per frame
	void Update() {

	}

	private void SetHP(ushort tHp) {
		for(int i = 0; i < tHp; i++) {
			health[i].sprite = heart;
			if(i == 5)
				break;
		}
	}

	//Function called when clicking on a buttons that changes category.
	//Onclick, returns a string assigned in inspector
	public void CategoryChanged(string categorySelected) {

        //TODO DEVELOP NEW UNLOCK SYSTEM
		//MainScript mainScript = (MainScript)heroImporter.GetComponent<MainScript>();

        //int theMenuColor;

        //Check which is the new category and calls the update

        //TODO UPDATE WITH NEW UNLOCKED OBJECTS
        switch (categorySelected) {
			case "Hats":
			//theMenuColor = 0;
			//updateBottomMenu(categorySelected, theMenuColor, hatObjects, mainScript.unlockedLevelObjects);
			break;
			case "Shirts":
			//theMenuColor = 1;
			//updateBottomMenu(categorySelected, theMenuColor, shirtObjects, mainScript.unlockedEnemyObjects);
			break;
			case "Pants":
			//theMenuColor = 2;
			//updateBottomMenu(categorySelected, theMenuColor, pantsObjects, mainScript.unlockedCollectibleObjects);
			break;
			case "Shoes":
			//theMenuColor = 3;
			//updateBottomMenu(categorySelected, theMenuColor, shoesObjects, mainScript.unlockedTrapObjects);
			break;
			case "Weapons":
			//theMenuColor = 4;
			//updateBottomMenu(categorySelected, theMenuColor, weaponObjects, mainScript.unlockedGroundObjects);
			break;
			case "Runner":
			//theMenuColor = 5;
			//updateBottomMenu(categorySelected, theMenuColor, characterObjects, mainScript.unlockedEditObjects);
			break;
		}
	}

	void updateBottomMenu(string categorySelected, int theMenuColor, GameObject[] selectedToggles, string[] unlockedSelectedObjects) {

		//Update the bottom menu colors
		bottomMenu.GetComponent<Image>().color = menuColors[theMenuColor];
		//Update the category button colors and text
		categoryBTN.GetComponent<Image>().color = menuColors[theMenuColor];
		categoryBTN.transform.Find("Text").GetComponent<Text>().text = categorySelected;

		//For each object toggles of the bottom menu, show the attributed object and money, if unlocked.
		for(int i = 0; i < ObjectToggles.Length; i++) {
			if(selectedToggles[i] != null && unlockedSelectedObjects[i] != null) { //If the object exists and is unlocked
																				   //Get the sprite of the objects
				Sprite selectedSprite = selectedToggles[i].GetComponent<SpriteRenderer>().sprite;
				//Assign it to the toggles, with name and money.
				ObjectToggles[i].transform.Find("Image").GetComponent<Image>().sprite = selectedSprite;
				ObjectToggles[i].name = unlockedSelectedObjects[i];
				ObjectToggles[i].GetComponent<Toggle>().interactable = true;
				Debug.Log("There is an object at toogle : " + i);
			}
			else { //Locked objects
				ObjectToggles[i].transform.Find("Image").GetComponent<Image>().sprite = lockSprite;
				ObjectToggles[i].transform.Find("Label").GetComponent<Text>().text = "";
				ObjectToggles[i].GetComponent<Toggle>().interactable = false;
				Debug.Log("This toogle is locked : " + i);
			}
			ObjectToggles[i].GetComponent<Toggle>().isOn = false;
		}
	}

	public void objectSelected(string selectedObject) {
		switch(selectedObject) {
			case "":
			break;
		}
	}
}
