using AstekUtility.ServiceLocatorTool;
using RubicsCube;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager INSTANCE { get; private set; }

    public Button _spinButton;
    public GameObject _followPanel;
    public GameObject Demoshuffle;

    [SerializeField] private TextMeshProUGUI _winningText;
    [SerializeField] private GameObject _winningPanel;
    [SerializeField] private TMP_Text _message;
    [SerializeField] private TextMeshProUGUI _waitingText;
    [SerializeField] private List<GameObject> RubicControllers;
    [SerializeField] internal GameObject _revealbutton;
    // [SerializeField] private GameObject _muteButton;

    [SerializeField] private GameObject _skipPanel;

    [SerializeField] private Button _previewButton;
    [SerializeField] private Button _NormalPaytable;
    [SerializeField] private Button _FollowPaytable;
    [SerializeField] private Button _refreshButton;
    [SerializeField] private Button _instructionButton;
    [SerializeField] private Button _playAgainButton;
    [SerializeField] private GameObject _showRubicButton;
    [SerializeField] private GameObject _freeSpinImage;
    private string _waitPrefix = "Please Wait... ";
    private string _playAgainString = "Play Again";
    private string _winningMsg = "Got It!!";
    private string _losignMsg = "You Lose";
    [SerializeField] private float _winningPanelDelay = 5f;


    [Header("Rubics Mode")]
    [SerializeField] private GameObject cylinder;

    [SerializeField] private GameObject hideIconsParent;

    [SerializeField] private GameObject hideGlowyBit;
    [SerializeField] private GameObject submitCube;
    [SerializeField] private GameObject Quit;
    [SerializeField] private GameObject renderPanel;
    [SerializeField] private GameObject rubicsTimerBar;
    [SerializeField] private Image rubicsModeTimer;
    [SerializeField] private Gradient timerGradient;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip timerEndSounds;
    [SerializeField, Tooltip("Max time provided to solve the cube is in Min")] private float timeToSolveCubeInMin;
    [SerializeField] private ReadCube readCube;
    [SerializeField] private CubeState cubeState;
    public bool FollowMeCheck = false;

    [Header("Tic Tac Toe Functionality")]
    [SerializeField] private TicTacToeInteraction _interaction;

    public bool RubicMode { get; private set; }
    private int _movesLeft;

    public bool stopTimer = false;

    private void Awake()
    {
        if (INSTANCE == null)
        {
            INSTANCE = this;
        }

    }

    private void OnEnable()
    {
        GameController.BetConfirmed += OnBetConfirmed;
        FollowPanelTimer.TimerEnded += OnFollowTimerEnded;
        CheckForWinningPatterns.PatternNotFound += OnPatternNotFound;
        CheckForWinningPatterns.PatternFound += OnPatternFound;
    }

    private void OnDisable()
    {
        GameController.BetConfirmed -= OnBetConfirmed;
        FollowPanelTimer.TimerEnded -= OnFollowTimerEnded;
        CheckForWinningPatterns.PatternNotFound -= OnPatternNotFound;
        CheckForWinningPatterns.PatternFound -= OnPatternFound;
    }
    // Start is called before the first frame update
    IEnumerator Start()
    {
        readCube = FindObjectOfType<ReadCube>();
        cubeState = FindObjectOfType<CubeState>();

        //if (PlayerPrefs.HasKey("FollowMeCheck"))
        //{
        //    FollowMeCheck = PlayerPrefs.GetInt("FollowMeCheck") == 1; // Convert the saved int value to bool
        //    //PlayerPrefs.SetInt("FollowMeCheck", FollowMeCheck ? 0 : 1); // Convert bool to int
        //    //PlayerPrefs.Save();
        //}
        //else
        //{
        //    // If the key doesn't exist, set the default value
        //    FollowMeCheck = false;
        //    // Save the default value to PlayerPrefs
        //    PlayerPrefs.SetInt("FollowMeCheck", FollowMeCheck ? 1 : 0); // Convert bool to int
        //    PlayerPrefs.Save(); // Save changes
        //}
        foreach (var rubicBtn in RubicControllers)
        {
            rubicBtn.SetActive(false);
        }

        //_revealbutton.SetActive(false);
        _followPanel.SetActive(false);
        Demoshuffle.SetActive(true);
        _skipPanel.SetActive(false);
        _showRubicButton.SetActive(false);
        _spinButton.gameObject.SetActive(true);
        // StartCoroutine(nameof(DelaySpinButton));
        _refreshButton.interactable = true;
        _instructionButton.interactable = true;
        RubicMode = false;
        _movesLeft = 10;
        yield return null;
    }

    private void OnFollowTimerEnded()
    {
        StartCoroutine(SetWinningPanelActive(_losignMsg));
    }
    private void OnBetConfirmed()
    {
        _refreshButton.interactable = false;
        _instructionButton.interactable = false;
        readCube.ReadState();
    }
    public void ToggleBool()
    {
        if (!FollowMeCheck)
        {
            FollowMeCheck = !FollowMeCheck;
        }
        // Save the updated boolean value to PlayerPrefs
        PlayerPrefs.SetInt("FollowMeCheck", FollowMeCheck ? 1 : 0); // Convert bool to int
        PlayerPrefs.Save(); // Save changes
    }
    public void StartGameSetUp()
    {
        PlayerPrefs.DeleteKey("HitPlayAuto");
        PlayerPrefs.DeleteKey("ReviewAuto");
        PlayerPrefs.DeleteKey("DropAuto");
        SceneManager.LoadScene(1);
    }
    public void StartDemoshuffle()
    {
        PlayerPrefs.DeleteKey("HitPlayAuto");
        PlayerPrefs.DeleteKey("ReviewAuto");
        PlayerPrefs.DeleteKey("DropAuto");
        SceneManager.LoadScene(4);
    }

    private IEnumerator DelaySpinButton()
    {
        yield return new WaitForSeconds(3f);
        _spinButton.interactable = true;
    }

    public void CloseRubicMode()
    {
        RubicMode = false;
        _revealbutton.SetActive(false);
        foreach (var rubicBtn in RubicControllers)
        {
            rubicBtn.SetActive(false);
        }
    }

    private void OnPatternNotFound()
    {
        GameController.Instance.EndJackPotMode();
        if (RubicMode)
        {
            CloseRubicMode();
            _winningPanel.SetActive(true);

            print("Turn on WINNING PANEL");

            _waitingText.text = "Play Again";
            _message.text = $"Face Completed: {0}\n Reward: {0}";
            PlayerPrefs.SetFloat("Balance", GameController.Instance._currentPoints);
            return;
        }
        PlayerPrefs.SetFloat("Balance", GameController.Instance._currentPoints);
        RubicMode = false;
        //if (!CheckForWinningPatterns.INSTANCE.isBonus && _winningPanelDelay != 3)
            StartCoroutine(Turn_on_shuffle_cube_button());

        ImageCylinderSpawner.Instance.SpinAllowed = true;
        ToggleBool();
        CheckForWinningPatterns.INSTANCE._spin_btn_img.sprite = CheckForWinningPatterns.INSTANCE._Spin_Buttons[0];
        _instructionButton.interactable = true;

        _winningPanelDelay = 0f;
        //CheckForWinningPatterns.INSTANCE._reveal_button.gameObject.SetActive(true);
    }

    IEnumerator Turn_on_shuffle_cube_button()
    {
        yield return new WaitForSeconds(2);
        if (!CheckForWinningPatterns.INSTANCE.isBonus && _winningPanelDelay != 3)
        {
            _followPanel.SetActive(true);
            Demoshuffle.SetActive(true);
            _spinButton.interactable = true;
            _previewButton.interactable = false;
            _refreshButton.interactable = false;
        }        
    }

    public void ShowFreeSpin()
    {
        StartCoroutine(FreeSpinShow());
    }

    public IEnumerator FreeSpinShow()
    {
        _freeSpinImage.SetActive(true);
        print("Spin OFF Start Free Spin");
        _spinButton.gameObject.SetActive(false);
        _spinButton.interactable = false;
        yield return new WaitForSeconds(2f);
        _freeSpinImage.SetActive(false);
    }

    public void TurnOnHelperButtons()
    {
        foreach (var rubicBtn in RubicControllers)
        {
            rubicBtn.SetActive(true);
        }
        print("Spin OFF Turn On Helper Button");
        _spinButton.interactable = false;
        _refreshButton.interactable = false;
        //nstructionButton.interactable = false;
        _revealbutton.SetActive(true);
    }

    private void OnPatternFound(string characterName)
    {
        GameController.Instance.EndJackPotMode();
        CloseRubicMode();
        //_renderPanel.SetActive(false);
        //Highlight matchingPatterns
        _winningPanelDelay = 3f;

        print("Spin OFF On Pattern Found");
        _spinButton.interactable = false;
        _refreshButton.interactable = false;
        _instructionButton.interactable = false;
        RubicMode = false;
        Debug.Log(CheckForWinningPatterns.INSTANCE.isBonus);
        if (!CheckForWinningPatterns.INSTANCE.isBonus)
        {
            string msg;
            if (_interaction.AnswerSelectedMessage != null)
            {
                msg = _interaction.AnswerSelectedMessage;
            }
            else
            {
                msg = _winningMsg;
            }
            print("Enable Winning Panel" + msg);
            _winningPanel.SetActive(true);

            print("Turn on WINNING PANEL");

            _winningText.text = msg;
        }
        //StopCoroutine(WaitForUserInput());
    }

    private IEnumerator SetWinningPanelActive(string text)
    {
        _playAgainButton.interactable = false;
        yield return new WaitForSeconds(_winningPanelDelay);
        //  int _waitTimer = 15;
        print("Enable Winning Panel" + text);
        _winningPanel.SetActive(true);

        print("Turn on WINNING PANEL");

        // _winningText.text = text;
        //  _message.text = "";
        //while (_waitTimer > 0)
        //{
        //    _waitingText.text = _waitPrefix + _waitTimer + "s";
        //    _waitTimer--;
        //    yield return new WaitForSeconds(1f);
        //}
        _waitingText.text = _playAgainString;
        _playAgainButton.interactable = true;
    }

    public void OnClickFollowButton()
    {
        StopAllCoroutines();
        RubicMode = true;
        cylinder.SetActive(false);
        hideGlowyBit.SetActive(false);

        print("MASK Off");

        hideIconsParent.SetActive(false);
        submitCube.SetActive(true);
        Quit.SetActive(true);
        TurnOnHelperButtons();
        _showRubicButton.SetActive(true);
        _spinButton.interactable = false;
        _refreshButton.interactable = false;
        //nstructionButton.interactable = false;
        _NormalPaytable.gameObject.SetActive(false);
        _FollowPaytable.gameObject.SetActive(true);
        _followPanel.SetActive(false);
        Demoshuffle.SetActive(false);
        //TODO: skip panel was added to remove timer
        _skipPanel.SetActive(false);
    }

    public IEnumerator StartTimer()
    {
        rubicsTimerBar.SetActive(true);
        float netTime = timeToSolveCubeInMin * 60;
        float timeCounterInSec = timeToSolveCubeInMin * 60;

        while (timeCounterInSec > 0 && !stopTimer)
        {
            float fillPercentage = timeCounterInSec / netTime;
            rubicsModeTimer.fillAmount = fillPercentage;
            rubicsModeTimer.color = timerGradient.Evaluate(fillPercentage);
            timeCounterInSec = Mathf.Clamp((timeCounterInSec - Time.deltaTime), 0, netTime);
            switch ((int)timeCounterInSec)
            {
                case 5:
                    audioSource.volume = 0.5f;
                    audioSource.clip = timerEndSounds;
                    audioSource.Play();
                    break;

                case 4:
                    audioSource.volume = 0.6f;
                    break;

                case 3:
                    audioSource.volume = 0.7f;
                    break;

                case 2:
                    audioSource.volume = 0.8f;
                    break;

                case 1:
                    audioSource.volume = 0.9f;
                    break;

                case 0:
                    audioSource.volume = 1f;
                    break;
            }
            yield return null;
        }
        audioSource.Pause();

        if (!stopTimer)
            rubicsModeTimer.fillAmount = 0;
        //Run OnSubmit
        ServiceLocator.Instance.Get<RubikCubeController>().OnSubmit();
    }

    public void ReloadScene(int index)
    {
        SceneManager.LoadScene(index);
    }
}
