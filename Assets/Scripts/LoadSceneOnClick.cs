using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneOnClick : MonoBehaviour
{
    public Button yourButton; // Drag and drop your button here via the Unity Inspector.
    public string yourSceneName; // The name of the scene you want to load.
    public float delay = 2f; // The delay before the scene loads. You can set this value in the inspector.

    void Start()
    {
        // Add an event listener to the button's onClick event.
        yourButton.onClick.AddListener(() => StartCoroutine(LoadSceneAfterDelay()));
    }

    // Load scene after delay
    IEnumerator LoadSceneAfterDelay()
    {
        // Wait for the specified delay.
        yield return new WaitForSeconds(delay);

        // Load the scene with the specified name.
        SceneManager.LoadScene(yourSceneName);
    }
}
