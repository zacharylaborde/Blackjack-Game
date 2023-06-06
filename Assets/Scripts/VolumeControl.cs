using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class VolumeControl : MonoBehaviour
{
    private Slider _slider;

    private void Awake()
    {
        _slider = GetComponent<Slider>();
        _slider.onValueChanged.AddListener(HandleSliderValueChange);
    }

    private void Start()
    {
        _slider.value = AudioManager.Instance.Volume;  // Initialize the slider value with the current volume
    }

    private void HandleSliderValueChange(float value)
    {
        AudioManager.Instance.SetVolume(value);
    }
}
