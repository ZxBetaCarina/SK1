using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateButton : MonoBehaviour
{
    private void OnEnable()
    {
        transform.DOScale(.9f, 1).SetLoops(-1, LoopType.Yoyo);
    }
}
