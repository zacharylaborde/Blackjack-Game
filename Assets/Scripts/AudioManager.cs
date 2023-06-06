using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public float Volume { get; private set; } = 1f; // default volume

    // Called before Start()
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetVolume(float volume)
    {
        Volume = volume;
        AudioListener.volume = Volume;
    }

    public void ToggleMute(bool isMuted)
    {
        AudioListener.volume = isMuted ? 0 : Volume;
    }
}
