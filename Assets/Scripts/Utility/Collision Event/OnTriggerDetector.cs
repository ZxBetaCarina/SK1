using UnityEngine;
using UnityEngine.Events;

namespace AstekUtility
{
    public class OnTriggerDetector : MonoBehaviour
    {
        public UnityEvent<Collider> TriggerEnter;
        public UnityEvent<Collider> TriggerStay;
        public UnityEvent<Collider> TriggerExit;

        private void OnTriggerEnter(Collider other)
        {
            TriggerEnter?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            TriggerStay?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            TriggerExit?.Invoke(other);
        }

    }
}