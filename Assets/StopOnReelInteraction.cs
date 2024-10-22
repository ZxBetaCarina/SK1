using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StopOnReelInteraction : MonoBehaviour
{
    [SerializeField] private Button[] _buttons;

    //private void Update()
    //{
    //    if (!_rotatingStarted)
    //    {
    //        if (ImageCylinderSpawner.Instance.isRotating)
    //        {
    //            _rotatingStarted = true;
    //        }
    //    }
    //    else if (_rotatingStarted && !ImageCylinderSpawner.Instance.isRotating)
    //    {
    //        _stopperButtonParent.SetActive(false);
    //    }
    //}

    public void StopReel(int index)
    {
        ImageCylinderSpawner.Instance.StopCylinderAtIndex(index);
    }

    public void Activate()
    {
        foreach (Button button in _buttons)
        {
            button.interactable = true;
            button.gameObject.SetActive(true);
        }
    }

    public void Deactivate()
    {
        foreach (Button button in _buttons)
        {
            button.interactable = false;
            button.gameObject.SetActive(false);
        }
    }
}
