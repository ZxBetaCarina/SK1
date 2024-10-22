using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace RubicsCube
{
    public class SelectFace : MonoBehaviour
    {
        private CubeState cubeState;
        private ReadCube readCube;
        private int layerMask = 1 << 8;
        public static Action<Transform> RotateCenterRow;
        public GameObject centerHorizontal;
        public GameObject centerVertical;
        public GameObject centerCenter;
        public GameObject frontPivot;
        public GameObject backPivot;
        public GameObject upPivot;
        public GameObject downPivot;
        public GameObject leftPivot;
        public GameObject rightPivot;

        // Start is called before the first frame update

        void Start()
        {
            centerHorizontal = new("HorizontalCenter");

            centerVertical = new("VerticalCenter");

            centerCenter = new("CenterCenter");

            upPivot = new("UpPivot");

            downPivot = new("DownPivot");

            leftPivot = new("LeftPivot");

            rightPivot = new("RightPivot");
            frontPivot = new("frontPivot");
            backPivot = new("backPivot");


            centerHorizontal.transform.position = transform.position;
            centerHorizontal.transform.rotation = Quaternion.identity;
            centerHorizontal.transform.parent = transform;
            centerHorizontal.transform.AddComponent<PivotRotation>();


            centerVertical.transform.position = transform.position;
            centerVertical.transform.rotation = Quaternion.identity;
            centerVertical.transform.parent = transform;
            centerVertical.transform.AddComponent<PivotRotation>();

            centerCenter.transform.position = transform.position;
            centerCenter.transform.rotation = Quaternion.identity;
            centerCenter.transform.parent = transform;
            centerCenter.transform.AddComponent<PivotRotation>();

            upPivot.transform.position = transform.position;
            upPivot.transform.rotation = Quaternion.identity;
            upPivot.transform.parent = transform;
            upPivot.transform.AddComponent<PivotRotation>();

            downPivot.transform.position = transform.position;
            downPivot.transform.rotation = Quaternion.identity;
            downPivot.transform.parent = transform;
            downPivot.transform.AddComponent<PivotRotation>();

            leftPivot.transform.position = transform.position;
            leftPivot.transform.rotation = Quaternion.identity;
            leftPivot.transform.parent = transform;
            leftPivot.transform.AddComponent<PivotRotation>();

            rightPivot.transform.position = transform.position;
            rightPivot.transform.rotation = Quaternion.identity;
            rightPivot.transform.parent = transform;
            rightPivot.transform.AddComponent<PivotRotation>();

            frontPivot.transform.position = transform.position;
            frontPivot.transform.rotation = Quaternion.identity;
            frontPivot.transform.parent = transform;
            frontPivot.transform.AddComponent<PivotRotation>();

            backPivot.transform.position = transform.position;
            backPivot.transform.rotation = Quaternion.identity;
            backPivot.transform.parent = transform;
            backPivot.transform.AddComponent<PivotRotation>();

            readCube = FindObjectOfType<ReadCube>();
            cubeState = FindObjectOfType<CubeState>();
            //readCube.ReadState();
        }

        // Update is called once per frame
        void Update()
        {
            //List<List<GameObject>> cubeSides = new List<List<GameObject>>()
            //    {
            //        cubeState.Up,
            //        cubeState.Down,
            //        cubeState.Left,
            //        cubeState.Right,
            //        cubeState.Front,
            //        cubeState.Back,
            //        cubeState.CenterHorizontal,
            //        cubeState.CenterVertical,
            //        cubeState.CenterCenter
            //    };
            ////If the face hit exists within a side
            //foreach (List<GameObject> cubeSide in cubeSides)
            //{
            //    cubeState.PickUp(cubeSide);
            //}
        }

        public async void RotateSide(List<GameObject> cubeside, int rotateDir)
        {
            //readCube.ReadState();
            if (cubeside == cubeState.Front)
            {
                RotateCenterRow?.Invoke(frontPivot.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                frontPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
            else if (cubeside == cubeState.Back)
            {

                RotateCenterRow?.Invoke(backPivot.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                backPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
            else if (cubeside == cubeState.CenterHorizontal)
            {
                //seperate logic
                RotateCenterRow?.Invoke(centerHorizontal.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                centerHorizontal.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
            else if (cubeside == cubeState.CenterVertical)
            {
                //seperate logic
                RotateCenterRow?.Invoke(centerVertical.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                centerVertical.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
            else if (cubeside == cubeState.CenterCenter)
            {
                RotateCenterRow?.Invoke(centerCenter.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                centerCenter.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
            else if (cubeside == cubeState.Left)
            {
                RotateCenterRow?.Invoke(leftPivot.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                leftPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
            else if (cubeside == cubeState.Right)
            {
                RotateCenterRow?.Invoke(rightPivot.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                rightPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
            else if (cubeside == cubeState.Up)
            {
                RotateCenterRow?.Invoke(upPivot.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                upPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
            else if (cubeside == cubeState.Down)
            {

                RotateCenterRow?.Invoke(downPivot.transform);
                cubeState.PickUp(cubeside);
                await Task.Delay(50);
                downPivot.transform.GetComponent<PivotRotation>().Rotate(cubeside, rotateDir);
            }
        }
    }
}
