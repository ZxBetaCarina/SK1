using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubicsCube
{
    public class CubeState : MonoBehaviour
    {
        // sides
        public List<GameObject> Front = new List<GameObject>();
        public List<GameObject> Back = new List<GameObject>();
        public List<GameObject> Up = new List<GameObject>();
        public List<GameObject> Down = new List<GameObject>();
        public List<GameObject> Left = new List<GameObject>();
        public List<GameObject> Right = new List<GameObject>();
        public List<GameObject> CenterVertical = new();
        public List<GameObject> CenterHorizontal = new();
        public List<GameObject> CenterCenter = new();

        public static bool autoRotating = false;
        public static bool started = false;
        private Transform centerParent;

        private void OnEnable()
        {
            SelectFace.RotateCenterRow += PickUpParent;
        }

        private void OnDisable()
        {
            SelectFace.RotateCenterRow -= PickUpParent;
        }
        public void PickUp(List<GameObject> cubeSide)
        {
            foreach (GameObject face in cubeSide)
            {
                face.transform.parent.transform.parent = centerParent;
            }

        }

        private void PickUpParent(Transform parent)
        {
            centerParent = parent;
        }

        public void PutDown(List<GameObject> littleCubes, Transform pivot)
        {
            foreach (GameObject littleCube in littleCubes)
            {
                if (littleCube != littleCubes[4])
                {
                    littleCube.transform.parent.transform.parent = pivot;
                }
            }
        }

        string GetSideString(List<GameObject> side)
        {
            string sideString = "";
            foreach (GameObject face in side)
            {
                sideString += face.name[0].ToString();
            }
            return sideString;
        }

        public string GetStateString()
        {
            string stateString = "";
            stateString += GetSideString(Up);
            stateString += GetSideString(Right);
            stateString += GetSideString(Front);
            stateString += GetSideString(Down);
            stateString += GetSideString(Left);
            stateString += GetSideString(Back);
            return stateString;
        }

        public void TestAlignment()
        {
            foreach(GameObject obj in Front)
            {
                obj.GetComponentInChildren<AlignToTop>().Align();
            }
            foreach (GameObject obj in Back)
            {
                obj.GetComponentInChildren<AlignToTop>().Align();
            }
            foreach (GameObject obj in Left)
            {
                obj.GetComponentInChildren<AlignToTop>().Align();
            }
            foreach (GameObject obj in Right)
            {
                obj.GetComponentInChildren<AlignToTop>().Align();
            }
            foreach (GameObject obj in Up)
            {
                obj.GetComponentInChildren<AlignToTop>().Align();
            }
            foreach (GameObject obj in Down)
            {
                obj.GetComponentInChildren<AlignToTop>().Align();
            }
        }
    }
}
