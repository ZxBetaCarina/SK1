using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundSetting : MonoBehaviour
{
    [SerializeField] private List<AudioSource> audioSource;
    [SerializeField] List<Sprite> _audio_Buttons;

    private Image _button_Image;
    private bool _is_Muted = false;

    private void Start()
    {
        _button_Image = GetComponent<Image>();
    }

        public void StartOrStopPlaying()
    {
        _is_Muted = !_is_Muted;

        foreach (AudioSource audio in audioSource)
        {
            audio.mute = _is_Muted;
        }

        _button_Image.sprite = _is_Muted ? _audio_Buttons[1] : _audio_Buttons[0];
    }
}
