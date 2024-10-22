using System.Collections.Generic;
using UnityEngine;

namespace RubicsCube
{
    public class ReadCube : MonoBehaviour
    {
        [SerializeField] private Transform tUp;
        [SerializeField] private Transform tDown;
        [SerializeField] private Transform tLeft;
        [SerializeField] private Transform tRight;
        [SerializeField] private Transform tFront;
        [SerializeField] private Transform tBack;

        private List<GameObject> _frontRays = new List<GameObject>();
        private List<GameObject> _backRays = new List<GameObject>();
        private List<GameObject> _upRays = new List<GameObject>();
        private List<GameObject> _downRays = new List<GameObject>();
        private List<GameObject> _leftRays = new List<GameObject>();
        private List<GameObject> _rightRays = new List<GameObject>();
        [SerializeField]
        private List<GameObject> centerVerticalRays = new List<GameObject>();
        [SerializeField]
        private List<GameObject> centerHorizontalRays = new List<GameObject>();
        [SerializeField]
        private List<GameObject> centerCenterRays = new List<GameObject>();


        [SerializeField] private CubeState cubeState;

        private int layerMask = 1 << 8; // this layerMask is for the faces of the cube only
        public GameObject emptyGO;
        private bool leftOrRight = false;
        private bool upOrDown = false;

        public bool isRead = true;

        // Start is called before the first frame update
        void Start()
        {
            SetRayTransforms();

            //cubeState = FindObjectOfType<CubeState>();
            //cubeMap = FindObjectOfType<CubeMap>();
            Invoke(nameof(ReadState), .2f);
            CubeState.started = true;


        }

        private void Update()
        {

            cubeState.TestAlignment();
        }

        public void ReadState()
        {
            //cubeState.TestAlignment();
            isRead = false;
            // set the state of each position in the list of sides so we know
            // what color is in what position
            leftOrRight = false;
            cubeState.Up = ReadFace(_upRays, tUp);
            cubeState.Down = ReadFace(_downRays, tDown);
            cubeState.Left = ReadFace(_leftRays, tLeft);
            cubeState.Right = ReadFace(_rightRays, tRight);
            cubeState.Front = ReadFace(_frontRays, tFront);
            cubeState.Back = ReadFace(_backRays, tBack);
            cubeState.CenterHorizontal = ReadFace(centerHorizontalRays, tBack);
            cubeState.CenterVertical = ReadFace(centerVerticalRays, tBack);
            cubeState.CenterCenter=ReadFace(centerCenterRays, tBack);
            // update the map with the found positions
            //cubeMap.Set();
            isRead = true;
        }


        void SetRayTransforms()
        {
            // populate the ray lists with raycasts eminating from the transform, angled towards the cube.
            upOrDown = true;
            _upRays = BuildRays(tUp, new Vector3(90, 90, 0));
            _downRays = BuildRays(tDown, new Vector3(270, 90, 0));
            upOrDown = false;
            _frontRays = BuildRays(tFront, new Vector3(0, 90, 0));
            _backRays = BuildRays(tBack, new Vector3(0, 270, 0));
            leftOrRight = true;
            _leftRays = BuildRays(tLeft, new Vector3(0, 180, 0));
            _rightRays = BuildRays(tRight, new Vector3(0, 0, 0));
            leftOrRight = false;
            centerCenterRays = new List<GameObject> {_upRays[3], _upRays[4], _upRays[5], _rightRays[1], _rightRays[4], _rightRays[7], _downRays[3], _downRays[4], _downRays[5],
                _leftRays[1], _leftRays[4], _leftRays[7] };
        }


        List<GameObject> BuildRays(Transform rayTransform, Vector3 direction)
        {
            // The ray count is used to name the rays so we can be sure they are in the right order.
            int rayCount = 0;
            List<GameObject> rays = new List<GameObject>();
            // This creates 9 rays in the shape of the side of the cube with
            // Ray 0 at the top left and Ray 8 at the bottom right:
            //  |0|1|2|
            //  |3|4|5|
            //  |6|7|8|

            for (int y = 1; y > -2; y--)
            {
                for (int x = -1; x < 2; x++)
                {
                    Vector3 startPos = new Vector3(rayTransform.position.x + x,
                                                    rayTransform.position.y + y,
                                                    rayTransform.position.z);
                    GameObject rayStart = Instantiate(emptyGO, startPos, Quaternion.identity, rayTransform);
                    rayStart.name = rayCount.ToString();
                    rays.Add(rayStart);
                    rayCount++;
                    if (!leftOrRight)
                    {
                        if (x == 0)
                        {
                            centerVerticalRays.Add(rayStart);
                        }
                    }
                    if (!upOrDown)
                    {
                        if (y == 0)
                        {
                            centerHorizontalRays.Add(rayStart);
                        }
                    }
                }
            }
            rayTransform.localRotation = Quaternion.Euler(direction);
            return rays;

        }

        public List<GameObject> ReadFace(List<GameObject> rayStarts, Transform rayTransform)
        {
            List<GameObject> facesHit = new List<GameObject>();
            if (rayStarts == centerHorizontalRays)
            {
                foreach (GameObject rayStart in rayStarts)
                {
                    Vector3 ray = rayStart.transform.position;
                    RaycastHit hit;

                    if (Physics.Raycast(ray, Vector3.forward, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.forward * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    if (Physics.Raycast(ray, Vector3.back, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.back * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    if (Physics.Raycast(ray, Vector3.left, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.left * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    if (Physics.Raycast(ray, Vector3.right, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.right * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                }

                return facesHit;
            }
            if (rayStarts == centerVerticalRays)
            {
                foreach (GameObject rayStart in rayStarts)
                {
                    Vector3 ray = rayStart.transform.position;
                    RaycastHit hit;

                    if (Physics.Raycast(ray, Vector3.left, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.left * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    if (Physics.Raycast(ray, Vector3.right, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.right * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    if (Physics.Raycast(ray, Vector3.up, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.up * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    if (Physics.Raycast(ray, Vector3.down, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.right * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                }
                return facesHit;
            }
            if (rayStarts == centerCenterRays)
            {
                foreach (GameObject rayStart in rayStarts)
                {
                    Vector3 ray = rayStart.transform.position;
                    RaycastHit hit;

                    if (Physics.Raycast(ray, Vector3.back, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.left * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    else if (Physics.Raycast(ray, Vector3.up, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.right * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    //Right
                    else if (Physics.Raycast(ray, Vector3.forward, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.up * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                    //Up
                    else if (Physics.Raycast(ray, Vector3.down, out hit, Mathf.Infinity, layerMask))
                    {
                        Debug.DrawRay(ray, Vector3.right * hit.distance, Color.yellow);
                        facesHit.Add(hit.collider.gameObject);
                        //print(hit.collider.gameObject.name);
                    }
                }
                return facesHit;
            }

            foreach (GameObject rayStart in rayStarts)
            {
                Vector3 ray = rayStart.transform.position;
                RaycastHit hit;

                // Does the ray intersect any objects in the layerMask?
                if (Physics.Raycast(ray, rayTransform.forward, out hit, Mathf.Infinity, layerMask))
                {
                    Debug.DrawRay(ray, rayTransform.forward * hit.distance, Color.yellow);
                    facesHit.Add(hit.collider.gameObject);
                    //print(hit.collider.gameObject.name);
                }
                else
                {
                    Debug.DrawRay(ray, rayTransform.forward * 1000, Color.green);
                }
            }
            return facesHit;
        }

    }
}
