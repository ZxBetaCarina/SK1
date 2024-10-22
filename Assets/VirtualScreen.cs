using AstekUtility.ServiceLocatorTool;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class VirtualScreen : GraphicRaycaster,IGameService
{
    [SerializeField] public Camera screenCamera; // Reference to the camera responsible for rendering the virtual screen's rendertexture

    public GraphicRaycaster screenCaster; // Reference to the GraphicRaycaster of the canvas displayed on the virtual screen
    public GameObject _objectSelected;


    protected override void Awake()
    {
        base.Awake();
        ServiceLocator.Instance.Register<VirtualScreen>(this);
    }

    protected override void OnDestroy()
    {
        ServiceLocator.Instance.Unregister<VirtualScreen>();
        base.OnDestroy();
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        Ray ray = eventCamera.ScreenPointToRay(eventData.position); // Mouse
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.transform == transform)
            {
                // Figure out where the pointer would be in the second camera based on texture position or RenderTexture.
                Vector3 virtualPos = new Vector3(hit.textureCoord.x, hit.textureCoord.y);
                virtualPos.x *= screenCamera.targetTexture.width;
                virtualPos.y *= screenCamera.targetTexture.height;

                eventData.position = virtualPos;

                screenCaster.Raycast(eventData, resultAppendList);
                _objectSelected = hit.collider.gameObject;
            }
        }
        Debug.Log(_objectSelected.name);
    }

}