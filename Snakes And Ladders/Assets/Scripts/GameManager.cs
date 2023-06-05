using UnityEngine;
using TMPro;

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
                WaitingForAnimation();
                break;
            case GameState.NewTurn:
                NewTurn();
                break;
            case GameState.RollAgain:
                RollAgain();
                break;

        }
    }

    private void WaitingForAnimation()
    {
        
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
    WaitingForRoll,
    WaitingForClick,
    WaitingForAnimation,
    NewTurn,
    RollAgain
}
