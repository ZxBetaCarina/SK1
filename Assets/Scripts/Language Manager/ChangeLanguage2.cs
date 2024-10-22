using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class ChangeLanguage2 : MonoBehaviour
{
    //public TMP_Dropdown languageDropdown;
    public string selectedLanguage;
    [SerializeField] private TextMeshProUGUI _skillGameTitle;

    private void Start()
    {
        //Debug.Log(PlayerPrefs.GetString("LanguageChoosed")); 
        selectedLanguage = PlayerPrefs.GetString("LanguageChoosed");
        LanguageManager.instance.ChangeLanguage(selectedLanguage);
        //if (languageDropdown != null)
        //{
        //    List<string> availableLanguages = new List<string>(LanguageManager.instance.GetAvailableLanguages());
        //    languageDropdown.ClearOptions();
        //    languageDropdown.AddOptions(availableLanguages);
        //    languageDropdown.onValueChanged.AddListener(ChangeLanguage);
        //    //PlayerPrefs.SetString("SelectedLanguage", selectedLanguage);

        //}
    }

    //private void ChangeLanguage(int index)
    //{

    //    selectedLanguage = languageDropdown.options[index].text;
    //    LanguageManager.instance.ChangeLanguage(selectedLanguage);
    //    PlayerPrefs.SetString("LanguageChoosed", selectedLanguage);
    //}
}
