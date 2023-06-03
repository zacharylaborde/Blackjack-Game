using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitGameOnClick : MonoBehaviour
{
    public Button yourButton; // Drag and drop your button here via the Unity Inspector.

    void Start()
    {
        // Add an event listener to the button's onClick event.
        yourButton.onClick.AddListener(QuitGame);
    }

    void QuitGame()
    {
        // Quit the game.
        Application.Quit();
    }
}