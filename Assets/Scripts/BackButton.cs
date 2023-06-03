using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BackButton : MonoBehaviour
{
    public GameObject mainMenuPanel; // Assign your main menu panel in inspector
    public GameObject currentPanel;  // Assign your current panel in inspector
    public float delayTime = 1f;     // Time delay before switching panels

    public void StartGoBackToMainMenu()
    {
        StartCoroutine(GoBackToMainMenu());
    }

    IEnumerator GoBackToMainMenu()
    {
        yield return new WaitForSeconds(delayTime);
        currentPanel.SetActive(false); // Hide the current panel
        mainMenuPanel.SetActive(true);  // Show the main menu panel
    }
}
