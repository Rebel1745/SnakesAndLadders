using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceRoller : MonoBehaviour
{
    public static DiceRoller instance;

    [SerializeField] TMP_Text diceCountText;

    public bool IsDoneRolling = false;
    public int DiceTotal;

    void Start()
    {
        instance = this;
        SetDiceText("?");
    }

    void NewTurn()
    {
        IsDoneRolling = false;
    }

    public void RollTheDice()
    {
        // random number between 1 and 6 for the dice roll
        int rolledNum = Random.Range(1, 7);
        DiceTotal = rolledNum;
        SetDiceText(rolledNum.ToString());
        IsDoneRolling = true;
    }

    void SetDiceText(string text)
    {
        diceCountText.text = text;
    }
}
