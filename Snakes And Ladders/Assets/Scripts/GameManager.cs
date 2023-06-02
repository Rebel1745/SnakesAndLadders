using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] TMP_Text infoText;

    // Players
    string[] PlayerNames = { "One", "Two", "Three", "Four" };
    public int TotalPlayers = 2;
    public int CurrentPlayerId;
    public string CurrentPlayerName;

    // Dice
    public int DiceTotal;

    // States 
    // TODO: Replace with enum
    public bool IsDoneRolling = false;
    public bool IsDoneClicking = false;
    public bool IsDoneAnimating = false;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        CurrentPlayerName = PlayerNames[CurrentPlayerId];
    }

    // Update is called once per frame
    void Update()
    {
        if (IsDoneRolling && IsDoneClicking && IsDoneAnimating)
            NewTurn();
    }

    void NewTurn()
    {
        IsDoneRolling   = false;
        IsDoneClicking  = false;
        IsDoneAnimating = false;

        // advance player
        CurrentPlayerId = (CurrentPlayerId + 1) % TotalPlayers;
        CurrentPlayerName = PlayerNames[CurrentPlayerId];

        DiceRoller.instance.SetDiceText("?");
        SetInfoText("Player " + CurrentPlayerName + " to roll");
    }

    public void SetInfoText(string newText)
    {
        infoText.text = newText;
    }
}
