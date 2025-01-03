using AstekUtility.ServiceLocatorTool;
using Gameplay;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PaytableCalculator : MonoBehaviour, IGameService
{
    [SerializeField] public PayTable_SO _paytable;
    [SerializeField] private List<TextMeshProUGUI> _rewardsInOrder;
    [SerializeField] private List<TextMeshProUGUI> _rewardFollowMeInOrder;

    private void Awake()
    {
        ServiceLocator.Instance.Register<PaytableCalculator>(this);
    }

    private void Start()
    {
        CalculatePaytableUIRewardAmount();
    }

    private void OnDestroy()
    {
        ServiceLocator.Instance.Unregister<PaytableCalculator>();
    }

    public float GetTimeForIcon(string spriteName)
    {
        foreach (SerializeDict dict in _paytable._paytableCore)
        {
            if (dict.Character == spriteName)
            {
                return dict.Paytable.Timer;
            }
        }
        return -1;
    }

    public bool IsIconTwoBetterThanIconOne(string name1, string name2)
    {
        float val1 = 0, val2 = 0;
        foreach (SerializeDict dict in _paytable._paytableCore)
        {
            if (dict.Character == name1)
            {
                val1 = dict.Paytable.RewardPercentage;
            }
            else if (dict.Character == name2)
            {
                val2 = dict.Paytable.RewardPercentage;
            }

            if (val1 != 0 && val2 != 0)
                break;
        }

        if (val2 > val1)
            return true;
        else
            return false;
    }

    /// <summary>
    /// Calculates Reward Given The Amount
    /// </summary>
    /// <returns></returns>
    public float CalcReward(string characterName, int amount)
    {
        if (CheckIfAmountIsAllowed(amount))
        {
            foreach (SerializeDict dict in _paytable._paytableCore)
            {
                if (dict.Character == characterName)
                {
                    return amount * dict.Paytable.RewardPercentage / 100;
                }
            }
        }
        return 0;
    }

    public (Icons sprite, RaycastOriginTransforms pattern) ReturnHighestValuePattern(List<(Icons, RaycastOriginTransforms)> possibilities)
    {
        int reward = 0;
        (Icons, RaycastOriginTransforms) currentAnswer = (null, null);

        foreach ((Icons, RaycastOriginTransforms) ans in possibilities)
        {
            foreach (SerializeDict dict in _paytable._paytableCore)
            {
                if (reward == 0 && ans.Item1.Name == dict.Character)
                {
                    reward = dict.Paytable.RewardPercentage;
                    currentAnswer = ans;
                }
                else if (ans.Item1.Name == dict.Character && dict.Paytable.RewardPercentage > reward)
                {
                    reward = dict.Paytable.RewardPercentage;
                    currentAnswer = ans;
                }
            }
        }

        return currentAnswer;
    }

    private bool CheckIfAmountIsAllowed(int amount)
    {
        if (!_paytable.AllowedAmount.Contains(amount))
        {
            Debug.LogError($"Invalid Amount Passed {amount}");
            return false;
        }
        else
        {
            return true;
        }
    }

    public void CalculatePaytableUIRewardAmount()
    {
        int i = 0;
        foreach (TextMeshProUGUI amountText in _rewardsInOrder)
        {
            float reward = (float)(GameController.Instance.GetPointsForRewardAtIndex(GameController.Instance.CurrentBetIndex) * _paytable._paytableCore[i].Paytable.RewardPercentage / 100);
            amountText.text = "PTS " + $"{reward}" + "\n" + " USD " + $"{reward/100}";
            i++;
        }
    }

    public void CalculatePaytableUIFollowpanel()
    {
        int i = 1;
        foreach (TextMeshProUGUI amountText in _rewardFollowMeInOrder)
        {
            int reward = (int)(GameController.Instance.GetPointsForRewardAtIndex(GameController.Instance.CurrentBetIndex) * (i*17.75f) / 100);
            amountText.text = $"{reward}";
            i++;
        }
    }
}
