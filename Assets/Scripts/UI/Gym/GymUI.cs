using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GymUI : MonoBehaviour {

    // Get the content in the abilities panel
    public Image abilityPanel;
    public Image highlightAbilityPanel;
    public Text playerName;
    public Text levelText;
    public Image[] healthIcons;
    public Image[] strengthIcons;
    public Image[] combosIcons;

    // Get the content in the stats panel
    public Text maxspeedText;
    public Text accelerationText;
    public Text sprintText;
    public Text dashText;
    public Text defenseText;

    // Colors for the icons
    public Color highlightOrange;
    public Color defaultWhite;

    // Colors for the abilities panel
    public Color azuraColor;
    public Color stankoColor;

    // Access the Training Panel objects
    public GameObject trainingPanel;
    public GameObject[] trainingPanelObjects;

    // Access the Character Locked Panel objects
    public Text lockedCharacterTitle;
    public Text lockedCharacterDesc;

    // Access the Energy Bar Panel objects
    public Text[] energyBarPanelTexts;
    public Button[] energyBarPanelButtons;

    // Access the Rewards Panel objects
    public Text[] rewardsPanelTexts;
    public GameObject[] rewardsPanelButton;

    // Access the Receive Rewards Panel
    public GameObject receiveRewardPanel;
    public Text[] receiveRewardsPanelTexts;
    public GameObject[] receiveRewardsPanelButtons;

    // Values are placeholder only. They should be replaced with the real data.
    // These values are programmed in the UI. Replacing them should be easy peasy.
    public bool[] characters;
    public string currentCharacter = "Azura";
    public GameObject[] charactersButtons;
    public int level = 7;
    public int health = 2;
    public int strenght = 4;
    public int combos = 3;
    public int maxspeed = 5;
    public int acceleration = 7;
    public int sprint = 5;
    public int dash = 6;
    public int defense = 2;

    public int energyBars = 1;
    public Text energyBarsCounter;

    public int trainingPoints = 1;
    public int remainingSeconds = 60;

    public int availableTrainingAds = 2;

    public string currentTraining;

    private void Start() {

        // Unlocked(true) vs Locked(false) characters 
        characters[0] = true;
        characters[1] = true;
        characters[2] = false;

        InitCharactersButtons();
        UpdateAbilities();
        UpdateStats();
        UpdateEnergyBarPanel();
        UpdateMainEnergyBarCounter();
        UpdateTrainingPanel();
    }

    private void Update() {

    }

    public void InitCharactersButtons() {
        for (int i = 0; i < characters.Length; i++) {
            if (!characters[i]) {
                charactersButtons[i].GetComponent<Toggle>().interactable = false;
                charactersButtons[i].transform.GetChild(1).gameObject.SetActive(true);
            }
        }

        //TODO: A similar function should be done to check which toggle should be "ON" based on last selected character
    }

    public void SelectCharacter(string selectedCharacter) {
        currentCharacter = selectedCharacter;
        UpdateAbilities();
        UpdateStats();
        UpdateMainEnergyBarCounter();

    }

    public void UpdateAbilities() {
        playerName.text = currentCharacter;
        levelText.text = "LEVEL " + level;

        //Set the color of the Panel
        switch (currentCharacter) {
            case "Stanko": {
                    abilityPanel.color = stankoColor;
                    highlightAbilityPanel.color = stankoColor;
                }
                break;
            case "Azura": {
                    abilityPanel.color = azuraColor;
                    highlightAbilityPanel.color = azuraColor;
                }
                break;
        }

        // Reset the colors of the icons
        for (int i = 0; i < 5; i++) {
            healthIcons[i].color = defaultWhite;
        }
        for (int i = 0; i < 5; i++) {
            strengthIcons[i].color = defaultWhite;
        }
        for (int i = 0; i < 5; i++) {
            combosIcons[i].color = defaultWhite;
        }

        // Apply the orange
        for (int i = 0; i < health; i++) {
            healthIcons[i].color = highlightOrange;
        }
        for (int i = 0; i < strenght; i++) {
            strengthIcons[i].color = highlightOrange;
        }
        for (int i = 0; i < combos; i++) {
            combosIcons[i].color = highlightOrange;
        }
    }

    public void UpdateStats() {
        maxspeedText.text = maxspeed + "/100";
        accelerationText.text = acceleration + "/100";
        sprintText.text = sprint + "/100";
        dashText.text = dash + "/100";
        defenseText.text = defense + "/100";
    }

    public void onCLick(string objectClicked) {

        switch (objectClicked) {
            case "Back":
                SceneManager.LoadScene("MainMenu");
                break;
            case "dressing":
                SceneManager.LoadScene("DressingRoom");
                break;
            case "Store": {
                    SceneManager.LoadScene("Store");
                }
                break;
        }
    }

    public void OpenTrainPanel(string selectedTraining) {

        currentTraining = selectedTraining;

        switch (selectedTraining) {
            case "max-speed":
                trainingPanelObjects[0].GetComponent<Text>().text = "Sprint";
                trainingPanelObjects[1].GetComponent<Text>().text = "Improves " + currentCharacter + "'s max speed. " +
                    "With more max speed, run faster once your acceleration is completed.";
                break;
            case "acceleration":
                trainingPanelObjects[0].GetComponent<Text>().text = "Spinning";
                trainingPanelObjects[1].GetComponent<Text>().text = "Improves " + currentCharacter + "'s acceleration. " +
                    "With more acceleration, the time required to reach max speed is shorter.";
                break;
            case "sprint":
                trainingPanelObjects[0].GetComponent<Text>().text = "Leg Press";
                trainingPanelObjects[1].GetComponent<Text>().text = "Improves " + currentCharacter + "'s sprint. " +
                    "With better sprint, the sprint lasts longer and recovers faster.";
                break;
            case "dash":
                trainingPanelObjects[0].GetComponent<Text>().text = "Volleyball";
                trainingPanelObjects[1].GetComponent<Text>().text = "Improves " + currentCharacter + "'s dash. " +
                    "With better dash, jump attacks are performed faster and push enemies farther away.";
                break;
            case "defense":
                trainingPanelObjects[0].GetComponent<Text>().text = "Boxing";
                trainingPanelObjects[1].GetComponent<Text>().text = "Improves " + currentCharacter + "'s defense. " +
                    "With better defense, the invincibility period is longer and the recoil is bigger after receiving an attack.";
                break;
        }

        UpdateTrainingPanel();
        trainingPanel.SetActive(true);
    }

    // Called when pressing button "Train" in training-panel
    // Level up the current character + 1 stat
    public void LevelUp() {

        trainingPanel.SetActive(false);

        trainingPoints--;
        level++;

        switch (currentTraining) {
            case "max-speed":
                maxspeed++;
                break;
            case "acceleration":
                acceleration++;
                break;
            case "sprint":
                sprint++;
                break;
            case "dash":
                dash++;
                break;
            case "defense":
                defense++;
                break;
        }

        UpdateStats();
        UpdateTrainingPanel();
        UpdateEnergyBarPanel();
        UpdateAbilities();
        // Checks if a level up reward was obtained
        UnlockReward();
    }

    public void watchTrainingRewardAd() {
        availableTrainingAds--;
        trainingPoints++;
        UpdateTrainingPanel();
        UpdateEnergyBarPanel();
    }

    // Rewards given on specific levels depending of the characters.
    // Opens receive-reward-panel
    public void UnlockReward() {

        switch (level) {
            case 1:
                receiveRewardPanel.SetActive(true);
                break;
            case 5:
                receiveRewardPanel.SetActive(true);
                break;
            case 10:
                receiveRewardPanel.SetActive(true);
                break;
            case 15:
                receiveRewardPanel.SetActive(true);
                break;
            case 20:
                receiveRewardPanel.SetActive(true);
                break;
            case 25:
                receiveRewardPanel.SetActive(true);
                break;
            case 30:
                receiveRewardPanel.SetActive(true);
                break;
            case 40:
                receiveRewardPanel.SetActive(true);
                break;
            case 50:
                receiveRewardPanel.SetActive(true);
                break;
            case 60:
                receiveRewardPanel.SetActive(true);
                break;
            case 70:
                receiveRewardPanel.SetActive(true);
                break;
            case 100:
                receiveRewardPanel.SetActive(true);
                break;
            default:
                break;
        }
    }

    public void UpdateTrainingPanel() {

        if (energyBars == 0) {
            trainingPanelObjects[6].GetComponent<Button>().interactable = false;
        }
        else {
            trainingPanelObjects[6].GetComponent<Button>().interactable = true;
        }

        if (trainingPoints == 0) {
            trainingPanelObjects[7].GetComponent<Button>().interactable = false;
        }
        else {
            trainingPanelObjects[7].GetComponent<Button>().interactable = true;
        }

        if (availableTrainingAds == 0) {
            trainingPanelObjects[5].GetComponent<Button>().interactable = false;
        }
        else {
            trainingPanelObjects[5].GetComponent<Button>().interactable = true;
        }

        trainingPanelObjects[3].GetComponent<Text>().text = "YOUR TRAINING POINTS: " + trainingPoints;

    }

    public void UpdateEnergyBarPanel() {
        energyBarPanelTexts[2].text = "YOU HAVE " + energyBars;
        energyBarPanelTexts[3].text = "YOU HAVE " + trainingPoints + " TRAINING POINTS";

        if(energyBars == 0) {
            energyBarPanelButtons[2].interactable = false;
        }
        else {
            energyBarPanelButtons[2].interactable = true;
        }
    }

    public void UpdateMainEnergyBarCounter() {
        energyBarsCounter.text = "x " + energyBars;
    }

    // 1 Energy bars gives back 3 training points
    // Called from training-panel and energy-bar-panel
    public void EatEnergyBar() {
        energyBars--;
        trainingPoints += 3;

        UpdateTrainingPanel();
        UpdateEnergyBarPanel();
        UpdateMainEnergyBarCounter();
    }
}
