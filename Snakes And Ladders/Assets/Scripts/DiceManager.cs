using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DiceManager : MonoBehaviour
{
    public static DiceManager instance;

    [SerializeField] TMP_Text diceCountText;
    [SerializeField] Sprite[] diceImages;
    [SerializeField] Image diceImage;

    bool isRolling = false;
    public bool IsUsingDiceImage = false;
    public float TimeForDiceToRoll = 1f;

    void Start()
    {
        instance = this;
        SetDiceText("?");
    }

    private void Update()
    {
        if (GameManager.instance.State != GameState.WaitingForRoll)
            return;

        if (GameManager.instance.IsCurrentPlayerCPU)
            RollTheDice();

        if (isRolling)
            UpdateDice();
    }

    public void RollTheDice()
    {
        // Have we already rolled the dice?
        if (GameManager.instance.State != GameState.WaitingForRoll || isRolling)
            return;

        StartRolling();
    }

    void UpdateDice()
    {
        int rolledNum = Random.Range(1, 7);
        SetDice(rolledNum);
    }

    void StartRolling()
    {
        isRolling = true;
        Invoke("StopRolling", TimeForDiceToRoll);
    }

    void StopRolling()
    {
        // random number between 1 and 6 for the dice roll
        int rolledNum = Random.Range(1, 7);
        GameManager.instance.DiceTotal = rolledNum;
        SetDice(rolledNum);

        isRolling = false;

        // move on to the piece clicking
        GameManager.instance.UpdateGameState(GameState.WaitingForClick);
    }

    public void SetDice(int number)
    {
        if (IsUsingDiceImage)
            SetDiceImage(number);
        else
            SetDiceText(number.ToString());
    }

    void SetDiceText(string text)
    {
        diceCountText.text = text == "0" ? "?" : text;
    }

    void SetDiceImage(int number)
    {
        if (number != 0)
            diceCountText.text = "";
        else
            SetDiceText("0");
        diceImage.sprite = diceImages[number];
    }
}
