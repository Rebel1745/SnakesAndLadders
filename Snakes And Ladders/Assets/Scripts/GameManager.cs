using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] TMP_Text infoText;
    [SerializeField] GameObject gameUI;

    public GameState State;

    // board
    [SerializeField] int boardWidth;
    [SerializeField] int boardHeight;

    // Players
    int totalPlayers;
    PlayerPiece currentPlayerPiece;
    public int CurrentPlayerId;
    string currentPlayerName;
    public bool IsCurrentPlayerCPU = false;

    // Dice
    public int DiceTotal;

    void Awake()
    {
        instance = this;
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
            case GameState.GameOverScreen:
                GameOver();
                break;

        }
    }

    void GameOver()
    {
        SetInfoText(currentPlayerName + " has won!");
    }

    void SetupGame()
    {
        // TODO: get the info from the players array to place the players and reactivate them
        foreach(GameObject g in PlayerManager.instance.Players)
        {
            g.SetActive(true);
        }
        // Setup the variables that control player names and ids
        totalPlayers = PlayerManager.instance.Players.Length;
        CurrentPlayerId = 0;
        UpdateCurrentPlayerDetails();
        // Activate the GameUI
        gameUI.SetActive(true);
        // Start the game by changing the state to WaitingForRoll
        UpdateGameState(GameState.WaitingForRoll);
    }

    void UpdateCurrentPlayerDetails()
    {
        currentPlayerPiece = PlayerManager.instance.Players[CurrentPlayerId].GetComponent<PlayerPiece>();
        currentPlayerName = currentPlayerPiece.PlayerName;
        IsCurrentPlayerCPU = currentPlayerPiece.IsCPU;
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
        SetInfoText(currentPlayerName + " click piece to move");
    }

    private void WaitingForRoll()
    {
        DiceManager.instance.SetDice(0);
        SetInfoText(currentPlayerName + " to roll");
    }

    void RollAgain()
    {
        UpdateGameState(GameState.WaitingForRoll);
    }

    void NewTurn()
    {
        // advance player
        CurrentPlayerId = (CurrentPlayerId + 1) % totalPlayers;
        UpdateCurrentPlayerDetails();

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
