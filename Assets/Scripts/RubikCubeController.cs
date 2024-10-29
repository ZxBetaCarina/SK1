using AstekUtility.ServiceLocatorTool;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace RubicsCube
{
    [Serializable]
    public class ImagesStrings
    {
        public string name;
        public Sprite sprite;
    }

    public enum CubeSides
    {
        Front,
        Back,
        Left,
        Right,
        Up,
        Down
    }

    public class RubikCubeController : MonoBehaviour, IGameService
    {

        [SerializeField] private List<Transform> originTransforms;
        [SerializeField] private Vector3 _positionOfEstimatedFrontPosition;

        [Header("Rewards")]
        [SerializeField] private PayTable_SO _paytable;
        [SerializeField] private GameObject _submitCubeButton;

        [Header("Slot and cube images")]
        #region SlotImages

        [SerializeField] private List<Sprite> spriteCollection = new();
        [SerializeField] private List<SpriteRenderer> upLayer1 = new();
        [SerializeField] private List<SpriteRenderer> upLayer2 = new();
        [SerializeField] private List<SpriteRenderer> upLayer3 = new();
        [SerializeField] private List<SpriteRenderer> upLayer4 = new();
        [SerializeField] private List<SpriteRenderer> downLayer1 = new();
        [SerializeField] private List<SpriteRenderer> downLayer2 = new();
        [SerializeField] private List<SpriteRenderer> downLayer3 = new();
        [SerializeField] private List<SpriteRenderer> downLayer4 = new();
        [SerializeField] private List<SpriteRenderer> leftLayer1 = new();
        [SerializeField] private List<SpriteRenderer> leftLayer2 = new();
        [SerializeField] private List<SpriteRenderer> leftLayer3 = new();
        [SerializeField] private List<SpriteRenderer> leftLayer4 = new();
        [SerializeField] private List<SpriteRenderer> rightLayer1 = new();
        [SerializeField] private List<SpriteRenderer> rightLayer2 = new();
        [SerializeField] private List<SpriteRenderer> rightLayer3 = new();
        [SerializeField] private List<SpriteRenderer> rightLayer4 = new();
        [SerializeField] private List<SpriteRenderer> centerVertical1 = new();
        [SerializeField] private List<SpriteRenderer> centerVertical2 = new();
        [SerializeField] private List<SpriteRenderer> centerVertical3 = new();
        [SerializeField] private List<SpriteRenderer> centerVertical4 = new();
        [SerializeField] private List<SpriteRenderer> centerHorizontal1 = new();
        [SerializeField] private List<SpriteRenderer> centerHorizontal2 = new();
        [SerializeField] private List<SpriteRenderer> centerHorizontal3 = new();
        [SerializeField] private List<SpriteRenderer> centerHorizontal4 = new();
        [SerializeField] private List<SpriteRenderer> frontSprites = new();
        #endregion

        #region CubeImages

        [SerializeField] private List<SpriteRenderer> CubeUpLayer1 = new();
        [SerializeField] private List<SpriteRenderer> CubeUpLayer2 = new();
        [SerializeField] private List<SpriteRenderer> CubeUpLayer3 = new();
        [SerializeField] private List<SpriteRenderer> CubeUpLayer4 = new();
        [SerializeField] private List<SpriteRenderer> CubeDownLayer1 = new();
        [SerializeField] private List<SpriteRenderer> CubeDownLayer2 = new();
        [SerializeField] private List<SpriteRenderer> CubeDownLayer3 = new();
        [SerializeField] private List<SpriteRenderer> CubeDownLayer4 = new();
        [SerializeField] private List<SpriteRenderer> CubeLeftLayer1 = new();
        [SerializeField] private List<SpriteRenderer> CubeLeftLayer2 = new();
        [SerializeField] private List<SpriteRenderer> CubeLeftLayer3 = new();
        [SerializeField] private List<SpriteRenderer> CubeLeftLayer4 = new();
        [SerializeField] private List<SpriteRenderer> CubeRightLayer1 = new();
        [SerializeField] private List<SpriteRenderer> CubeRightLayer2 = new();
        [SerializeField] private List<SpriteRenderer> CubeRightLayer3 = new();
        [SerializeField] private List<SpriteRenderer> CubeRightLayer4 = new();
        [SerializeField] private List<SpriteRenderer> CubeCenterVertical1 = new();
        [SerializeField] private List<SpriteRenderer> CubeCenterVertical2 = new();
        [SerializeField] private List<SpriteRenderer> CubeCenterVertical3 = new();
        [SerializeField] private List<SpriteRenderer> CubeCenterVertical4 = new();
        [SerializeField] private List<SpriteRenderer> CubeCenterHorizontal1 = new();
        [SerializeField] private List<SpriteRenderer> CubeCenterHorizontal2 = new();
        [SerializeField] private List<SpriteRenderer> CubeCenterHorizontal3 = new();
        [SerializeField] private List<SpriteRenderer> CubeCenterHorizontal4 = new();
        [SerializeField] private List<SpriteRenderer> CubeFrontFace = new();
        #endregion

        #region Faces

        [Header("Faces")]
        [SerializeField] private List<FaceToSprite> _faceAndRenderers;

        #endregion


        private List<SpriteRenderer> _detected = new();

        [SerializeField] private List<ImagesStrings> imagesStrings = new();
        [SerializeField] private List<GameObject> _selectedFace = new();
        [SerializeField] private SelectFace _cubeSelectFace;
        [SerializeField] private CubeState _cubeState;
        [SerializeField] private ReadCube _readCube;

        //TODO:This is a temporary measure and only meant until we fix the timing issues
        [SerializeField] private GameObject _shufflingPanel;

        public List<SpriteRenderer> spriteRenderers;
        public List<SpriteRenderer> OldSprites;
        public List<Vector3> spritePositions;
        public List<GameObject> newSpriteObjects;
        public Transform originalParent;
        public Transform parentTransform;
        public float rotationSpeed = 30f; // Adjust this value as needed
        public float targetRotation = -90f; // Target rotation angle
        private bool shouldRotate = false;

        private int activeUpLayer = 0;
        private int activeDownLayer = 0;
        private int activeLeftLayer = 0;
        private int activeRightLayer = 0;
        private int activeCenterVerticalLayer = 0;
        private int activeCenterHorizontalLayer = 0;

        private bool _isShuffling = true;

        private void Awake()
        {
            ServiceLocator.Instance.Register<RubikCubeController>(this);
        }

        private void OnDestroy()
        {
            ServiceLocator.Instance.Unregister<RubikCubeController>();
        }

        private void OnEnable()
        {
            GameController.BetConfirmed += OnBetConfirmed;
        }

        private void OnDisable()
        {
            GameController.BetConfirmed -= OnBetConfirmed;
        }
        private void Start()
        {
            foreach (SpriteRenderer s in frontSprites)
            {
                s.gameObject.SetActive(false);
            }

        }

        private void UpdateCubeFrontFace()
        {
            for (int i = 0; i < CubeFrontFace.Count; i++)
            {
                //if (_cubeState.front[i].transform.GetChild(0).GetComponent<SpriteRenderer>() != null) Debug.Log("found");
                frontSprites[i].sprite = _cubeState.Front[i].GetComponentInChildren<Icons>().GetSprite();
                foreach (ImagesStrings iS in imagesStrings)
                {
                    if (frontSprites[i].sprite.name == iS.sprite.name)
                    {
                        frontSprites[i].gameObject.name = iS.name;
                    }
                }
            }
        }


        private void OnBetConfirmed()
        {
            Invoke(nameof(UpdateCubeFrontFaceList), .2f);
        }

        private void UpdateCubeFrontFaceList()
        {
            for (int i = 0; i < _cubeState.Front.Count; i++)
            {
                CubeFrontFace[i] = _cubeState.Front[i].transform.GetChild(0).GetComponent<SpriteRenderer>();
            }
        }



        IEnumerator InvokeRubicMoved()
        {
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            yield return new WaitForSeconds(.2f);
            ////Note:
            ////Wait because cube rotation is not done yet

            _readCube.ReadState();


            //yield return new WaitForEndOfFrame();
            //yield return new WaitForEndOfFrame();
            //UpdateCubeFrontFace();
            //yield return null;
        }


        #region Move Rubic Faces

        /// <summary>
        /// pass Face and int dir= 1 for increase and -1 for decrease
        /// </summary>
        /// <param name="face"></param>
        /// <param name="dir"></param>
        public void RotateSide(Faces face, int dir)
        {
            switch (face)
            {
                case Faces.Front:

                    if (dir > 0)
                    {
                        IncreaseFrontLayer();
                    }
                    else
                    {
                        DecreaseFrontLayer();
                    }

                    break;

                case Faces.Back:

                    if (dir > 0)
                    {
                        IncreaseBackLayer();
                    }
                    else
                    {
                        DecreaseBackLayer();
                    }

                    break;

                case Faces.Up:

                    if (dir > 0)
                    {
                        IncreaseUpLayer();
                    }
                    else
                    {
                        DecreaseUpLayer();
                    }

                    break;

                case Faces.Down:

                    if (dir > 0)
                    {
                        IncreaseDownLayer();
                    }
                    else
                    {
                        DecreaseDownLayer();
                    }

                    break;

                case Faces.Left:

                    if (dir > 0)
                    {
                        IncreaseLeftLayer();
                    }
                    else
                    {
                        DecreaseLeftLayer();
                    }

                    break;

                case Faces.Right:

                    if (dir > 0)
                    {
                        IncreaseRightLayer();
                    }
                    else
                    {
                        DecreaseLeftLayer();
                    }

                    break;
            }
        }

        private bool inProgress = false;
        public void IncreaseUpLayer()
        {

            _readCube.ReadState();
            _selectedFace = _cubeState.Up;
            _cubeSelectFace.RotateSide(_cubeState.Up, 1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void IncreaseDownLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Down;
            _cubeSelectFace.RotateSide(_cubeState.Down, 1);
            StartCoroutine(InvokeRubicMoved());
        }

        public void IncreaseLeftLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Left;
            _cubeSelectFace.RotateSide(_cubeState.Left, 1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void IncreaseRightLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Right;
            _cubeSelectFace.RotateSide(_cubeState.Right, 1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void IncreaseCenterVertical()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.CenterVertical;
            _cubeSelectFace.RotateSide(_cubeState.CenterVertical, -1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void IncreaseCenterHorizontal()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.CenterHorizontal;
            _cubeSelectFace.RotateSide(_cubeState.CenterHorizontal, 1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void DecreaseUpLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Up;
            _cubeSelectFace.RotateSide(_cubeState.Up, -1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void DecreaseDownLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Down;
            _cubeSelectFace.RotateSide(_cubeState.Down, -1);
            StartCoroutine(InvokeRubicMoved());
        }

        public void DecreaseLeftLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Left;
            _cubeSelectFace.RotateSide(_cubeState.Left, -1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void DecreaseRightLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Right;
            _cubeSelectFace.RotateSide(_cubeState.Right, -1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void DecreaseCenterVertical()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.CenterVertical;
            _cubeSelectFace.RotateSide(_cubeState.CenterVertical, 1);
            StartCoroutine(InvokeRubicMoved());
        }
        public void DecreaseCenterHorizontal()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.CenterHorizontal;
            _cubeSelectFace.RotateSide(_cubeState.CenterHorizontal, -1);
            StartCoroutine(InvokeRubicMoved());
        }

        public void IncreaseFrontLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Front;
            _cubeSelectFace.RotateSide(_cubeState.Front, 1);
            StartCoroutine(InvokeRubicMoved());
        }

        public void DecreaseFrontLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Front;
            _cubeSelectFace.RotateSide(_cubeState.Front, -1);
            StartCoroutine(InvokeRubicMoved());
        }

        public void IncreaseBackLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Back;
            _cubeSelectFace.RotateSide(_cubeState.Back, 1);
            StartCoroutine(InvokeRubicMoved());
        }

        public void DecreaseBackLayer()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.Back;
            _cubeSelectFace.RotateSide(_cubeState.Back, -1);
            StartCoroutine(InvokeRubicMoved());
        }

        public void IncreaseCenterCenter()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.CenterCenter;
            _cubeSelectFace.RotateSide(_cubeState.CenterCenter, 1);
            StartCoroutine(InvokeRubicMoved());
        }

        public void DecreaseCenterCenter()
        {
            _readCube.ReadState();
            _selectedFace = _cubeState.CenterCenter;
            _cubeSelectFace.RotateSide(_cubeState.CenterCenter, -1);
            StartCoroutine(InvokeRubicMoved());
        }

        #endregion

        private void InitiateLayers()
        {
            AddToLayerList(upLayer2, CubeUpLayer2);
            AddToLayerList(upLayer3, CubeUpLayer3);
            AddToLayerList(upLayer4, CubeUpLayer4);
            AddToLayerList(downLayer2, CubeDownLayer2);
            AddToLayerList(downLayer3, CubeDownLayer3);
            AddToLayerList(downLayer4, CubeDownLayer4);
            AddToLayerList(leftLayer2, CubeLeftLayer2);
            AddToLayerList(leftLayer3, CubeLeftLayer3);
            AddToLayerList(leftLayer4, CubeLeftLayer4);
            AddToLayerList(rightLayer2, CubeRightLayer2);
            AddToLayerList(rightLayer3, CubeRightLayer3);
            AddToLayerList(rightLayer4, CubeRightLayer4);
            AddToLayerList(centerHorizontal2, CubeCenterHorizontal2);
            AddToLayerList(centerHorizontal3, CubeCenterHorizontal3);
            AddToLayerList(centerHorizontal4, CubeCenterHorizontal4);
            AddToLayerList(centerVertical2, CubeCenterVertical2);
            AddToLayerList(centerVertical3, CubeCenterVertical3);
            AddToLayerList(centerVertical4, CubeCenterVertical4);
        }

        public void HideRubic()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void AddToLayerList(List<SpriteRenderer> addToSlotList, List<SpriteRenderer> addToCubeList)
        {
            for (int i = 0; i < 3; i++)
            {
                //Just assign sprites It doesn't needed to setActive()
                int random = UnityEngine.Random.Range(0, 7 /*+ GameController.Instance.CurrentBetIndex*/);
                addToSlotList[i].sprite = imagesStrings[random].sprite;
                addToCubeList[i].sprite = imagesStrings[random].sprite;
                addToSlotList[i].gameObject.name = imagesStrings[random].name;

                //set the name so that winning condition can be satisfied

                //addToList[i].transform.gameObject.SetActive(true);
            }
        }
        public async void DetectSprite()
        {
            _shufflingPanel.SetActive(true);
            int randomShuffleAmount = UnityEngine.Random.Range(10, 20);
            for (int i = 0; i < randomShuffleAmount; i++)
            {
                //Gaurd clause to prevent memory leaks
                if (!Application.isPlaying)
                    return;

                int selectOperation = UnityEngine.Random.Range(0, 13);
                switch (selectOperation)
                {
                    case 0:
                        IncreaseUpLayer();
                        break;

                    case 1:
                        IncreaseRightLayer();
                        break;

                    case 2:
                        IncreaseLeftLayer();
                        break;

                    case 3:
                        IncreaseDownLayer();
                        break;

                    case 4:
                        IncreaseCenterHorizontal();
                        break;

                    case 5:
                        IncreaseCenterVertical();
                        break;

                    case 6:
                        DecreaseUpLayer();
                        break;

                    case 7:
                        DecreaseRightLayer();
                        break;

                    case 8:
                        DecreaseLeftLayer();
                        break;

                    case 9:
                        DecreaseDownLayer();
                        break;

                    case 10:
                        DecreaseCenterVertical();
                        break;

                    case 11:
                        DecreaseCenterHorizontal();
                        break;
                }

                await Task.Delay(200);
            }

            _isShuffling = false;
            _shufflingPanel.SetActive(false);

            //UpdateCubeFrontFace();
            for (int j = 0; j < originTransforms.Count; j++)
            {
                RaycastHit2D hit = Physics2D.Raycast(originTransforms[j].position, Vector3.forward);
                if (hit.transform != null)
                {
                    hit.transform.GetComponent<SpriteRenderer>().sprite = CubeFrontFace[j].sprite;
                    frontSprites[j].sprite = hit.transform.GetComponent<Icons>().GetSprite();
                    frontSprites[j].name = hit.transform.gameObject.name;
                    hit.transform.gameObject.SetActive(false);
                    frontSprites[j].gameObject.SetActive(true);
                    _detected.Add(hit.transform.GetComponent<SpriteRenderer>());
                }
            }

            StartCoroutine(UIManager.INSTANCE.StartTimer());
            //InitiateLayers();
        }

        #region Rubics Cube Reward System

        private List<bool> DetectFacesPattern()
        {
            List<bool> facesMatching = new List<bool>();

            //Front
            if (CheckForWinningPatterns.INSTANCE.CheckForFacesRubicsCube(GetIconsFromFacePlate(_cubeState.Front)))
            {
                facesMatching.Add(true);
            }
            //Back
            if (CheckForWinningPatterns.INSTANCE.CheckForFacesRubicsCube(GetIconsFromFacePlate(_cubeState.Back)))
            {
                facesMatching.Add(true);
            }
            //Left
            if (CheckForWinningPatterns.INSTANCE.CheckForFacesRubicsCube(GetIconsFromFacePlate(_cubeState.Left)))
            {
                facesMatching.Add(true);
            }
            //Right
            if (CheckForWinningPatterns.INSTANCE.CheckForFacesRubicsCube(GetIconsFromFacePlate(_cubeState.Right)))
            {
                facesMatching.Add(true);
            }
            //Up
            if (CheckForWinningPatterns.INSTANCE.CheckForFacesRubicsCube(GetIconsFromFacePlate(_cubeState.Up)))
            {
                facesMatching.Add(true);
            }
            //Down
            if (CheckForWinningPatterns.INSTANCE.CheckForFacesRubicsCube(GetIconsFromFacePlate(_cubeState.Down)))
            {
                facesMatching.Add(true);
            }
            return facesMatching;
        }

        private List<Icons> GetIconsFromFacePlate(List<GameObject> facePlates)
        {
            List<Icons> faceIcons = new List<Icons>();
            foreach (GameObject obj in facePlates)
            {
                Icons icon = obj.GetComponentInChildren<Icons>();
                if (icon != null)
                {
                    faceIcons.Add(icon);
                }
            }
            return faceIcons;
        }

        public (float,int) RewardPercentage()
        {
            int count = DetectFacesPattern().Count;
            return (_paytable.RewardPercentForEachMatchingfaceInRubicsCube * count,count);
            
            // for testing cube results
            //int test = 2;
            //return (_paytable.RewardPercentForEachMatchingfaceInRubicsCube * test,test);
            
        }

        public void OnSubmit()
        {
            UIManager.INSTANCE.stopTimer = true;
            _submitCubeButton.SetActive(false);
            if (RewardPercentage().Item1 > 0)
            {
                CheckForWinningPatterns.PatternFound?.Invoke("RubikMode");
            }
            else
            {
                print("Pattern Not found : On Submit");
                CheckForWinningPatterns.PatternNotFound?.Invoke();
            }
        }

        #endregion


        [Serializable]
        private class FaceToSprite
        {
            public Faces Face;
            public Transform RayPoint;
        }

    }
}
