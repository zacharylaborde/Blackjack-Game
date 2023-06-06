using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class SoundToggle : MonoBehaviour
{
    public AudioSource clickAudioSource;

    private Toggle toggle;

    private void Awake()
    {
        toggle = GetComponent<Toggle>();
        toggle.onValueChanged.AddListener(HandleToggleChange);
    }

    private void HandleToggleChange(bool isOn)
    {
        // Always play the click sound
        clickAudioSource.Play();

        // Start a coroutine to apply the new toggle value after a delay
        StartCoroutine(ApplyToggleValueAfterDelay(isOn, 0.2f));  // 0.2 seconds delay
    }

    private IEnumerator ApplyToggleValueAfterDelay(bool isOn, float delay)
    {
        yield return new WaitForSeconds(delay);

        // Apply the new toggle value
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource source in audioSources)
        {
            if (source != clickAudioSource) // Ignore the click sound
            {
                if (isOn)
                {
                    source.UnPause(); // Resume playing the audio
                }
                else
                {
                    source.Pause(); // Pause the audio
                }
            }
        }
    }
}
