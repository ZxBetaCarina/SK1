using DG.Tweening;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimateButton : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private string string1;
    [SerializeField] private string string2;
    [SerializeField] private string string3;

    private void OnValidate()
    {
        if (text == null) text = GetComponent<TMP_Text>();
    }

    private void OnEnable()
    {
        transform.DOScale(.9f, 1).SetLoops(-1, LoopType.Yoyo);
        RandomTextSetter();
    }

    private void RandomTextSetter()
    {
        var randomInt = Random.Range(0, 3);
        Debug.Log("==============================================Random Int"+randomInt);
        switch (randomInt)
        {
            case 0:
                text.text = string1;
                break;
            case 1:
                text.text = string2;
                break;
            case 2:
                text.text = string3;
                break;
        }
    }
}
