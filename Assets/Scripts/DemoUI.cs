using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private Button one;
    [SerializeField] private Button two;
    [SerializeField] private Button three;
    [SerializeField] private Button four;

    private static DemoUI _instance;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        panel.SetActive(false);
    }

    private void OnEnable()
    {
        one.onClick.AddListener(ChangeToScene1);
        two.onClick.AddListener(ChangeToScene2);
        three.onClick.AddListener(ChangeToScene3);
        four.onClick.AddListener(ChangeToScene4);
    }

    private void OnDisable()
    {
        one.onClick.RemoveListener(ChangeToScene1);
        two.onClick.RemoveListener(ChangeToScene2);
        three.onClick.RemoveListener(ChangeToScene3);
        four.onClick.RemoveListener(ChangeToScene4);
    }

    private void ChangeToScene1()
    {
        SceneManager.LoadScene(1);
        panel.SetActive(false);
    }

    private void ChangeToScene2()
    {
        SceneManager.LoadScene(2);
        panel.SetActive(false);
    }

    private void ChangeToScene3()
    {
        SceneManager.LoadScene(3);
        panel.SetActive(false);
    }

    private void ChangeToScene4()
    {
        SceneManager.LoadScene(0);
        panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
                return;
            }

            panel.SetActive(true);
        }
    }
}