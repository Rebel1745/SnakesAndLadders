using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager instance;

    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform playerHolder;
    [SerializeField] Transform[] playerStartPositions;

    [SerializeField] GameObject playerSelectUI;
    [SerializeField] int maximumPlayers = 8;
    [SerializeField] TMP_Text numberOfPlayersText;
    int numberOfPlayers = 2;
    [SerializeField] Transform playerDetailsHolder;
    [SerializeField] GameObject[] allPlayerDetails;
    public Material[] PlayerColours;

    public GameObject[] Players;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        UpdateNumberOfPlayerDetails();
    }

    void UpdateNumberOfPlayerDetails()
    {
        bool activated = false;
        for (int i = 0; i < allPlayerDetails.Length; i++)
        {
            activated = i <= numberOfPlayers - 1 ? true : false;
            allPlayerDetails[i].SetActive(activated);
        }
    }

    public void ShowPlayerSelectScreen()
    {
        playerSelectUI.SetActive(true);
        ChangeNumberOfPlayersText();
    }

    public void HidePlayerSelectScreen()
    {
        playerSelectUI.SetActive(false);
    }

    public void ChangePlayerNumbers(int amount)
    {
        numberOfPlayers += amount;

        if (numberOfPlayers < 1)
            numberOfPlayers = maximumPlayers;
        if (numberOfPlayers > maximumPlayers)
            numberOfPlayers = 1;

        ChangeNumberOfPlayersText();
        UpdateNumberOfPlayerDetails();
    }

    void ChangeNumberOfPlayersText()
    {
        numberOfPlayersText.text = numberOfPlayers.ToString();
    }

    public void CreatePlayers()
    {
        PlayerDetails currentPlayerDetails;
        PlayerPiece currentPlayerPiece;

        Players = new GameObject[numberOfPlayers];

        for (int i = 0; i < numberOfPlayers; i++)
        {
            currentPlayerDetails = playerDetailsHolder.GetChild(i).GetComponent<PlayerDetails>();
            Players[i] = Instantiate(playerPrefab, playerStartPositions[i].position, Quaternion.identity, playerHolder);
            currentPlayerPiece = Players[i].GetComponent<PlayerPiece>();
            currentPlayerPiece.SetupPlayerPiece(i, currentPlayerDetails.PlayerName, currentPlayerDetails.isCPU, currentPlayerDetails.currentColourIndex, playerStartPositions[i].position);

            // deactivate until the game is ready to start
            Players[i].SetActive(false);
        }

        HidePlayerSelectScreen();

        GameManager.instance.UpdateGameState(GameState.BuildGameBoard);
    }
}