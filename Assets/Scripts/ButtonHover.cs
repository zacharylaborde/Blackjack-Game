using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    // Object of the animator
    Animator animator;
    // Check if the button has an animator
    public bool hasAnimator = false;
    // Object of the Shadow
    Shadow shadow;
    // Object of the AudioSource
    AudioSource audioSourceClick;
    AudioSource audioSourceHover;

    // Object of the Text
    public TextMeshProUGUI aboutText;

    // Reference to the quit confirmation dialog
    public GameObject quitConfirmationDialog;

    void Start()
    {
        // Gets the component of the animator if it exists
        if (hasAnimator) {
            animator = GetComponent<Animator>();
        }

        // Gets the component of the Shadow
        shadow = GetComponent<Shadow>();
        // Get the components of the audioSource
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSourceClick = audioSources[0];
        audioSourceHover = audioSources[1];
        // Set to false by default
        shadow.enabled = false;

        if (aboutText != null) {
            aboutText.enabled = false;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Check if the quit confirmation dialog is active
        if (quitConfirmationDialog != null && quitConfirmationDialog.activeSelf) {
            return; // Exit the method without performing hover actions
        }

        // Sets the animator component "isHovered" to true if button is hovered and has animator
        if (hasAnimator) {
            animator.SetBool("IsHovered", true);
        }

        // On hover, sets enabled shadow to true
        shadow.enabled = true;

        // Play hover audio source
        audioSourceHover.Play();

        if (aboutText != null) {
            aboutText.enabled = true;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Check if the quit confirmation dialog is active
        if (quitConfirmationDialog != null && quitConfirmationDialog.activeSelf) {
            return; // Exit the method without performing hover actions
        }

        // Sets the animator component "isHovered" to false if button is not hovered and has animator
        if (hasAnimator) {
            animator.SetBool("IsHovered", false);
        }

        // When not hovered, sets enabled shadow to false
        shadow.enabled = false;

        if (aboutText != null) {
            aboutText.enabled = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Check if the quit confirmation dialog is active
        if (quitConfirmationDialog != null && quitConfirmationDialog.activeSelf) {
            return; // Exit the method without performing hover actions
        }

        // Play audio source
        audioSourceClick.Play();
    }
}
