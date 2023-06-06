using UnityEngine;
using TMPro;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] TMP_Text infoText;

    public GameState State;

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
        CurrentPlayerName = PlayerNames[CurrentPlayerId];
    }

    void Start()
    {
        UpdateGameState(GameState.WaitingForRoll);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
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

    private void WaitingForClick()
    {
        SetInfoText("Player " + CurrentPlayerName + " click piece to move");
    }

    private void WaitingForRoll()
    {
        DiceRoller.instance.SetDiceText("?");
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
}

public enum GameState
{
    SelectNumberOfPlayers,
    SelectPlayerDetails,
    BuildGameBoard,
    CreatePlayers,
    WaitingForRoll,
    WaitingForClick,
    WaitingForAnimation,
    CheckForSnakesAndLadders,
    NewTurn,
    RollAgain,
    GameOverScreen
}
