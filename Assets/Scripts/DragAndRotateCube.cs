using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using static UnityEngine.GraphicsBuffer;

namespace RubicsCube
{
    public class DragAndRotateCube : MonoBehaviour
    {
        private Vector2 _firstPressPos;
        private Vector2 _secondPressPos;
        private Vector2 _currentSwipe;
        private Vector3 _previousMousePosition;
        private Vector3 _mouseDelta;
        [SerializeField] private Transform rubicsCube;
        [SerializeField] private GameObject targetCube;
        [SerializeField] private float speed;
        [SerializeField] private Transform cameraObject;
        [SerializeField] private float minDistToConsiderSwipe;
        [SerializeField] private RelativePivot relativePivot;
        [SerializeField] private CubeState cubeState;

        private void Awake()
        {
            relativePivot.SetButtonFunctionAccordingToFace();
        }

        void Update()
        {
            Swipe();
            //If we also want drag remove the bottom comment if condition and use Drag part
            if (cameraObject.rotation != targetCube.transform.rotation)
            {
                float step = speed * Time.deltaTime;
                cameraObject.rotation = Quaternion.RotateTowards(cameraObject.rotation, targetCube.transform.rotation, step);
                if (cameraObject.rotation == targetCube.transform.rotation)
                {
                    relativePivot.SetButtonFunctionAccordingToFace();
                }
            }
            //Drag();

        }
        void Drag()
        {
            if (Input.GetMouseButton(0))
            {
                // while the mouse is held down the cube can be moved around its central axis to provide visual feedback
                _mouseDelta = Input.mousePosition - _previousMousePosition;
                _mouseDelta *= 0.1f; // reduction of rotation speed
                rubicsCube.rotation = Quaternion.Euler(_mouseDelta.y, -_mouseDelta.x, 0) * rubicsCube.rotation;
            }
            else
            {
                // automatically move to the target position
                if (rubicsCube.rotation != targetCube.transform.rotation)
                {
                    float step = speed * Time.deltaTime;
                    rubicsCube.rotation = Quaternion.RotateTowards(rubicsCube.rotation, targetCube.transform.rotation, step);
                }
            }
            _previousMousePosition = Input.mousePosition;


        }
        void Swipe()
        {
            if (Input.GetMouseButtonDown(0))
            {
                _firstPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            }
            if (Input.GetMouseButtonUp(0))
            {
                // get the 2D poition of the second mouse click
                _secondPressPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                //create a vector from the first and second click positions
                _currentSwipe = new Vector2(_secondPressPos.x - _firstPressPos.x, _secondPressPos.y - _firstPressPos.y);

                if (_currentSwipe.magnitude < minDistToConsiderSwipe)
                    return;

                //normalize the 2d vector
                _currentSwipe.Normalize();

                if (LeftSwipe(_currentSwipe))
                {
                    //_targetCube.transform.Rotate(0, -90, 0, Space.Self);
                    targetCube.transform.Rotate(Vector3.up, -90, Space.Self);
                }
                else if (RightSwipe(_currentSwipe))
                {
                    //_targetCube.transform.Rotate(0, 90, 0, Space.Self);
                    targetCube.transform.Rotate(Vector3.up, 90, Space.Self);
                }
                else if (UpLeftSwipe(_currentSwipe))
                {
                    //_targetCube.transform.Rotate(-90, 0, 0, Space.Self);
                    targetCube.transform.Rotate(Vector3.left, 90, Space.Self);
                }
                else if (UpRightSwipe(_currentSwipe))
                {
                    //_targetCube.transform.Rotate(0, 0, 90, Space.Self);
                    targetCube.transform.Rotate(Vector3.forward, 90, Space.Self);
                }
                else if (DownLeftSwipe(_currentSwipe))
                {
                    //_targetCube.transform.Rotate(0, 0, -90, Space.Self);
                    targetCube.transform.Rotate(Vector3.forward, -90, Space.Self);
                }
                else if (DownRightSwipe(_currentSwipe))
                {
                    //_targetCube.transform.Rotate(90, 0, 0, Space.Self);
                    targetCube.transform.Rotate(Vector3.left, -90, Space.Self);
                }
            }
        }

        bool LeftSwipe(Vector2 swipe)
        {
            return _currentSwipe.x < 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f;
        }

        bool RightSwipe(Vector2 swipe)
        {
            return _currentSwipe.x > 0 && _currentSwipe.y > -0.5f && _currentSwipe.y < 0.5f;
        }

        bool UpLeftSwipe(Vector2 swipe)
        {
            return _currentSwipe.y > 0 && _currentSwipe.x < 0f;
        }

        bool UpRightSwipe(Vector2 swipe)
        {
            return _currentSwipe.y > 0 && _currentSwipe.x > 0f;
        }

        bool DownLeftSwipe(Vector2 swipe)
        {
            return _currentSwipe.y < 0 && _currentSwipe.x < 0f;
        }

        bool DownRightSwipe(Vector2 swipe)
        {
            return _currentSwipe.y < 0 && _currentSwipe.x > 0f;
        }

    }
}