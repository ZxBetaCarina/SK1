using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DemoUI : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject exit;
    [SerializeField] private Button one;
    [SerializeField] private Button two;
    [SerializeField] private Button three;
    [SerializeField] private Button four;

    private static DemoUI _instance;

    private void Awake()
    {
      //  if (_instance == null)
        //{
          //  _instance = this;
            //DontDestroyOnLoad(gameObject);
        //}
        //else
        //{
          //  Destroy(gameObject);
        //}
    }

    private void Start()
    {
        panel.SetActive(false);
        exit.SetActive(false);
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
        SceneManager.LoadScene(5);
        panel.SetActive(false);
        exit.SetActive(false);
    }

    private void ChangeToScene2()
    {
        SceneManager.LoadScene(6);
        panel.SetActive(false);
        exit.SetActive(false);
    }

    private void ChangeToScene3()
    {
        SceneManager.LoadScene(7);
        panel.SetActive(false);
        exit.SetActive(false);
    }

    private void ChangeToScene4()
    {
        SceneManager.LoadScene(4);
        panel.SetActive(false);
        exit.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            if (panel.activeSelf)
            {
                panel.SetActive(false);
                exit.SetActive(false);
                return;
            }

            panel.SetActive(true);
            exit.SetActive(true);
        }
    }
    public void shufflethecube()
    {
        SceneManager.LoadScene(4);
        panel.SetActive(false);
        exit.SetActive(false);
    }
    public void ExitBtn()
    {
        panel.SetActive(false);
        exit.SetActive(false);
    }
    
}