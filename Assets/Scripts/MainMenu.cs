using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetFloat("Balance", 500);
        PlayerPrefs.SetInt("FirstGame", 1); // 0 represents false
        PlayerPrefs.Save();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChangeScene()
    {
        SceneManager.LoadScene(1);
    }
}
