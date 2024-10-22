using UnityEngine;
using UnityEngine.SceneManagement;

public class ManagePlayerPrefs : MonoBehaviour
{
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Clear PlayerPrefs on scene change
        PlayerPrefs.DeleteKey("HitPlayAuto");
        PlayerPrefs.DeleteKey("ReviewAuto");
        PlayerPrefs.DeleteKey("DropAuto");
    }
}
