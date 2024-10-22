using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FollowPanelTimer : MonoBehaviour
{
    //private float _timer;
    [SerializeField] private TMP_Text _timerText;
    [SerializeField] private Button _followButon;
    public static Action TimerEnded;
    private bool TimerEndInvoked = false;
    // Start is called before the first frame update
    private void OnEnable()
    {
        //_timer = 35 - ((1 + GameController.Instance.CurrentBetIndex) * 5);
        _followButon = GetComponent<Button>();
        TimerEndInvoked = false;

        //transform.DOScale(1, 2).SetLoops(-1,LoopType.Yoyo);
    }

}
