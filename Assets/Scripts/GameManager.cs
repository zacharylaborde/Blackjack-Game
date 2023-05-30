using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Fields
    private Deck deck;
    private Player player;
    private Dealer dealer;
    private int initialBankroll = 1000;

    // Constants
    private const int MinimumBet = 1;
    private const int MaximumBet = 500;

    // Game Buttons
    public Button dealButton;
    public Button hitButton;
    public Button standButton;
    public Button resetBetButton;

    public TextMeshProUGUI bankrollText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI playerHandText;
    public TextMeshProUGUI dealerHandText;

    public List<ButtonInteractable> buttonInteractables;    // List of ButtonInteractable scripts
    public List<TextMeshProUGUI> chipTexts;                 // List of Chip Texts

    private void Start()
    {
        StartGame();
    }

    // Start the game
    void StartGame()
    {
        // Initialize the deck
        deck = GameObject.Find("Deck").GetComponent<Deck>();

        // Initialize the player
        player = GameObject.Find("Player").GetComponent<Player>();
        player.Bankroll = initialBankroll;

        // Initialize the dealer
        dealer = GameObject.Find("Dealer").GetComponent<Dealer>();

        // Start the first round
        StartNewRound();
    }

    // Method to disable all ButtonInteractable scripts
    void DisableAllChipButtonInteractables()
    {
        resetBetButton.gameObject.SetActive(false);
        resetBetButton.interactable = false;

        foreach (var buttonInteractable in buttonInteractables) {
            if (buttonInteractable != null) {
                buttonInteractable.isInteractable = false;
            }
        }
        foreach (var chipText in chipTexts) {
            if (chipText != null) {
                chipText.gameObject.SetActive(false);
            }
        }
    }

    // Method to enable all ButtonInteractable scripts
    void EnableAllChipButtonInteractables()
    {
        resetBetButton.gameObject.SetActive(true);
        resetBetButton.interactable = true;

        foreach (var buttonInteractable in buttonInteractables) {
            if (buttonInteractable != null) {
                buttonInteractable.isInteractable = true;
            }
        }
        foreach(var chipText in chipTexts) {
            if (chipText != null) {
                chipText.gameObject.SetActive(true);
            }
        }
    }

    // Deal the initial two cards to the player and dealer
    public void DealInitialCards()
    {
        player.ResetHand();
        dealer.ResetHand();

        // Disable Betting        
        DisableAllChipButtonInteractables();

        // Enable the text for player and dealer hands
        playerHandText.gameObject.SetActive(true);
        dealerHandText.gameObject.SetActive(true);

        // Deal FIRST card to the PLAYER
        Card playerCard1 = deck.DrawCard();
        playerCard1.transform.SetParent(player.transform, true);
        player.AddCardToHand(playerCard1);        

        // Deal FIRST card to the DEALER
        Card dealerCard1 = deck.DrawCard();
        dealerCard1.transform.SetParent(dealer.transform, true);
        dealerCard1.transform.Rotate(0f, 0f, 180f); // Rotate the card by 180 degrees on the Z-axis
        dealer.AddCardToHand(dealerCard1);

        // Deal SECOND card to the PLAYER
        Card playerCard2 = deck.DrawCard();
        playerCard2.transform.SetParent(player.transform, true);
        player.AddCardToHand(playerCard2);

        // Deal SECOND card to the DEALER
        Card dealerCard2 = deck.DrawCard();
        dealerCard2.transform.SetParent(dealer.transform, true);
        dealer.AddCardToHand(dealerCard2);

        // Disable Deal Button
        dealButton.gameObject.SetActive(false);
        dealButton.interactable = false;

        // Enable Hit and Stand Buttons
        hitButton.gameObject.SetActive(true);
        hitButton.interactable = true;
        standButton.gameObject.SetActive(true);
        standButton.interactable = true;


    }

    // Allows the player to place a bet within the predefined limits
    public void PlaceBet(int amount)
    {
        if (player.Bankroll > 0) {

            int newBetAmount = player.CurrentBet + amount;
            int betDifference = amount - player.Bankroll;

            // Check if the new bet amount is within the maximum limit and player has enough funds
            if (newBetAmount <= MaximumBet && (player.Bankroll - amount) >= 0) {
                player.CurrentBet = newBetAmount;
                player.Bankroll -= amount;

                // Update the bet amount and bankroll text on the UI
                bankrollText.text = "Balance: $" + player.Bankroll.ToString();
                betText.text = "Bet: $" + player.CurrentBet.ToString();
            }
            // Check if the new bet amount is within the maximum limit and exceeds player's balance
            else if (newBetAmount <= MaximumBet && (player.Bankroll - amount) < 0) {
                player.CurrentBet += player.Bankroll;
                player.Bankroll = 0;

                // Update the bet amount and bankroll text on the UI
                bankrollText.text = "Balance: $" + player.Bankroll.ToString();
                betText.text = "Bet: $" + player.CurrentBet.ToString();
            }
            else {
                Debug.LogWarning("Cannot exceed the maximum bet.");
            }
        }
        else {
            Debug.LogWarning("Cannot have a negative balance.");
        }

        // Check if the current bet meets the condition for enabling the Deal button
        if (player.CurrentBet >= 1) {
            dealButton.gameObject.SetActive(true);
            dealButton.interactable = true;
        }
    }

    // Resets the current bet amount to 0 and adds the previous bet amount back to the player's bankroll.
    public void ResetBet()
    {
        int totalBetAmount = player.CurrentBet;

        // Reset the current bet amount to 0
        player.CurrentBet = 0;

        // Add the previous bet amount back to the player's bankroll
        player.Bankroll += totalBetAmount;

        // Update the bankroll and bet amount text on the UI
        bankrollText.text = "Balance: $" + player.Bankroll.ToString();
        betText.text = "Bet: $" + player.CurrentBet.ToString();

        // Disable Deal button because CurrentBet is 0
        dealButton.gameObject.SetActive(false);
        dealButton.interactable = false;
    }

    // Deals an additional card to the player
    public void PlayerHit()
    {
        if (player.Hand.Count < 11) {
            Card card = deck.DrawCard();
            card.transform.SetParent(player.transform, true);
            player.AddCardToHand(card);
        }
        else {
            Debug.LogWarning("Cannot exceed max card limit.");
        }
    }

    // Proceeds to the dealer's turn
    public void PlayerStand()
    {
        DealerTurn();
        EvaluateRoundResult();
        UpdateBankroll();
        StartNewRound();
    }

    // Controls the dealer's actions, including hitting or standing based on predefined rules
    void DealerTurn()
    {
        while (dealer.CalculateHandValue() < 17) {
            Card card = deck.DrawCard();
            dealer.AddCardToHand(card);
        }
    }

    // Compares the player's and dealer's hand values to determine the round result
    void EvaluateRoundResult()
    {
        int playerHandValue = player.CalculateHandValue();
        int dealerHandValue = dealer.CalculateHandValue();

        if (playerHandValue > 21) {
            // Player busts, dealer wins
            Debug.Log("Player busts! Dealer wins.");
        }
        else if (dealerHandValue > 21) {
            // Dealer busts, player wins
            Debug.Log("Dealer busts! Player wins.");
        }
        else if (playerHandValue == dealerHandValue) {
            // It's a tie
            Debug.Log("It's a tie.");
        }
        else if (playerHandValue > dealerHandValue) {
            // Player wins
            Debug.Log("Player wins.");
        }
        else {
            // Dealer wins
            Debug.Log("Dealer wins.");
        }
    }

    // Updates the player's bankroll based on the round result
    void UpdateBankroll()
    {
        int playerHandValue = player.CalculateHandValue();
        int dealerHandValue = dealer.CalculateHandValue();

        if (playerHandValue > 21) {
            // Player busts, lose the bet
            player.Bankroll += player.CurrentBet;
        }
        else if (dealerHandValue > 21) {
            // Dealer busts, win the bet
            player.Bankroll += player.CurrentBet * 2;
        }
        else if (playerHandValue == dealerHandValue) {
            // It's a tie, return the bet
            player.Bankroll += player.CurrentBet;
        }
        else if (playerHandValue > dealerHandValue) {
            // Player wins, win the bet
            player.Bankroll += player.CurrentBet * 2;
        }
        else {
            // Dealer wins, lose the bet
            player.Bankroll += player.CurrentBet;
        }
    }

    // Prompts the player to start a new round or quit the game
    void StartNewRound()
    {
        // Disable the text for player and dealer hands
        playerHandText.gameObject.SetActive(false);
        dealerHandText.gameObject.SetActive(false);

        if (player.Bankroll >= MinimumBet) {
            // Reset player's current bet
            player.CurrentBet = 0;

            // Enable Betting
            EnableAllChipButtonInteractables();

            // Reset player and dealer hands
            player.ResetHand();
            dealer.ResetHand();

            // Reinitialize the deck
            deck.InitializeDeck();
            deck.Shuffle();

            bankrollText.text = "Balance: $" + player.Bankroll.ToString();
            Debug.Log("Bankroll: " + player.Bankroll);
            Debug.Log("Place your bet (between " + MinimumBet + " and " + MaximumBet + ").");          

            // Prompt the player to place a bet and continue the game
        }
        else {
            // Player does not have enough funds to continue playing
            Debug.Log("Game over. Insufficient funds to continue.");
            // End the game or display a game over message
        }
    }
}
