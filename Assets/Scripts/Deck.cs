using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    // Public variable to store the card prefab
    public GameObject cardPrefab;

    // List to store the Card objects in the deck
    public List<Card> cards;

    // Creates and initializes a standard deck of 52 cards
    public void InitializeDeck()
    {
        DestroyAllChildObjects();
        cards = new List<Card>();

        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit))) 
        {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank))) 
            {                
                // Get the corresponding card prefab for the suit and rank
                string prefabName = $"{suit.ToString()}_{rank.ToString()}";
                GameObject cardPrefab = Resources.Load<GameObject>($"CardPrefabs/{prefabName}");

                if (cardPrefab != null) {
                    // Create a new card object
                    GameObject cardObject = Instantiate(cardPrefab, transform);
                    Card card = cardObject.GetComponent<Card>();

                    // Set the suit and rank of the card
                    card.suit = suit;
                    card.rank = rank;

                    // Adjust the local scale of the card object
                    cardObject.transform.localScale = Vector3.one;

                    // Add the card to the deck
                    cards.Add(card);
                } 
                else 
                {
                    Debug.LogWarning($"Card prefab {prefabName} not found.");
                }
            }
        }
    }

    // Prints the rank and suit of each card in the deck
    public void PrintDeck()
    {
        foreach (Card card in cards) {
            Debug.Log($"Card: {card.rank} of {card.suit}");
        }
    }

    // Shuffles the deck randomly
    public void Shuffle()
    {
        // Use Fisher-Yates shuffle algorithm to shuffle the deck
        for (int i = 0; i < cards.Count; i++) {
            int randomIndex = Random.Range(i, cards.Count);
            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }
    }

    // Draws and removes a card from the deck
    public Card DrawCard()
    {
        if (cards.Count == 0) {
            Debug.LogWarning("Deck is empty. Cannot draw a card.");
            return null;
        }

        Card card = cards[0];
        cards.RemoveAt(0);

        // Play a sound here
        AudioSource audioSource = GetComponent<AudioSource>();
        audioSource.Play();

        return card;
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
