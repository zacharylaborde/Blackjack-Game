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
    private Card dealerCard1;
    private GameObject bettingChips;
    private int initialBankroll = 1000;
    public GameObject pauseMenu;
    public GameObject naturalBlackjackParticlesPrefab;
    public float particleEffectDuration = 2f;

    // Constants
    private const int MinimumBet = 1;
    private const int MaximumBet = 500;

    // Game Buttons
    public Button dealButton;
    public Button hitButton;
    public Button standButton;
    public Button resetBetButton;
    public Button newGameButton;
    public Button newRoundButton;
    public Button quitButton;
    public Button pauseMenuQuitButton;
    public Button pauseMenuSettingsButton;

    // Game Text
    public TextMeshProUGUI bankrollText;
    public TextMeshProUGUI betText;
    public TextMeshProUGUI playerHandText;
    public TextMeshProUGUI dealerHandText;

    public Image messageImage;

    public List<ButtonInteractable> buttonInteractables;    // List of ButtonInteractable scripts
    public List<TextMeshProUGUI> chipTexts;                 // List of Chip Texts
    private List<Button> gameButtons;

    // Sound and Music
    public AudioClip blackjackSound;
    public AudioClip playerWinsSound;
    public AudioClip dealerWinsSound;
    public AudioClip pushSound;

    public AudioSource soundEffectsSource;
    public AudioSource backgroundMusicSource;

    // Confirmation Dialog
    public GameObject quitConfirmationDialog;
    public Button confirmQuitButton;
    public Button cancelQuitButton;

    private bool isQuitConfirmationDialogActive = false;
    private bool isGamePaused = false;

    private void Start()
    {
        StartGame();

        // Get references to all game buttons
        gameButtons = new List<Button>()
        {
            dealButton,
            hitButton,
            standButton,
            resetBetButton,
            newGameButton,
            newRoundButton,
            quitButton
        };
    }

    // Start the game
    void StartGame()
    {
        // Initialize Audio Sources
        soundEffectsSource = gameObject.AddComponent<AudioSource>();
        backgroundMusicSource = gameObject.AddComponent<AudioSource>();

        // Start playing the background music
        backgroundMusicSource.Play();

        // Initialize the confirmation dialog
        quitConfirmationDialog.SetActive(false);
        confirmQuitButton.onClick.AddListener(ConfirmQuit);
        cancelQuitButton.onClick.AddListener(CancelQuit);

        // Initialize the deck
        deck = GameObject.Find("Deck").GetComponent<Deck>();

        // Initialize the player
        player = GameObject.Find("Player").GetComponent<Player>();
        player.Bankroll = initialBankroll;

        // Initialize the dealer
        dealer = GameObject.Find("Dealer").GetComponent<Dealer>();

        // Initizlize betting chips
        bettingChips = GameObject.Find("Betting Chips");

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

        // Disable place message text along with the background
        messageImage.gameObject.SetActive(false);

        // Disable chip visability
        bettingChips.gameObject.SetActive(false);
    }

    // Method to enable all ButtonInteractable scripts
    void EnableAllChipButtonInteractables()
    {
        //resetBetButton.gameObject.SetActive(true);
        //resetBetButton.interactable = true;

        foreach (var buttonInteractable in buttonInteractables) {
            if (buttonInteractable != null) {
                buttonInteractable.isInteractable = true;
            }
        }
        foreach (var chipText in chipTexts) {
            if (chipText != null) {
                chipText.gameObject.SetActive(true);
            }
        }

        // Update chip button visibility
        UpdateChipButtonVisibility();

        // Enable place bets message to player along with the background
        messageImage.GetComponentInChildren<TextMeshProUGUI>().text = "Place Bets!";
        messageImage.gameObject.SetActive(true);

        // Enable chip visability
        bettingChips.gameObject.SetActive(true);
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
        dealerCard1 = deck.DrawCard();
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

        // Update player and dealer UI
        playerHandText.text = "Hand: " + player.CalculateHandValue().ToString();
        dealerHandText.text = "Dealer: " + dealer.CalculateHandValue(false).ToString();     // does not count the hole card value

        // Check if the player has natural 21
        if (player.CalculateHandValue() == 21) {
            EvaluateRoundResult();
            UpdateBankroll();
            PostRoundActions();
        }
    }

    // Allows the player to place a bet within the predefined limits
    public void PlaceBet(int amount)
    {
        if (player.Bankroll > 0) {

            double newBetAmount = player.CurrentBet + amount;
            double betDifference = amount - player.Bankroll;

            resetBetButton.gameObject.SetActive(true);
            resetBetButton.interactable = true;

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
                Debug.LogWarning("Invalid Bet Amount: The entered bet amount is not within the allowed range. Please enter a value between the minimum and maximum bet limits.");
            }
        }
        else {
            Debug.LogWarning("Insufficient Balance: Your current balance is not sufficient to place the selected bet.");
        }

        // Update chip button visibility
        UpdateChipButtonVisibility();

        // Check if the current bet meets the condition for enabling the Deal button
        if (player.CurrentBet >= MinimumBet) {
            dealButton.gameObject.SetActive(true);
            dealButton.interactable = true;
        }
    }

    // Set the visibility of chip buttons based on the player's bankroll
    private void UpdateChipButtonVisibility()
    {
        int[] chipDenominations = { 1, 5, 25, 100, 500 };
        int[] betLimits = { 1, 5, 25, 100, 500 };

        for (int i = 0; i < chipDenominations.Length; i++) {
            bool canAffordChip = player.Bankroll >= chipDenominations[i];
            bool withinBetLimit = player.CurrentBet + chipDenominations[i] <= MaximumBet;

            buttonInteractables[i].gameObject.SetActive(canAffordChip && withinBetLimit);
            chipTexts[i].gameObject.SetActive(canAffordChip && withinBetLimit);
        }
    }

    // Resets the current bet amount to 0 and adds the previous bet amount back to the player's bankroll.
    public void ResetBet()
    {
        double totalBetAmount = player.CurrentBet;

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

        resetBetButton.gameObject.SetActive(false);
        resetBetButton.interactable = false;

        // Update chip button visibility
        UpdateChipButtonVisibility();
    }

    // Deals an additional card to the player
    public void PlayerHit()
    {
        if (player.Hand.Count < 11) {
            Card card = deck.DrawCard();
            card.transform.SetParent(player.transform, true);
            player.AddCardToHand(card);

            // Update playerhand text UI
            playerHandText.text = "Hand: " + player.CalculateHandValue().ToString();

            // Check if the player busts
            if (player.CalculateHandValue() > 21) {
                EvaluateRoundResult();
                UpdateBankroll();
                PostRoundActions();
            }
        }
        else {
            Debug.LogWarning("Cannot exceed max card limit.");
        }
    }

    // Proceeds to the dealer's turn
    public void PlayerStand()
    {
        // Flip the hole card & update dealer's hand UI
        dealerCard1.transform.Rotate(0f, 0f, 180f); // Rotate the card by 180 degrees on the Z-axis
        dealerHandText.text = "Dealer: " + dealer.CalculateHandValue(true).ToString();

        DealerTurn();
        EvaluateRoundResult();
        UpdateBankroll();
        PostRoundActions();
    }

    // Controls the dealer's actions, including hitting or standing based on predefined rules
    void DealerTurn()
    {
        int dealerHandValue = dealer.CalculateHandValue(true);
        int playerHandValue = player.CalculateHandValue();

        // Check if the dealer's hand beats the player's hand
        if (dealerHandValue > playerHandValue) {
            // Update dealerhand text UI
            dealerHandText.text = "Dealer: " + dealer.CalculateHandValue(true).ToString();
            return;
        }

        while (dealerHandValue < 17) {
            Card card = deck.DrawCard();
            card.transform.SetParent(dealer.transform, true);
            dealer.AddCardToHand(card);

            // Update dealerhand text UI
            dealerHandText.text = "Dealer: " + dealer.CalculateHandValue(true).ToString();

            dealerHandValue = dealer.CalculateHandValue(true);
        }
    }

    // Compares the player's and dealer's hand values to determine the round result
    void EvaluateRoundResult()
    {
        // Disable Hit and Stand Buttons
        hitButton.gameObject.SetActive(false);
        hitButton.interactable = false;
        standButton.gameObject.SetActive(false);
        standButton.interactable = false;

        int playerHandValue = player.CalculateHandValue();
        int dealerHandValue = dealer.CalculateHandValue(true);

        if (playerHandValue == 21 && player.Hand.Count == 2) {
            // Player has a natural blackjack
            Debug.Log("Player has blackjack! Player wins.");
            messageImage.GetComponentInChildren<TextMeshProUGUI>().text = "Player Wins!";
            soundEffectsSource.PlayOneShot(blackjackSound, 1.0f);

            // Instantiate the particle effect at the position specified by the prefab
            GameObject particleEffect = Instantiate(naturalBlackjackParticlesPrefab, new Vector3(0, 1, -0.1f), Quaternion.identity);

            // Play the particle effect for the specified duration
            Destroy(particleEffect, particleEffectDuration);
        }
        else if (playerHandValue > 21) {
            // Player busts, dealer wins
            Debug.Log("Player busts! Dealer wins.");
            messageImage.GetComponentInChildren<TextMeshProUGUI>().text = "Dealer Wins!";
            soundEffectsSource.PlayOneShot(dealerWinsSound, 1.0f);
        }
        else if (dealerHandValue > 21) {
            // Dealer busts, player wins
            Debug.Log("Dealer busts! Player wins.");
            messageImage.GetComponentInChildren<TextMeshProUGUI>().text = "Player Wins!";
            soundEffectsSource.PlayOneShot(playerWinsSound, 1.0f);
        }
        else if (playerHandValue == dealerHandValue) {
            // It's a tie
            Debug.Log("It's a tie.");
            messageImage.GetComponentInChildren<TextMeshProUGUI>().text = "Push!";
            soundEffectsSource.PlayOneShot(pushSound, 1.0f);
        }
        else if (playerHandValue > dealerHandValue) {
            // Player wins
            Debug.Log("Player wins.");
            messageImage.GetComponentInChildren<TextMeshProUGUI>().text = "Player Wins!";
            soundEffectsSource.PlayOneShot(playerWinsSound, 1.0f);
        }
        else {
            // Dealer wins
            Debug.Log("Dealer wins.");
            messageImage.GetComponentInChildren<TextMeshProUGUI>().text = "Dealer Wins!";
            soundEffectsSource.PlayOneShot(dealerWinsSound, 1.0f);
        }

        // Enable round result message to player
        messageImage.gameObject.SetActive(true);
    }

    // Updates the player's bankroll based on the round result
    void UpdateBankroll()
    {
        int playerHandValue = player.CalculateHandValue();
        int dealerHandValue = dealer.CalculateHandValue(true);

        if (playerHandValue > 21) {
            // Player busts, lose the bet
        }
        else if (dealerHandValue > 21) {
            // Dealer busts, win the bet; Payout is 1:1
            player.Bankroll += player.CurrentBet * 2;
        }
        else if (playerHandValue == dealerHandValue) {
            // It's a tie, return the bet
            player.Bankroll += player.CurrentBet;
        }
        else if (playerHandValue == 21 && player.Hand.Count == 2) {
            // Player has a natural blackjack, Payout is 3:2
            player.Bankroll += (int)System.Math.Floor(player.CurrentBet * 2.5);
        }
        else if (playerHandValue > dealerHandValue) {
            // Player wins, win the bet; Payout is 1:1
            player.Bankroll += player.CurrentBet * 2;
        }
        else {
            // Dealer wins, lose the bet
        }

        // Reset player's current bet
        player.CurrentBet = 0;

        // Update UI for Bankroll and Bet
        bankrollText.text = "Balance: $" + player.Bankroll.ToString();
        betText.text = "Bet: $" + player.CurrentBet.ToString();

    }

    // Prompts the player to start a new round or quit the game
    public void StartNewRound()
    {
        // Disable message text
        messageImage.gameObject.SetActive(false);
        // Disable the text for player and dealer hands
        playerHandText.gameObject.SetActive(false);
        dealerHandText.gameObject.SetActive(false);
        // Disable all buttons
        newGameButton.gameObject.SetActive(false);
        newGameButton.interactable = false;
        newRoundButton.gameObject.SetActive(false);
        newRoundButton.interactable = false;
        quitButton.gameObject.SetActive(false);
        quitButton.interactable = false;
        dealButton.gameObject.SetActive(false);
        dealButton.interactable = false;
        hitButton.gameObject.SetActive(false);
        hitButton.interactable = false;
        standButton.gameObject.SetActive(false);
        standButton.interactable = false;
        resetBetButton.gameObject.SetActive(false);
        resetBetButton.interactable = false;

        if (player.Bankroll >= MinimumBet) {
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

    // Method to confirm quitting the game
    void ConfirmQuit()
    {
        // Quit the application
        Application.Quit();
    }

    // Method to cancel quitting the game
    void CancelQuit()
    {
        isQuitConfirmationDialogActive = false;
        pauseMenuQuitButton.interactable = true;
        pauseMenuSettingsButton.interactable = true;

        // Hide the confirmation dialog
        quitConfirmationDialog.SetActive(false);

        // Enable New Game and New Round buttons
        newGameButton.interactable = true;
        newRoundButton.interactable = true;
        quitButton.interactable = true;
    }

    // Method to handle the quit button click
    public void OpenQuitConfirmationDialog()
    {
        isQuitConfirmationDialogActive = true;
        pauseMenuQuitButton.interactable = false;
        pauseMenuSettingsButton.interactable = false;

        // Show the quit confirmation dialog
        quitConfirmationDialog.SetActive(true);

        // Disable New Game and New Round buttons while dialog box is up
        newGameButton.interactable = false;
        newRoundButton.interactable = false;
        quitButton.interactable = false;
    }

    // Method to restart the game
    public void RestartGame()
    {
        // Reset necessary game state variables here

        // Reload the current scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // Method to toggle the pause menu
    private void TogglePauseMenu()
    {
        // Toggle the pause menu active state
        pauseMenu.SetActive(!pauseMenu.activeSelf);

        // Enable or disable all game buttons based on the pause menu state
        foreach (Button button in gameButtons) {
            button.interactable = !pauseMenu.activeSelf;
        }

        foreach (var buttonInteractable in buttonInteractables) {
            if (buttonInteractable != null) {
                buttonInteractable.isInteractable = !pauseMenu.activeSelf;
            }
        }

        // Toggle the game paused flag
        isGamePaused = pauseMenu.activeSelf;

        // Pause or resume the game
        //Time.timeScale = pauseMenu.activeSelf ? 0f : 1f;
    }

    // Method for post-round actions
    public void PostRoundActions()
    {
        if (player.Bankroll > 0) {
            // Player has a positive balance
            // Enable New Round and Quit buttons
            newRoundButton.gameObject.SetActive(true);
            newRoundButton.interactable = true;
        }
        else {
            // Player has a balance of 0
            newGameButton.gameObject.SetActive(true);
            newGameButton.interactable = true;
        }
        // Enable Quit button
        quitButton.gameObject.SetActive(true);
        quitButton.interactable = true;
    }

    // Method to display a message for a specific duration
    private IEnumerator DisplayMessage(string message, float duration)
    {
        // Set the message text
        messageImage.GetComponentInChildren<TextMeshProUGUI>().text = message;

        // Show the message text element
        messageImage.gameObject.SetActive(true);

        // Wait for the specified duration
        yield return new WaitForSeconds(duration);

        // Hide the message text element
        messageImage.gameObject.SetActive(false);
    }


    // Update is called once per frame
    private void Update()
    {
        // Check if the quit confirmation dialog is active
        if (isQuitConfirmationDialogActive) {
            // Don't allow opening the pause menu
            return;
        }

        // Open pause menu when the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape)) {
            // Display the pause menu
            TogglePauseMenu();
        }
    }
}