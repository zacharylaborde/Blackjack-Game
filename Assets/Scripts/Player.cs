using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Fields
    private List<Card> hand;      // Stores the player's current hand of cards
    public int bankroll;          // Stores the player's available funds
    public int currentBet;        // Stores the player's current bet amount

    // Properties
    public List<Card> Hand {
        get { return hand; }
    }

    public int Bankroll {
        get { return bankroll; }
        set { bankroll = value; }
    }

    public int CurrentBet {
        get { return currentBet; }
        set { currentBet = value; }
    }

    // Initialize the player's fields
    void Start()
    {
        hand = new List<Card>();
        bankroll = 0;
        currentBet = 0;
    }

    // Adds a card to the player's hand
    public void AddCardToHand(Card card)
    {
        hand.Add(card);
        CenterCardHandOnBoard();
    }

    // Centers the card hand on the board by calculating the positions of the cards.
    public void CenterCardHandOnBoard()
    {
        float cardSpacing = 0.12f; // Adjust this value to control the spacing between cards

        // Calculate the total width of the card hand
        float totalWidth = (hand.Count - 1) * cardSpacing;

        // Calculate the starting position for the first card in the hand to center it on the board
        Vector3 startPosition = transform.position - new Vector3(totalWidth / 2.0f, 0.0f, 0.0f);

        // Position each card in the hand based on the calculated starting position and card spacing
        for (int i = 0; i < hand.Count; i++) {
            Vector3 cardPosition = startPosition + new Vector3(i * cardSpacing, 0.0f, 0.0f);
            hand[i].transform.position = cardPosition;
        }
    }

    // Calculates the total value of the player's hand
    public int CalculateHandValue()
    {
        int totalValue = 0;
        bool hasAce = false;

        foreach (Card card in hand) {
            totalValue += card.Value;

            if (card.rank == Card.Rank.Ace) {
                hasAce = true;
            }
        }

        // If the hand has an Ace and the total value is <= 11, treat Ace as 11
        if (hasAce && totalValue <= 11) {
            totalValue += 10;
        }

        return totalValue;
    }

    // Resets the player's hand
    public void ResetHand()
    {
        hand.Clear();
    }
}
