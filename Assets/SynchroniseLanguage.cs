using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SynchroniseLanguage : MonoBehaviour
{
    [SerializeField] private string languageKey;
    void Update()
    {
        GetComponent<TextMeshProUGUI>().text = LanguageManager.instance.GetLocalizedString(languageKey);
    }
}
