using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    // Fields
    private List<Card> hand;       // Stores the dealer's current hand of cards

    // Properties
    public List<Card> Hand {
        get { return hand; }
    }

    // Initialize the dealer's fields
    void Start()
    {
        hand = new List<Card>();
    }

    // Adds a card to the dealer's hand
    public void AddCardToHand(Card card)
    {
        hand.Add(card);
        CenterCardHandOnBoard();
    }

    // Centers the card hand on the board by calculating the positions of the cards.
    private void CenterCardHandOnBoard()
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

    // Calculates the total value of the dealer's hand
    public int CalculateHandValue(bool includeHoleCard)
    {
        int totalValue = 0;
        bool hasAce = false;

        // Iterate over each card in the hand
        for (int i = 0; i < hand.Count; i++) {
            // Check if the current card is the hole card and exclude it if includeHoleCard is false
            if (!includeHoleCard && i == 0)
                continue;

            Card card = hand[i];
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

    // Resets the dealer's hand
    public void ResetHand()
    {
        hand.Clear();
        DestroyAllChildObjects();
    }

    // Destroy all child objects under the current object
    private void DestroyAllChildObjects()
    {
        int childCount = transform.childCount;

        for (int i = childCount - 1; i >= 0; i--) {
            GameObject childObject = transform.GetChild(i).gameObject;
            Destroy(childObject);
        }
    }
}
