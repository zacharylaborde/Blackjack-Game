using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dealer : MonoBehaviour
{
    // Fields
    private List<Card> hand;       // Stores the dealer's current hand of cards
    public float cardDealSpeed = 4.0f;  // Speed at which the card moves across the board

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

    // Calculates the total value of the dealer's hand
    public int CalculateHandValue(bool includeHoleCard)
    {
        int totalValue = 0;
        int aceCount = 0;

        // Iterate over each card in the hand
        for (int i = 0; i < hand.Count; i++) {
            // Check if the current card is the hole card and exclude it if includeHoleCard is false
            if (!includeHoleCard && i == 0)
                continue;

            Card card = hand[i];
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
