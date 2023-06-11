using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Fields
    private List<Card> hand;      // Stores the player's current hand of cards
    public double bankroll;          // Stores the player's available funds
    public double currentBet;        // Stores the player's current bet amount
    public float cardDealSpeed = 4.0f;  // Speed at which the card moves across the board

    // Properties
    public List<Card> Hand {
        get { return hand; }
    }

    public double Bankroll {
        get { return bankroll; }
        set { bankroll = value; }
    }

    public double CurrentBet {
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
        StartCoroutine(MoveCardAcrossBoard(card));
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

    // Coroutine to move the card across the board
    private IEnumerator MoveCardAcrossBoard(Card card)
    {
        float elapsedTime = 0.0f;
        Vector3 startPosition = card.transform.position;
        Vector3 targetPosition = transform.position;

        while (elapsedTime < 1.0f) {
            elapsedTime += Time.deltaTime * cardDealSpeed;
            card.transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime);
            yield return null;
        }

        CenterCardHandOnBoard(); // Recenter the card hand after the card has reached its final position
    }

    // Calculates the total value of the player's hand
    public int CalculateHandValue()
    {
        int totalValue = 0;
        int aceCount = 0;

        foreach (Card card in hand) {
            totalValue += card.Value;

            if (card.rank == Card.Rank.Ace) {
                aceCount++;
            }
        }

        // Adjust the value of Aces if needed
        while (aceCount > 0 && totalValue > 21) {
            totalValue -= 10;
            aceCount--;
        }

        return totalValue;
    }

    // Resets the player's hand
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
