using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private Player player;
    [SerializeField] private Text scoreText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text pointsEarnedText;
    [SerializeField] private GameObject playButton;
    [SerializeField] private GameObject gameOver;
    [SerializeField] private GameObject backToHomeBtn;
    [SerializeField] private GameObject _difficulty_ui;
    [SerializeField] private GameObject _conversion_ui;

    private int score;
    public int Score => score;

    internal int _difficulty_state;
    private int scoremultiplier = 1;

    private void Awake()
    {
        if (Instance != null)
        {
            DestroyImmediate(gameObject);
        }
        else
        {
            Instance = this;
            Application.targetFrameRate = 60;
            Pause();
        }
        gameOver.SetActive(false);
        pointsEarnedText.gameObject.SetActive(false);
        pointsText.gameObject.SetActive(false);
        backToHomeBtn.SetActive(false);
    }
    

    public void Play()
    {
        score = 0;
        scoreText.text = score.ToString();

        playButton.SetActive(false);
        gameOver.SetActive(false);

        player.enabled = true;
        player.SetPlayerState = PlayerState.Running;

        Pipes[] pipes = FindObjectsOfType<Pipes>();

        for (int i = 0; i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    public void GameOver()
    {
        //playButton.SetActive(true);
        gameOver.SetActive(true);

        pointsText.text = Math.Round(score * .01f,2) + " $";

        float points = PlayerPrefs.GetFloat("Balance");
        points = points + score * .01f;

        PlayerPrefs.SetFloat("Balance", PlayerPrefs.GetFloat("Balance") + score);

        pointsEarnedText.gameObject.SetActive(true);
        pointsText.gameObject.SetActive(true);
        backToHomeBtn.SetActive(true);


        Pause();
    }


    public void LoadLevel1()
    {
        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        player.SetPlayerState = PlayerState.Paused;
        player.enabled = false;
    }

    public void IncreaseScore()
    {
        score+= scoremultiplier;
        scoreText.text = score.ToString();
    }

    public void Easy()
    {
        _difficulty_ui.SetActive(false);
        _conversion_ui.SetActive(true);
        _difficulty_state = 0;
        scoremultiplier = 3;
    }

    public void Medium()
    {
        _difficulty_ui.SetActive(false);
        _conversion_ui.SetActive(true);
        _difficulty_state = 1;
        scoremultiplier = 5;
    }

    public void Hard()
    {
        _difficulty_ui.SetActive(false);
        _conversion_ui.SetActive(true);
        _difficulty_state = 2;
        scoremultiplier = 10;
    }
}
