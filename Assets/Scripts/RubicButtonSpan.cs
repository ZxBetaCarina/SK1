using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RubicsCube
{
    public class RubicButtonSpan : MonoBehaviour, IPointerDownHandler
    {
        [SerializeField] private int _movesToDisable = 3;
        Button _button;

        private void OnEnable()
        {
            _button = GetComponent<Button>();
            _button.interactable = true;
            _movesToDisable = 3;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_button.interactable)
                _movesToDisable--;
        }

        private void DisableButton()
        {
            _button.interactable = false;
        }
    }
}
