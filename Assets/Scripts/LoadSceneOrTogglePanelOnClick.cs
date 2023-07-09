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
                Debug.Log("Your panel name: " + yourPanel.name);

                // Prepare to reset the scrollbar of the About Panel UI
                if (yourPanel.name.Equals("About Panel") || yourPanel.name.Equals("How To Play Panel"))
                {
                    // Traverse the tree to get the scrollbar game object
                    GameObject scrollbar = yourPanel.transform.GetChild(1).gameObject.transform.GetChild(1).gameObject;

                    // Get the scrollbar component from the game object
                    Scrollbar sb = scrollbar.GetComponentInChildren<Scrollbar>();

                    // Reset it to 1 so it will always be at the top when activated
                    sb.value = 1;
                }
            }
        }
    }
}