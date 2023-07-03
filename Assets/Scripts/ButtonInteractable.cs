using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonInteractable : MonoBehaviour
{
    private GameManager gameManager;
    public int betAmount;
    public AudioSource audioSourceClick;
    public AudioSource invalidChipClick;

    // Flag to control script functionality
    public bool isInteractable = true;

    private void Start()
    {
        // Find and get the reference to the GameManager script
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnMouseDown()
    {
        // Check if the script is interactable
        if (isInteractable) {
            // Call the method in the GameManager script to interact with the button
            gameManager.PlaceBet(betAmount);
            audioSourceClick.Play();
        }
        else
        {
            invalidChipClick.Play();
        }
    }
}
