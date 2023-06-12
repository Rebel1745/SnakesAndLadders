using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] TMP_Text infoText;

    public GameState State;

    // board
    [SerializeField] int boardWidth;
    [SerializeField] int boardHeight;

    // Players
    readonly string[] PlayerNames = { "One", "Two", "Three", "Four" };
    public int TotalPlayers = 2;
    public int CurrentPlayerId;
    public string CurrentPlayerName;

    // Dice
    public int DiceTotal;

    void Awake()
    {
        instance = this;
        //CurrentPlayerName = PlayerNames[CurrentPlayerId];
    }

    void Start()
    {
        UpdateGameState(GameState.SelectPlayerDetails);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.SelectPlayerDetails:
                SelectPlayerDetails();
                break;
            case GameState.BuildGameBoard:
                CreateGameBoard();
                break;
            case GameState.SetupGame:
                SetupGame();
                break;
            case GameState.WaitingForRoll:
                WaitingForRoll();
                break;
            case GameState.WaitingForClick:
                WaitingForClick();
                break;
            case GameState.WaitingForAnimation:
                // nothing to do as yet
                break;
            case GameState.CheckForSnakesAndLadders:
                break;
            case GameState.NewTurn:
                NewTurn();
                break;
            case GameState.RollAgain:
                RollAgain();
                break;

        }
    }

    void SetupGame()
    {
        // TODO: get the info from the players array to place the players and reactivate them
        // Setup the variables that control player names and ids
        // Start the game by changing the state to WaitingForRoll
        // Activate the GameUI
    }

    void SelectPlayerDetails()
    {
        PlayerManager.instance.ShowPlayerSelectScreen();
    }

    void CreateGameBoard()
    {
        BoardManager.instance.CreateBoard(boardWidth, boardHeight);
    }

    private void WaitingForClick()
    {
        SetInfoText("Player " + CurrentPlayerName + " click piece to move");
    }

    private void WaitingForRoll()
    {
        DiceManager.instance.SetDiceText("?");
        SetInfoText("Player " + CurrentPlayerName + " to roll");
    }

    void RollAgain()
    {
        UpdateGameState(GameState.WaitingForRoll);
    }

    void NewTurn()
    {
        // advance player
        CurrentPlayerId = (CurrentPlayerId + 1) % TotalPlayers;
        CurrentPlayerName = PlayerNames[CurrentPlayerId];

        UpdateGameState(GameState.WaitingForRoll);
    }

    public void SetInfoText(string newText)
    {
        infoText.text = newText;
    }

    public string GetInfoText()
    {
        return infoText.text;
    }
}

public enum GameState
{
    SelectPlayerDetails,
    BuildGameBoard,
    SetupGame,
    WaitingForRoll,
    WaitingForClick,
    WaitingForAnimation,
    CheckForVictory,
    CheckForSnakesAndLadders,
    NewTurn,
    RollAgain,
    GameOverScreen
}
