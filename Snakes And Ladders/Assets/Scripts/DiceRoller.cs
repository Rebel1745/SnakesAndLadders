using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DiceRoller : MonoBehaviour
{
    public static DiceRoller instance;

    [SerializeField] TMP_Text diceCountText;

    void Start()
    {
        instance = this;
        SetDiceText("?");
    }

    public void RollTheDice()
    {
        // Have we already rolled the dice?
        if (GameManager.instance.State != GameState.WaitingForRoll)
            return;

        // random number between 1 and 6 for the dice roll
        int rolledNum = Random.Range(1, 7);
        GameManager.instance.DiceTotal = rolledNum;
        SetDiceText(rolledNum.ToString());

        // move on to the piece clicking
        GameManager.instance.UpdateGameState(GameState.WaitingForClick);
    }

    public void SetDiceText(string text)
    {
        diceCountText.text = text;
    }
}
