//Imports
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
    // Object of the Shadow
    Shadow shadow;
    // Object of the AudioSource
    AudioSource audioSourceClick;
    AudioSource audioSourceHover;

    // Object of the Text
    public TextMeshProUGUI aboutText;


    // Start is called before the first frame update
    void Start()
    {
        // Gets the component of the animator
        animator = GetComponent<Animator>();
        // Gets the component of the Shadow
        shadow = GetComponent<Shadow>();
        // Get tje components of the audioSource
        AudioSource[] audioSources = GetComponents<AudioSource>();
        audioSourceClick = audioSources[0];
        audioSourceHover = audioSources[1];
        // Set to false by default
        shadow.enabled = false;

        if (aboutText != null) 
        {
            aboutText.enabled = false; 
        }
    }

    // On button hovered
    public void OnPointerEnter(PointerEventData eventData)
    {
        // Sets the animator componenet "isHovered" to true if button is hovered
        animator.SetBool("IsHovered", true);

        //On hover, sets enabled shadow to true
        shadow.enabled = true;

        // Play hover audio source
        audioSourceHover.Play();

        if (aboutText != null)
        {
            aboutText.enabled = true;
        }
    }

    // When button is not hovered
    public void OnPointerExit(PointerEventData eventData)
    {
        // Sets the animator componenet "isHovered" to false if button is not hovered
        animator.SetBool("IsHovered", false);

        //when not hovered, sets enabled shadow to false
        shadow.enabled = false;

        if (aboutText != null) 
        {
            aboutText.enabled = false; 
        }
    }

    // When button is clicked
    public void OnPointerClick(PointerEventData eventData)
    {
        // Play audio source
        audioSourceClick.Play();
    }

}
