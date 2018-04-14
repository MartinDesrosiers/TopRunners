using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_languageSelection : MonoBehaviour 
{
	public string englishText = " ... english ...";
	public string frenchText = " ... francais ...";

	void OnEnable()
	{				
		MenuOptions.languageChanged += UpdateLanguage;
		UpdateLanguage();
	}

	void OnDisable()
	{			
		MenuOptions.languageChanged -= UpdateLanguage;	
	}

	void UpdateLanguage()
	{
		
		switch(MenuOptions.language)
		{
			case e_language.english:	this.gameObject.GetComponent<UnityEngine.UI.Text>().text = englishText;
										break;
			case e_language.french:		this.gameObject.GetComponent<UnityEngine.UI.Text>().text = frenchText;
										break;	
		}
	}

	public void SetText(string englishText = " ... ", string frenchText = " ... " )
	{
		this.englishText = englishText;
		this.frenchText = frenchText;
		UpdateLanguage();
	}

}