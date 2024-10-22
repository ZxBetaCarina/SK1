using AstekUtility.ServiceLocatorTool;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace RubicsCube
{
    public enum Faces
    {
        Front,
        Back,
        Up,
        Down,
        Left,
        Right,
    }

    [Serializable]
    public class FacePivot
    {
        [field: SerializeField] public Faces Face { get; private set; }
        [field: SerializeField] public Transform Pivot { get; private set; }
    }

    public class RelativePivot : MonoBehaviour
    {
        [SerializeField] private AudioSource audioSource;
        [SerializeField] private FacePivot[] cameraFacePivot;
        [SerializeField] private Transform[] rayPoint;
        [SerializeField] private ReadCube readCube;

        [Header("Buttons- dirToRotate_FaceToRotate")]
        [SerializeField] private Button down_Left;
        [SerializeField] private Button down_Center;
        [SerializeField] private Button down_Right;

        [SerializeField] private Button up_Left;
        [SerializeField] private Button up_Center;
        [SerializeField] private Button up_Right;

        [SerializeField] private Button right_Up;
        [SerializeField] private Button right_Center;
        [SerializeField] private Button right_Down;

        [SerializeField] private Button left_Up;
        [SerializeField] private Button left_Center;
        [SerializeField] private Button left_Down;

        private Dictionary<Faces, Transform> _faceToPivot;
        [SerializeField] private RubikCubeController _rubikController;

        private void Awake()
        {
            _faceToPivot = new Dictionary<Faces, Transform>();
            SetButtonFunctionAccordingToFace();
        }

        public void SetButtonFunctionAccordingToFace()
        {
            foreach (Transform point in rayPoint)
            {
                foreach (FacePivot pivot in cameraFacePivot)
                {
                    if (Mathf.Sqrt((pivot.Pivot.position - point.position).sqrMagnitude) < 0.1f)
                    {
                        if (!_faceToPivot.ContainsKey(pivot.Face))
                            _faceToPivot.Add(pivot.Face, point);
                        else
                            _faceToPivot[pivot.Face] = point;
                    }

                    if (_faceToPivot.ContainsKey(Faces.Up) && _faceToPivot.ContainsKey(Faces.Left)
                        && _faceToPivot[Faces.Up] != null && _faceToPivot[Faces.Left] != null)
                    {
                        goto Exit;
                    }
                }
            }

        Exit:

            ResetVerticalButton();
            ResetHorizontalButton();
            //For left and right buttons
            //switch (_faceToPivot[Faces.Up].name)
            //{
            //    case "Up":

            //        right_Up.onClick.AddListener(_rubikController.DecreaseUpLayer);
            //        right_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
            //        right_Down.onClick.AddListener(_rubikController.DecreaseDownLayer);

            //        left_Up.onClick.AddListener(_rubikController.IncreaseUpLayer);
            //        left_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
            //        left_Down.onClick.AddListener(_rubikController.IncreaseDownLayer);

            //        break;

            //    case "Down":

            //        right_Up.onClick.AddListener(_rubikController.IncreaseDownLayer);
            //        right_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
            //        right_Down.onClick.AddListener(_rubikController.IncreaseUpLayer);

            //        left_Up.onClick.AddListener(_rubikController.DecreaseDownLayer);
            //        left_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
            //        left_Down.onClick.AddListener(_rubikController.DecreaseUpLayer);

            //        break;

            //    case "Left":

            //        right_Up.onClick.AddListener(_rubikController.IncreaseLeftLayer);
            //        right_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
            //        right_Down.onClick.AddListener(_rubikController.IncreaseRightLayer);

            //        left_Up.onClick.AddListener(_rubikController.DecreaseLeftLayer);
            //        left_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
            //        left_Down.onClick.AddListener(_rubikController.DecreaseRightLayer);

            //        break;

            //    case "Right":

            //        right_Up.onClick.AddListener(_rubikController.DecreaseRightLayer);
            //        right_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
            //        right_Down.onClick.AddListener(_rubikController.DecreaseLeftLayer);

            //        left_Up.onClick.AddListener(_rubikController.IncreaseRightLayer);
            //        left_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
            //        left_Down.onClick.AddListener(_rubikController.IncreaseLeftLayer);

            //        break;

            //    case "Front":

            //        right_Up.onClick.AddListener(_rubikController.IncreaseFrontLayer);
            //        right_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
            //        right_Down.onClick.AddListener(_rubikController.IncreaseBackLayer);

            //        left_Up.onClick.AddListener(_rubikController.DecreaseFrontLayer);
            //        left_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
            //        left_Down.onClick.AddListener(_rubikController.DecreaseBackLayer);

            //        break;

            //    case "Back":

            //        right_Up.onClick.AddListener(_rubikController.DecreaseBackLayer);
            //        right_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
            //        right_Down.onClick.AddListener(_rubikController.DecreaseFrontLayer);

            //        left_Up.onClick.AddListener(_rubikController.IncreaseBackLayer);
            //        left_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
            //        left_Down.onClick.AddListener(_rubikController.IncreaseFrontLayer);

            //        break;
            //}

            ////For up and down movement
            //switch (_faceToPivot[Faces.Left].name)
            //{
            //    case "Left":

            //        up_Left.onClick.AddListener(_rubikController.DecreaseLeftLayer);
            //        up_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
            //        up_Right.onClick.AddListener(_rubikController.DecreaseRightLayer);

            //        down_Left.onClick.AddListener(_rubikController.IncreaseLeftLayer);
            //        down_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
            //        down_Right.onClick.AddListener(_rubikController.IncreaseRightLayer);

            //        break;

            //    case "Right":

            //        up_Left.onClick.AddListener(_rubikController.IncreaseRightLayer);
            //        up_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
            //        up_Right.onClick.AddListener(_rubikController.IncreaseLeftLayer);

            //        down_Left.onClick.AddListener(_rubikController.DecreaseRightLayer);
            //        down_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
            //        down_Right.onClick.AddListener(_rubikController.DecreaseLeftLayer);

            //        break;

            //    case "Up":

            //        up_Left.onClick.AddListener(_rubikController.IncreaseUpLayer);
            //        up_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
            //        up_Right.onClick.AddListener(_rubikController.IncreaseDownLayer);

            //        down_Left.onClick.AddListener(_rubikController.DecreaseUpLayer);
            //        down_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
            //        down_Right.onClick.AddListener(_rubikController.DecreaseDownLayer);

            //        break;

            //    case "Down":

            //        up_Left.onClick.AddListener(_rubikController.DecreaseDownLayer);
            //        up_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
            //        up_Right.onClick.AddListener(_rubikController.DecreaseUpLayer);

            //        down_Left.onClick.AddListener(_rubikController.IncreaseDownLayer);
            //        down_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
            //        down_Right.onClick.AddListener(_rubikController.IncreaseUpLayer);

            //        break;

            //    case "Front":

            //        up_Left.onClick.AddListener(_rubikController.IncreaseFrontLayer);
            //        up_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
            //        up_Right.onClick.AddListener(_rubikController.IncreaseBackLayer);

            //        down_Left.onClick.AddListener(_rubikController.DecreaseFrontLayer);
            //        down_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
            //        down_Right.onClick.AddListener(_rubikController.DecreaseBackLayer);

            //        break;

            //    case "Back":

            //        up_Left.onClick.AddListener(_rubikController.DecreaseBackLayer);
            //        up_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
            //        up_Right.onClick.AddListener(_rubikController.DecreaseFrontLayer);

            //        down_Left.onClick.AddListener(_rubikController.IncreaseBackLayer);
            //        down_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
            //        down_Right.onClick.AddListener(_rubikController.IncreaseFrontLayer);

            //        break;
            //}

            FaceToButtonConnection();

            _faceToPivot[Faces.Up] = null;
            _faceToPivot[Faces.Left] = null;
        }

        private void FaceToButtonConnection()
        {
            if (_faceToPivot[Faces.Up].name == "Up")
            {
                right_Up.onClick.AddListener(_rubikController.DecreaseUpLayer);
                right_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                right_Down.onClick.AddListener(_rubikController.DecreaseDownLayer);

                left_Up.onClick.AddListener(_rubikController.IncreaseUpLayer);
                left_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                left_Down.onClick.AddListener(_rubikController.IncreaseDownLayer);
                switch (_faceToPivot[Faces.Left].name)
                {
                    case "Left":

                        up_Left.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                        up_Right.onClick.AddListener(_rubikController.DecreaseRightLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseLeftLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                        down_Right.onClick.AddListener(_rubikController.IncreaseRightLayer);

                        break;

                    case "Right":

                        up_Left.onClick.AddListener(_rubikController.IncreaseRightLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                        up_Right.onClick.AddListener(_rubikController.IncreaseLeftLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseRightLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                        down_Right.onClick.AddListener(_rubikController.DecreaseLeftLayer);

                        break;


                    case "Front":

                        up_Left.onClick.AddListener(_rubikController.IncreaseFrontLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                        up_Right.onClick.AddListener(_rubikController.IncreaseBackLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseFrontLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                        down_Right.onClick.AddListener(_rubikController.DecreaseBackLayer);

                        break;

                    case "Back":

                        up_Left.onClick.AddListener(_rubikController.DecreaseBackLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                        up_Right.onClick.AddListener(_rubikController.DecreaseFrontLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseBackLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                        down_Right.onClick.AddListener(_rubikController.IncreaseFrontLayer);

                        break;
                }
            }
            else if (_faceToPivot[Faces.Up].name == "Down")
            {
                right_Up.onClick.AddListener(_rubikController.IncreaseDownLayer);
                right_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                right_Down.onClick.AddListener(_rubikController.IncreaseUpLayer);

                left_Up.onClick.AddListener(_rubikController.DecreaseDownLayer);
                left_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                left_Down.onClick.AddListener(_rubikController.DecreaseUpLayer);
                switch (_faceToPivot[Faces.Left].name)
                {
                    case "Left":

                        up_Left.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                        up_Right.onClick.AddListener(_rubikController.DecreaseRightLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseLeftLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                        down_Right.onClick.AddListener(_rubikController.IncreaseRightLayer);

                        break;

                    case "Right":

                        up_Left.onClick.AddListener(_rubikController.IncreaseRightLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                        up_Right.onClick.AddListener(_rubikController.IncreaseLeftLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseRightLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                        down_Right.onClick.AddListener(_rubikController.DecreaseLeftLayer);

                        break;

                    case "Front":

                        up_Left.onClick.AddListener(_rubikController.IncreaseFrontLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                        up_Right.onClick.AddListener(_rubikController.IncreaseBackLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseFrontLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                        down_Right.onClick.AddListener(_rubikController.DecreaseBackLayer);

                        break;

                    case "Back":

                        up_Left.onClick.AddListener(_rubikController.DecreaseBackLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                        up_Right.onClick.AddListener(_rubikController.DecreaseFrontLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseBackLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                        down_Right.onClick.AddListener(_rubikController.IncreaseFrontLayer);

                        break;
                }
            }
            else if (_faceToPivot[Faces.Up].name == "Front")
            {
                right_Up.onClick.AddListener(_rubikController.IncreaseFrontLayer);
                right_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                right_Down.onClick.AddListener(_rubikController.IncreaseBackLayer);

                left_Up.onClick.AddListener(_rubikController.DecreaseFrontLayer);
                left_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                left_Down.onClick.AddListener(_rubikController.DecreaseBackLayer);
                switch (_faceToPivot[Faces.Left].name)
                {
                    case "Left":

                        up_Left.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                        up_Right.onClick.AddListener(_rubikController.DecreaseRightLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseLeftLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                        down_Right.onClick.AddListener(_rubikController.IncreaseRightLayer);

                        break;

                    case "Right":

                        up_Left.onClick.AddListener(_rubikController.IncreaseRightLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                        up_Right.onClick.AddListener(_rubikController.IncreaseLeftLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseRightLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                        down_Right.onClick.AddListener(_rubikController.DecreaseLeftLayer);

                        break;

                    case "Up":

                        up_Left.onClick.AddListener(_rubikController.DecreaseUpLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                        up_Right.onClick.AddListener(_rubikController.DecreaseDownLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseUpLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                        down_Right.onClick.AddListener(_rubikController.IncreaseDownLayer);

                        break;

                    case "Down":

                        up_Left.onClick.AddListener(_rubikController.IncreaseDownLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                        up_Right.onClick.AddListener(_rubikController.IncreaseUpLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseDownLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                        down_Right.onClick.AddListener(_rubikController.DecreaseUpLayer);

                        break;
                }
            }
            else if (_faceToPivot[Faces.Up].name == "Back")
            {
                right_Up.onClick.AddListener(_rubikController.DecreaseBackLayer);
                right_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                right_Down.onClick.AddListener(_rubikController.DecreaseFrontLayer);

                left_Up.onClick.AddListener(_rubikController.IncreaseBackLayer);
                left_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                left_Down.onClick.AddListener(_rubikController.IncreaseFrontLayer);
                switch (_faceToPivot[Faces.Left].name)
                {
                    case "Left":

                        up_Left.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                        up_Right.onClick.AddListener(_rubikController.DecreaseRightLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseLeftLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                        down_Right.onClick.AddListener(_rubikController.IncreaseRightLayer);

                        break;

                    case "Right":

                        up_Left.onClick.AddListener(_rubikController.IncreaseRightLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                        up_Right.onClick.AddListener(_rubikController.IncreaseLeftLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseRightLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                        down_Right.onClick.AddListener(_rubikController.DecreaseLeftLayer);

                        break;

                    case "Up":

                        up_Left.onClick.AddListener(_rubikController.DecreaseUpLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                        up_Right.onClick.AddListener(_rubikController.DecreaseDownLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseUpLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                        down_Right.onClick.AddListener(_rubikController.IncreaseDownLayer);

                        break;

                    case "Down":

                        up_Left.onClick.AddListener(_rubikController.IncreaseDownLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                        up_Right.onClick.AddListener(_rubikController.IncreaseUpLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseDownLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                        down_Right.onClick.AddListener(_rubikController.DecreaseUpLayer);

                        break;
                }
            }
            else if (_faceToPivot[Faces.Up].name == "Right")
            {
                

                if (_faceToPivot[Faces.Left].name == "Front")
                {
                    right_Up.onClick.AddListener(_rubikController.IncreaseRightLayer);
                    right_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                    right_Down.onClick.AddListener(_rubikController.IncreaseLeftLayer);

                    left_Up.onClick.AddListener(_rubikController.DecreaseRightLayer);
                    left_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                    left_Down.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                }
                else if (_faceToPivot[Faces.Left].name == "Down")
                {
                    right_Up.onClick.AddListener(_rubikController.IncreaseRightLayer);
                    right_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                    right_Down.onClick.AddListener(_rubikController.IncreaseLeftLayer);

                    left_Up.onClick.AddListener(_rubikController.DecreaseRightLayer);
                    left_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                    left_Down.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                }
                else if (_faceToPivot[Faces.Left].name == "Back")
                {
                    right_Up.onClick.AddListener(_rubikController.IncreaseRightLayer);
                    right_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                    right_Down.onClick.AddListener(_rubikController.IncreaseLeftLayer);

                    left_Up.onClick.AddListener(_rubikController.DecreaseRightLayer);
                    left_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                    left_Down.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                }
                else
                {
                    right_Up.onClick.AddListener(_rubikController.IncreaseRightLayer);
                    right_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                    right_Down.onClick.AddListener(_rubikController.IncreaseLeftLayer);

                    left_Up.onClick.AddListener(_rubikController.DecreaseRightLayer);
                    left_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                    left_Down.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                }

                switch (_faceToPivot[Faces.Left].name)
                {
                    case "Front":

                        up_Left.onClick.AddListener(_rubikController.IncreaseFrontLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                        up_Right.onClick.AddListener(_rubikController.IncreaseBackLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseFrontLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                        down_Right.onClick.AddListener(_rubikController.DecreaseBackLayer);

                        break;

                    case "Back":

                        up_Left.onClick.AddListener(_rubikController.DecreaseBackLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                        up_Right.onClick.AddListener(_rubikController.DecreaseFrontLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseBackLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                        down_Right.onClick.AddListener(_rubikController.IncreaseFrontLayer);
                        break;

                    case "Up":

                        up_Left.onClick.AddListener(_rubikController.DecreaseUpLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                        up_Right.onClick.AddListener(_rubikController.DecreaseDownLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseUpLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                        down_Right.onClick.AddListener(_rubikController.IncreaseDownLayer);

                        break;

                    case "Down":

                        up_Left.onClick.AddListener(_rubikController.IncreaseDownLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                        up_Right.onClick.AddListener(_rubikController.IncreaseUpLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseDownLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                        down_Right.onClick.AddListener(_rubikController.DecreaseUpLayer);

                        break;
                }
            }
            else if (_faceToPivot[Faces.Up].name == "Left")
            {
                right_Up.onClick.AddListener(_rubikController.DecreaseLeftLayer);
                right_Center.onClick.AddListener(_rubikController.IncreaseCenterVertical);
                right_Down.onClick.AddListener(_rubikController.DecreaseRightLayer);

                left_Up.onClick.AddListener(_rubikController.IncreaseLeftLayer);
                left_Center.onClick.AddListener(_rubikController.DecreaseCenterVertical);
                left_Down.onClick.AddListener(_rubikController.IncreaseRightLayer);
                switch (_faceToPivot[Faces.Left].name)
                {
                    case "Front":

                        up_Left.onClick.AddListener(_rubikController.IncreaseFrontLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                        up_Right.onClick.AddListener(_rubikController.IncreaseBackLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseFrontLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                        down_Right.onClick.AddListener(_rubikController.DecreaseBackLayer);

                        break;

                    case "Back":

                        up_Left.onClick.AddListener(_rubikController.DecreaseBackLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterCenter);
                        up_Right.onClick.AddListener(_rubikController.DecreaseFrontLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseBackLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterCenter);
                        down_Right.onClick.AddListener(_rubikController.IncreaseFrontLayer);
                        break;

                    case "Up":

                        up_Left.onClick.AddListener(_rubikController.DecreaseUpLayer);
                        up_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                        up_Right.onClick.AddListener(_rubikController.DecreaseDownLayer);

                        down_Left.onClick.AddListener(_rubikController.IncreaseUpLayer);
                        down_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                        down_Right.onClick.AddListener(_rubikController.IncreaseDownLayer);

                        break;

                    case "Down":

                        up_Left.onClick.AddListener(_rubikController.IncreaseDownLayer);
                        up_Center.onClick.AddListener(_rubikController.IncreaseCenterHorizontal);
                        up_Right.onClick.AddListener(_rubikController.IncreaseUpLayer);

                        down_Left.onClick.AddListener(_rubikController.DecreaseDownLayer);
                        down_Center.onClick.AddListener(_rubikController.DecreaseCenterHorizontal);
                        down_Right.onClick.AddListener(_rubikController.DecreaseUpLayer);

                        break;
                }
            }
        }

        private void ResetHorizontalButton()
        {
            right_Up.onClick.RemoveAllListeners();
            right_Up.onClick.AddListener(audioSource.Play);
            right_Center.onClick.RemoveAllListeners();
            right_Center.onClick.AddListener(audioSource.Play);
            right_Down.onClick.RemoveAllListeners();
            right_Down.onClick.AddListener(audioSource.Play);

            left_Up.onClick.RemoveAllListeners();
            left_Up.onClick.AddListener(audioSource.Play);
            left_Center.onClick.RemoveAllListeners();
            left_Center.onClick.AddListener(audioSource.Play);
            left_Down.onClick.RemoveAllListeners();
            left_Down.onClick.AddListener(audioSource.Play);
        }

        private void ResetVerticalButton()
        {
            up_Left.onClick.RemoveAllListeners();
            up_Left.onClick.AddListener(audioSource.Play);
            up_Center.onClick.RemoveAllListeners();
            up_Center.onClick.AddListener(audioSource.Play);
            up_Right.onClick.RemoveAllListeners();
            up_Right.onClick.AddListener(audioSource.Play);

            down_Left.onClick.RemoveAllListeners();
            down_Left.onClick.AddListener(audioSource.Play);
            down_Center.onClick.RemoveAllListeners();
            down_Center.onClick.AddListener(audioSource.Play);
            down_Right.onClick.RemoveAllListeners();
            down_Right.onClick.AddListener(audioSource.Play);
        }
    }
}
