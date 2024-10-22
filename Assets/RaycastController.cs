using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RaycastController : MonoBehaviour
{
    private VirtualScreen _virtualScreen;
    [SerializeField] private GraphicRaycaster _canvasGraphicsRaycaster;
    [SerializeField] private GameObject renderpanel;

    private void Awake()
    {
        _virtualScreen = GetComponent<VirtualScreen>();
    }

    private void Update()
    {
        if (!renderpanel.activeInHierarchy)
        {
             _virtualScreen.enabled = false;
            _canvasGraphicsRaycaster.enabled = true;
        }
        else
        {
            _virtualScreen.enabled = true;
            _canvasGraphicsRaycaster.enabled = false;
        }
    }
}
