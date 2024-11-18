using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System;
using AstekUtility.ServiceLocatorTool;
using RubicsCube;
using Gameplay;

public class GameController : MonoBehaviour
{
    public float _reveal_time;
    
    public static GameController INSTANCE { get; private set; }

    public GameObject targetObject;  // Reference to the GameObject to be disabled/enabled
    public Button disableButton;     // Reference to the UI button for disabling
    public TMP_Text timerText;           // Reference to the UI text for displaying the timer
    public GameObject revealFX;
    [SerializeField] private PayTable_SO paytable;
    [SerializeField] private GameObject _winningFX;
    [SerializeField] private GameObject _winningAmountPanel;
    [SerializeField] private TextMeshProUGUI winningAmountText;
    private bool isButtonDisabled = false;
    private float disableDuration = 60f;
    private float timer;
    [SerializeField] private TextMeshProUGUI _bettingInput;
    // [SerializeField] private TextMeshProUGUI CoolTimer;

    [SerializeField] public Button _increaseBetButton;
    [SerializeField] public Button _maxBet;
    [SerializeField] public Button _minBet;
    [SerializeField] public Button _decreaseBetButton;
    [SerializeField] private TextMeshProUGUI _totalBet;
    [SerializeField] private TextMeshProUGUI _currentPointsText;
    [SerializeField] private GameObject coinImages;
    [SerializeField] private GameObject _coinFx;
    [SerializeField] private Button NormalPaytable;
    [SerializeField] private Button FollowPaytable;
    [SerializeField] private GameObject _pattern_FX;
    [SerializeField] private float _normalWinningMultiple;
    [SerializeField] private float _rubicModeWinningMultiple;
    [SerializeField] private float _jackPotModeWinningMultiple;
    [SerializeField] private AudioSource _winningAudio;
    [SerializeField] private GameObject _jackpotModepanel;
    [SerializeField] internal GameObject _exit_button;


    [HideInInspector] public List<Vector3> _patterns = new List<Vector3>();
    public List<string> _patternFormed = new List<string>();


    public bool IsRevealed = false;

    private readonly string[] _bettingAmountUSD = { "0.1", "0.5", "1.0", "1.5", "2.0", "2.5" };
    private readonly int[] _betPoints = { 10, 50, 100, 150, 200, 250 };
    private int _initialBet;
    public int CurrentBetIndex { get; private set; } = 0;
    public float _currentPoints = 500f;
    [SerializeField] private TMP_Text _availableCredits;
    public static Action BetChanged;
    public static Action BetConfirmed;
    public bool JackPotMode;

    internal List<GameObject> _highlight_objects = new();


    public void AddPatternPositions(Vector3 position)
    {
        _patterns.Add(position);
    }
    public static GameController Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        CurrentBetIndex = 0;
    }

    private void OnEnable()
    {
        CheckForWinningPatterns.PatternFound += OnPatternFound;
    }

    public void OnDisable()
    {
        CheckForWinningPatterns.PatternFound -= OnPatternFound;
    }

    public void Revealed()
    {
        IsRevealed = true;
    }

    void Start()
    {
        JackPotMode = false;
        _jackpotModepanel.SetActive(false);
        disableButton.onClick.AddListener(DisableObject);
        InitiateBet();
        AvailableCredit();
        if (PlayerPrefs.HasKey("Balance"))
        {
            _currentPoints = PlayerPrefs.GetFloat("Balance");
            _currentPointsText.text = _currentPoints + "Pts";
        }
        if (CurrentBetIndex == 0)
        {
            _minBet.interactable = false;
            _maxBet.interactable = true;
        }
        else if (CurrentBetIndex == 5)
        {
            _maxBet.interactable = false;
            _minBet.interactable = true;
        }
        else
        {
            _maxBet.interactable = true;
            _minBet.interactable = true;
        }
        Debug.Log(_currentPoints);
        CurrentBetIndex = CurrentBetIndex;
        _currentPointsText.text = _currentPoints + "Pts";

    }
    

    void Update()
    {
        if (isButtonDisabled)
        {
            timer -= Time.deltaTime;

            // Update timer text
            timerText.text = "Cooldown: " + Mathf.Ceil(timer).ToString();

            if (timer <= 0f)
            {
                // Enable the button after the cooldown
                isButtonDisabled = false;
                disableButton.interactable = true;
                timerText.text = "";
            }
        }
        
    }

    /// <summary>
    /// Return the points associated with respective reward index
    /// </summary>
    /// <param name="index"></param>
    public float GetPointsForRewardAtIndex(int index)
    {
        return _betPoints[index];
    }

    void AvailableCredit()
    {
        int roundedValue = Mathf.FloorToInt(_currentPoints);

        // Display the rounded value
        if (_availableCredits != null)
        {
            _availableCredits.text = roundedValue.ToString() + "Pts";
        }
        else
        {
            Debug.LogWarning("Display Text reference not set.");
        }

        // Alternatively, you can simply print it out
        //Debug.Log("Nearest Small Whole Number:  " + roundedValue);
    }
    void DisableObject()
    {
        // Disable the target object
        targetObject.SetActive(false);
        revealFX.SetActive(true);
        // Enable it back after 3 seconds
        StartCoroutine(EnableObjectAfterDelay(3f));

        // Disable the button for the cooldown duration
        disableButton.interactable = false;
        isButtonDisabled = true;
        timer = disableDuration;

        // Start the countdown timer
        StartCoroutine(CountdownTimer());
    }

    IEnumerator EnableObjectAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        targetObject.SetActive(true);
        revealFX.SetActive(false);
    }

    IEnumerator CountdownTimer()
    {
        while (timer > 0)
        {
            yield return null;
        }
    }

    private void OnPatternFound(string character)
    {
        _winningAudio.Play();
        _coinFx.SetActive(true);
        ImageCylinderSpawner.Instance.won = true;
        foreach (Vector3 pos in _patterns)
        {
            GameObject fx = Instantiate(_pattern_FX, pos, Quaternion.identity);
            _highlight_objects.Add(fx);
            //Destroy(fx, 3f);
        }

        if (UIManager.INSTANCE.RubicMode)
        {
            (float, int) rewards = ServiceLocator.Instance.Get<RubikCubeController>().RewardPercentage();
            //float winningAmount = (int)(rewards.Item1 * _betPoints[CurrentBetIndex] / 100);
            float cubeReward = (float)Math.Ceiling(rewards.Item1 / 100 * float.Parse(_bettingAmountUSD[CurrentBetIndex]));
            winningAmountText.text = $"Face Completed: {rewards.Item2}\n Reward: {cubeReward}";
            _currentPoints += cubeReward;
        }
        //else if (JackPotMode)
        //{
        //    float winningAmount = ServiceLocator.Instance.Get<PaytableCalculator>().CalcReward(character, _betPoints[CurrentBetIndex]);
        //    //winningAmountText.text = "Reward: " + winningAmount.ToString();
        //    _currentPoints += winningAmount;
        //    JackPotMode = false;
        //}
        else
        {
            if (_patternFormed.Count > 0 && CheckForWinningPatterns.INSTANCE.isBonus)
            {
                foreach (string name in _patternFormed)
                {
                    float cubeReward = ServiceLocator.Instance.Get<PaytableCalculator>().CalcReward(name, _betPoints[CurrentBetIndex]);
                    _currentPoints += cubeReward;
                }
                _patternFormed.Clear();
            }
            else
            {
                float cubeReward = ServiceLocator.Instance.Get<PaytableCalculator>().CalcReward(character, _betPoints[CurrentBetIndex]); /*= _normalWinningMultiple * _betPoints[CurrentBetIndex] / 100*/
                if (!winningAmountText.gameObject.activeInHierarchy)
                {
                    winningAmountText.text = "Reward: " + cubeReward.ToString() + " PTS";
                    _currentPoints += cubeReward;
                }
            }

        }
        _currentPointsText.text = _currentPoints + "Pts";
        PlayerPrefs.SetFloat("Balance", _currentPoints);
        AvailableCredit();
        Invoke(nameof(DisableCoinFx), 3f);
        GameObject winningFx = Instantiate(_winningFX);
        Destroy(winningFx, 4f);

        _patterns = new();
        print(1234);
        
        

    }


    public void FinaliseBetOnClickSpin()
    {
        if (_currentPoints >= _currentPoints - _betPoints[CurrentBetIndex / 100])
        {
            if (_currentPoints - _betPoints[CurrentBetIndex] / 100 < 0) return;
            _currentPoints -= _betPoints[CurrentBetIndex] / 1;
            _currentPointsText.text = _currentPoints + "Pts";
            AvailableCredit();
            _increaseBetButton.interactable = false;
            _decreaseBetButton.interactable = false;
            _maxBet.interactable = false;
            _minBet.interactable = false;
            CheckForWinningPatterns.INSTANCE.FinaliseBet();

            BetConfirmed?.Invoke();

            PlayerStats.Instance.SetBetAmount(_betPoints[CurrentBetIndex]);
            PlayerPrefs.SetInt("LastBetIndex",CurrentBetIndex);
        }

    }

    public void OnClickReviewButton()
    {
        CheckForWinningPatterns.INSTANCE.ReviewImages(true);
    }

    public void StartJackPotMode()
    {
        _jackpotModepanel.SetActive(true);
    }

    public void JackPotWinning()
    {
        Debug.Log("JackPot!!!");

    }

    public void EndJackPotMode()
    {
        JackPotMode = false;
        _jackpotModepanel.SetActive(false);
    }


    private void DisableCoinFx()
    {
        _coinFx.SetActive(false);
    }

    public void IncreaseBet()
    {
        if (!ImageCylinderSpawner.Instance.CylinderSpawning)
        {
            if (CurrentBetIndex < 5 && CurrentBetIndex >= 0)
            {
                CurrentBetIndex++;
                Mathf.Clamp(CurrentBetIndex, 0, 4);
                _bettingInput.text = _bettingAmountUSD[CurrentBetIndex] + "$";
                _totalBet.text = _betPoints[CurrentBetIndex] + "Pts";
                CheckForWinningPatterns.INSTANCE.ReviewImages(false);
                BetChanged?.Invoke();
                //Debug.Log(CurrentBetIndex);
                if (CurrentBetIndex == 0)
                {
                    _minBet.interactable = false;
                    _maxBet.interactable = true;
                }
                else if (CurrentBetIndex == 5)
                {
                    _maxBet.interactable = false;
                    _minBet.interactable = true;
                }
                else
                {
                    _maxBet.interactable = true;
                    _minBet.interactable = true;
                }
            }
        }
    }

    public void maxBet()
    {
        if (!ImageCylinderSpawner.Instance.CylinderSpawning)
        {
            CurrentBetIndex = 5;
            _bettingInput.text = _bettingAmountUSD[CurrentBetIndex] + "$";
            _totalBet.text = _betPoints[CurrentBetIndex] + "Pts";
            CheckForWinningPatterns.INSTANCE.ReviewImages(false);
            BetChanged?.Invoke();
            _maxBet.interactable = false;
            _minBet.interactable = true;

        }
    }

    public void minBet()
    {
        if (!ImageCylinderSpawner.Instance.CylinderSpawning)
        {
            CurrentBetIndex = 0;
            _bettingInput.text = _bettingAmountUSD[CurrentBetIndex] + "$";
            _totalBet.text = _betPoints[CurrentBetIndex] + "Pts";
            CheckForWinningPatterns.INSTANCE.ReviewImages(false);
            BetChanged?.Invoke();
            _minBet.interactable = false;
            _maxBet.interactable = true;

        }
    }
    public void DecreaseBet()
    {
        if (!ImageCylinderSpawner.Instance.CylinderSpawning)
        {
            if (CurrentBetIndex <= 5 && CurrentBetIndex > 0)
            {
                CurrentBetIndex--;
                Mathf.Clamp(CurrentBetIndex, 0, 4);
                _bettingInput.text = _bettingAmountUSD[CurrentBetIndex] + "$";
                _totalBet.text = _betPoints[CurrentBetIndex] + "Pts";
                CheckForWinningPatterns.INSTANCE.ReviewImages(false);
                BetChanged?.Invoke();
            }
            if (CurrentBetIndex == 0)
            {
                _minBet.interactable = false;
                _maxBet.interactable = true;
            }
            else if (CurrentBetIndex == 5)
            {
                _maxBet.interactable = false;
                _minBet.interactable = true;
            }
            else
            {
                _maxBet.interactable = true;
                _minBet.interactable = true;
            }
        }
    }

    public void InitiateBet()
    {
        AvailableCredit();
        CurrentBetIndex = PlayerPrefs.GetInt("LastBetIndex", 0);
        _increaseBetButton.interactable = true;
        _decreaseBetButton.interactable = true;
        //_maxBet.interactable = true;
        _bettingInput.text = _bettingAmountUSD[CurrentBetIndex] + "$";
        _totalBet.text = _betPoints[CurrentBetIndex] + "Pts";
        _currentPointsText.text = _currentPoints + "Pts";
        //NormalPaytable.gameObject.SetActive(true);
        FollowPaytable.gameObject.SetActive(false);
        timer = 0;
        //timerText.text = " ";

    }
    private void OnApplicationQuit()
    {
        PlayerPrefs.DeleteKey("LastBetIndex");
    }
}
