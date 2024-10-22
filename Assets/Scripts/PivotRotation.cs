using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubicsCube
{
    public class PivotRotation : MonoBehaviour
    {
        private List<GameObject> activeSide;
        private float speed = 300f;
        private Vector3 rotation;

        private Quaternion targetQuaternion;

        private ReadCube readCube;
        private CubeState cubeState;
        private int rotateDirection;

        // Start is called before the first frame update
        void Start()
        {
            readCube = FindObjectOfType<ReadCube>();
            cubeState = FindObjectOfType<CubeState>();
        }

        private bool inProgress = false;

        private void SpinSide(List<GameObject> side, int rotateDirection)
        {
            if (inProgress)
                return;
            else
                inProgress = true;

            // reset the rotation
            rotation = Vector3.zero;

            if (side == cubeState.Up)
            {
                rotation.y = rotateDirection * 90;
            }
            else if (side == cubeState.Down)
            {
                rotation.y = rotateDirection * 90;
            }
            else if (side == cubeState.Left)
            {
                rotation.z = rotateDirection * 90;
            }
            else if (side == cubeState.Right)
            {
                rotation.z = rotateDirection * 90;
            }
            else if (side == cubeState.Front)
            {
                rotation.x = rotateDirection * 90;
            }
            else if (side == cubeState.Back)
            {
                rotation.x = rotateDirection * 90;
            }
            else if (side == cubeState.CenterHorizontal)
            {
                rotation.y = rotateDirection * 90;
            }
            else if (side == cubeState.CenterVertical)
            {
                rotation.z = rotateDirection * 90;
            }
            else if (side == cubeState.CenterCenter)
            {
                rotation.x = rotateDirection * 90;
            }

            // rotate
            //transform.Rotate(rotation, Space.Self);
            Quaternion desiredRotation = transform.rotation * Quaternion.Euler(rotation);
            transform.rotation = Quaternion.Slerp(transform.rotation,desiredRotation,1);

            inProgress = false;
        }


        public void Rotate(List<GameObject> side, int rotatedirection)
        {
            this.rotateDirection = rotatedirection;
            activeSide = side;
            SpinSide(activeSide, rotateDirection);
            // Create a vector to rotate around
            //localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
        }

        public void StartAutoRotate(List<GameObject> side, float angle)
        {
            cubeState.PickUp(side);
            Vector3 localForward = Vector3.zero - side[4].transform.parent.transform.localPosition;
            targetQuaternion = Quaternion.AngleAxis(angle, localForward) * transform.localRotation;
            activeSide = side;
        }


        public void RotateToRightAngle()
        {
            Vector3 vec = transform.localEulerAngles;
            // round vec to nearest 90 degrees
            vec.x = Mathf.Round(vec.x / 90) * 90;
            vec.y = Mathf.Round(vec.y / 90) * 90;
            vec.z = Mathf.Round(vec.z / 90) * 90;

            targetQuaternion.eulerAngles = vec;
            //autoRotating = true;
        }
    }
}
