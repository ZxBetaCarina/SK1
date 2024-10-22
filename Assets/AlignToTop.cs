using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RubicsCube
{
    public class AlignToTop : MonoBehaviour
    {
        [SerializeField] private LayerMask alignmentLayer;
        public void Align()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!Physics.Raycast(transform.position, transform.up, out RaycastHit hit, Mathf.Infinity, alignmentLayer))
                {
                    transform.Rotate(0, 0, 90);
                }
            }
        }
    }
}
