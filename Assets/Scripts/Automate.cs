using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubicsCube
{
    public class Automate : MonoBehaviour
    {
        public static List<string> moveList = new List<string>() { };
        private readonly List<string> allMoves = new List<string>()
        { "U", "D", "L", "R", "F", "B",
          "U2", "D2", "L2", "R2", "F2", "B2",
          "U'", "D'", "L'", "R'", "F'", "B'"
        };

        private CubeState cubeState;
        private ReadCube readCube;

        // Start is called before the first frame update
        void Start()
        {
            cubeState = FindObjectOfType<CubeState>();
            readCube = FindObjectOfType<ReadCube>();
        }

        // Update is called once per frame
        void Update()
        {
            if (moveList.Count > 0 && !CubeState.autoRotating && CubeState.started)
            {
                //Do the move at the first index;
                DoMove(moveList[0]);

                // remove the move at the first index
                moveList.Remove(moveList[0]);
            }
        }

        public void Shuffle()
        {
            List<string> moves = new List<string>();
            int shuffleLength = Random.Range(10, 30);
            for (int i = 0; i < shuffleLength; i++)
            {
                int randomMove = Random.Range(0, allMoves.Count);
                moves.Add(allMoves[randomMove]);
            }
            moveList = moves;
        }


        void DoMove(string move)
        {
            readCube.ReadState();
            CubeState.autoRotating = true;
            if (move == "U")
            {
                RotateSide(cubeState.Up, -90);
            }
            if (move == "U'")
            {
                RotateSide(cubeState.Up, 90);
            }
            if (move == "U2")
            {
                RotateSide(cubeState.Up, -180);
            }
            if (move == "D")
            {
                RotateSide(cubeState.Down, -90);
            }
            if (move == "D'")
            {
                RotateSide(cubeState.Down, 90);
            }
            if (move == "D2")
            {
                RotateSide(cubeState.Down, -180);
            }
            if (move == "L")
            {
                RotateSide(cubeState.Left, -90);
            }
            if (move == "L'")
            {
                RotateSide(cubeState.Left, 90);
            }
            if (move == "L2")
            {
                RotateSide(cubeState.Left, -180);
            }
            if (move == "R")
            {
                RotateSide(cubeState.Right, -90);
            }
            if (move == "R'")
            {
                RotateSide(cubeState.Right, 90);
            }
            if (move == "R2")
            {
                RotateSide(cubeState.Right, -180);
            }
            if (move == "F")
            {
                RotateSide(cubeState.Front, -90);
            }
            if (move == "F'")
            {
                RotateSide(cubeState.Front, 90);
            }
            if (move == "F2")
            {
                RotateSide(cubeState.Front, -180);
            }
            if (move == "B")
            {
                RotateSide(cubeState.Back, -90);
            }
            if (move == "B'")
            {
                RotateSide(cubeState.Back, 90);
            }
            if (move == "B2")
            {
                RotateSide(cubeState.Back, -180);
            }
        }


        void RotateSide(List<GameObject> side, float angle)
        {
            // automatically rotate the side by the angle
            PivotRotation pr = side[4].transform.parent.GetComponent<PivotRotation>();
            pr.StartAutoRotate(side, angle);
        }

    }
}