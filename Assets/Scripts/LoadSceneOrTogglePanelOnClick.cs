using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneOrTogglePanelOnClick : MonoBehaviour
{
    public Button yourButton;
    public string yourSceneName;
    public float delay = 2f;
    public GameObject yourPanel;
    public GameObject panelToDisable;
    public bool isSceneLoading = true;
    
    private AudioSource audioSource;

    void Start()
    {
        yourButton.onClick.AddListener(() => StartCoroutine(PerformActionAfterDelay()));

        // Get the audio source on the button.
        audioSource = yourButton.GetComponent<AudioSource>();
    }

    IEnumerator PerformActionAfterDelay()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }

        yield return new WaitForSeconds(delay);

        if (isSceneLoading)
        {
            SceneManager.LoadScene(yourSceneName);
        }
        else
        {
            if (panelToDisable != null)
            {
                panelToDisable.SetActive(false);
            }

            if (yourPanel != null)
            {
                yourPanel.SetActive(!yourPanel.activeSelf);
            }
        }
    }
}