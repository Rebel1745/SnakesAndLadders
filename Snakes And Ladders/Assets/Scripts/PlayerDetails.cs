using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerDetails : MonoBehaviour
{
    [SerializeField] TMP_InputField playerNameInputField;
    [SerializeField] string defaultPlayerName;
    public string PlayerName;
    public bool isCPU = false;
    public int currentColourIndex = 0;
    
    [SerializeField] Image currentColourImage;

    private void Start()
    {
        SetCurrentColourImage();
        SetPlayerName(defaultPlayerName);
    }

    public void ChangeCurrentColour(int amount)
    {
        currentColourIndex += amount;

        if (currentColourIndex < 1)
            currentColourIndex = PlayerManager.instance.PlayerColours.Length - 1;
        if (currentColourIndex > PlayerManager.instance.PlayerColours.Length - 1)
            currentColourIndex = 0;

        SetCurrentColourImage();
    }

    void SetCurrentColourImage()
    {
        currentColourImage.color = PlayerManager.instance.PlayerColours[currentColourIndex].color;
    }

    public void SetPlayerName(string newPlayerName)
    {
        PlayerName = newPlayerName;
    }

    public void EndEditPlayerName(string newPlayerName)
    {
        if (newPlayerName == "")
        {
            SetPlayerName(defaultPlayerName);
            playerNameInputField.text = defaultPlayerName;
        }
    }

    public void SetPlayerType(int type)
    {
        if (type == 0)
            isCPU = false;
        else
            isCPU = true;
    }
}
