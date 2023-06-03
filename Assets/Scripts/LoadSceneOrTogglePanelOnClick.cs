using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneOrTogglePanelOnClick : MonoBehaviour
{
    public Button yourButton; // Drag and drop your button here via the Unity Inspector.
    public string yourSceneName; // The name of the scene you want to load.
    public float delay = 2f; // The delay before the scene loads. You can set this value in the inspector.
    public GameObject yourPanel; // Drag and drop your panel here via the Unity Inspector.
    public bool isSceneLoading = true; // If true, load a scene. If false, toggle a panel.

    void Start()
    {
        // Add an event listener to the button's onClick event.
        yourButton.onClick.AddListener(() => StartCoroutine(PerformActionAfterDelay()));
    }

    // Perform action (load scene or toggle panel) after delay
    IEnumerator PerformActionAfterDelay()
    {
        // Wait for the specified delay.
        yield return new WaitForSeconds(delay);

        // If isSceneLoading is true, load the scene. If it's false, toggle the panel.
        if (isSceneLoading)
        {
            SceneManager.LoadScene(yourSceneName);
        }
        else
        {
            yourPanel.SetActive(!yourPanel.activeSelf);
        }
    }
}
