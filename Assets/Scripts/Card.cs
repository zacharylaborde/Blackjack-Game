using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents a playing card with its suit and rank.
/// </summary>
public class Card : MonoBehaviour
{
    // Enum representing the suits of the card
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    // Enum representing the ranks of the card
    public enum Rank
    {
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    // Public variables for suit and rank of the card
    public Suit suit;
    public Rank rank;

    // Private variables for value and Ace toggle
    private int value;
    private bool isAceHigh = true;

    // Property to retrieve the value of the card
    public int Value
    {
        get { return GetCardValue(); }
    }

    // Property to set or get whether Ace is considered high
    public bool IsAceHigh
    {
        get { return isAceHigh; }
        set { isAceHigh = value; }
    }

    // Method to calculate the value of the card
    private int GetCardValue()
    {
        if (rank == Rank.Ace)
        {
            if (isAceHigh)
                return 11;
            else
                return 1;
        }

        if (rank >= Rank.Ten)
            return 10;

        return (int)rank;
    }
}