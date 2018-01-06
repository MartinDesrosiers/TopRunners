using UnityEngine;
using System.Collections.Generic;

public class Localization {

	public static Localization current;

	//The current language
	public string gameLanguage;

	//One dictionary for each languages. Same tags, but different translations
	Dictionary <string,string> englishTranslations = new Dictionary <string,string>();
	Dictionary <string,string> frenchTranslations = new Dictionary <string,string>();

	public Localization() {
        //Load the language in SettingsData. If null, check system's language.

        //TODO create new save file for languages
        /*SaveAndLoadData.LoadSettings();
		SettingsData.current = SaveAndLoadData.savedSettings;
		if (!System.String.IsNullOrEmpty(SettingsData.current.language)) {
			gameLanguage = SettingsData.current.language;
		} else {*/
			gameLanguage = "French";
			gameLanguage = Application.systemLanguage.ToString ();
			Debug.Log (gameLanguage); 
		//}

		loadTranslations();
	}

	void loadTranslations(){
		//Dictionaries of translations : (ID string, translated string)
		//Global UI translation
		englishTranslations.Add("locRun","Run");
		frenchTranslations.Add("locRun","Cours");

		englishTranslations.Add("locOnline","Online");
		frenchTranslations.Add("locOnline","En ligne");

		englishTranslations.Add("locLevels","Levels");
		frenchTranslations.Add("locLevels","Niveaux");

		englishTranslations.Add("locScores","Scores");
		frenchTranslations.Add("locScores","Scores");

		englishTranslations.Add("locGym","Gym");
		frenchTranslations.Add("locGym","Gym");

		englishTranslations.Add("locStore","Store");
		frenchTranslations.Add("locStore","Boutique");

		englishTranslations.Add("locBack","Back");
		frenchTranslations.Add("locBack","Retour");
	}

	public string Translate(string textID){

		string translatedText;

		//Checks the current language and translate the textID. Default in english. 
		switch (Localization.current.gameLanguage) {
		case "English" : {
				//Catch error if ID doesn't exist.
				if (Localization.current.englishTranslations.TryGetValue (textID, out translatedText)) {
					return translatedText;
				} else {
					return textID;
				}
			}
		case "French" : {
				if (Localization.current.frenchTranslations.TryGetValue (textID, out translatedText)) {
					return translatedText;
				} else {
					return textID;
				}
			}
		default : {
				if (Localization.current.englishTranslations.TryGetValue (textID, out translatedText)) {
					return translatedText;
				} else {
					return textID;
				}
			}
		}
	}
}
