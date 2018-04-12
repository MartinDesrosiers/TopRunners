using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum e_language { english, french };

public class MenuOptions: MonoBehaviour  {

    //Language
    public static e_language language;
    public delegate void LanguangeChangeHandler ();
    public static event LanguangeChangeHandler languageChanged;

    public void SetLanguage (int languageIndex) 
    {     
        // check if same value
        if (languageIndex == (int) language) 
		{
            return;
        }

        if (System.Enum.GetName (typeof (e_language), languageIndex) == null) 
		{            
            return;
        }

        language = (e_language) languageIndex;

        // Broadcast change to the suscribers
        if(languageChanged != null)
        {
            languageChanged();
        }
    }

}