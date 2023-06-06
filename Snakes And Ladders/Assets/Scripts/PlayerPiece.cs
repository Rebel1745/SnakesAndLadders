using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    [SerializeField] Tile startingTile;
    [SerializeField] int playerId;
    Tile currentTile;

    bool isAnimating = false;
    bool isAnimatingSnakeOrLadder = false;

    // movement variables
    Tile[] moveQueue;
    int moveQueueIndex;
    Vector3 targetPosition;
    Vector3 velocity = Vector3.zero;
    float smoothTime = 0.25f;
    [SerializeField] int smoothTimeMultiplier = 1;
    float smoothDistance = 0.01f;
    float maxHeight = 0.5f;
    [SerializeField] int maxHeightMultiplier = 1;
    [SerializeField] AnimationCurve heightCurve;
    Vector3 targetPositionWithHeight;
    float heightTime;
    float targetHeight;
    
    void Start()
    {
        targetPosition = this.transform.position;
    }
    
    void Update()
    {
        MovePiece();

        if (GameManager.instance.State == GameState.CheckForSnakesAndLadders)
            CheckForSnakesAndLadders();
    }

    // TODO: Make this public and call it from the GameManager when player pieces are programatically generated
    // TODO: Add different speed variables for going up ladders and down snakes (currently just uses smoothTime)
    void CheckForSnakesAndLadders()
    {
        // If there are no snakes or ladders on this tile, move on
        if(currentTile.SnakeDestinationTile == null && currentTile.LadderDestinationTile == null)
        {
            MoveToNextTurn();
        }
        else
        {
            isAnimating = true;
            isAnimatingSnakeOrLadder = true;
            targetPosition = currentTile.SnakeDestinationTile == null ? currentTile.LadderDestinationTile.transform.position : currentTile.SnakeDestinationTile.transform.position;
            moveQueue = new Tile[1];
            moveQueueIndex = 1;
            currentTile = currentTile.SnakeDestinationTile ?? currentTile.LadderDestinationTile;
            SetNewTargetPosition(targetPosition);
            GameManager.instance.UpdateGameState(GameState.WaitingForAnimation);
        }
    }

    void MoveToNextTurn()
    {
        //allow another roll if a 6 was rolled, otherwise move on to next turn
        if (moveQueue.Length == 6)
            GameManager.instance.UpdateGameState(GameState.RollAgain);
        else
            GameManager.instance.UpdateGameState(GameState.NewTurn);
    }

    void MovePiece()
    {
        // have we finished animating?
        if (!isAnimating)
            return;

        GameManager.instance.SetInfoText("Player " + GameManager.instance.CurrentPlayerName + " moving " + (moveQueue.Length - moveQueueIndex) + " more squares");

        if(Vector3.Distance(this.transform.position, targetPosition) < smoothDistance)
        {
            AdvanceMoveQueue();
        }

        targetHeight = heightCurve.Evaluate(heightTime / smoothTime) * maxHeight * maxHeightMultiplier;

        // only add height to the movement if we aren't moving on snakes and ladders
        if (!isAnimatingSnakeOrLadder)
            targetPositionWithHeight = new Vector3(targetPosition.x, targetPosition.y + targetHeight, targetPosition.z);
        else
            targetPositionWithHeight = targetPosition;

        this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPositionWithHeight, ref velocity, (smoothTime / (float)smoothTimeMultiplier));
        
        heightTime += Time.deltaTime;
    }

    void AdvanceMoveQueue()
    {
        if (moveQueue != null && moveQueueIndex < moveQueue.Length)
        {
            SetNewTargetPosition(moveQueue[moveQueueIndex].transform.position);
            moveQueueIndex++;
        }
        else
        {
            //finished all animations, check for snakes and ladders
            isAnimating = false;
            GameManager.instance.UpdateGameState(GameState.CheckForSnakesAndLadders);            
        }
    }

    void SetNewTargetPosition(Vector3 pos)
    {
        targetPosition = pos;
        velocity = Vector3.zero;
        heightTime = 0f;
    }

    private void OnMouseUp()
    {
        // are we waiting for a click?
        if (GameManager.instance.State != GameState.WaitingForClick)
            return;

        // Check if this piece belongs to us, if not then we can't click on it
        if (playerId != GameManager.instance.CurrentPlayerId)
            return;

        // move this piece
        int spacesToMove = GameManager.instance.DiceTotal;
        GameManager.instance.SetInfoText("Player " + GameManager.instance.CurrentPlayerName + " moving " + spacesToMove + " more squares");
        
        moveQueue = new Tile[spacesToMove];

        Tile finalTile = currentTile;

        for (int i = 0; i < spacesToMove; i++)
        {
            if(finalTile == null)
            {
                finalTile = startingTile;
            }
            else
            {
                finalTile = finalTile.NextTile;
            }

            moveQueue[i] = finalTile;
        }

        // TODO: check to see if the destination tile is legal

        moveQueueIndex = 0;
        currentTile = finalTile;
        isAnimating = true;
        isAnimatingSnakeOrLadder = false;

        // we have clicked on a valid piece, set it moving
        GameManager.instance.UpdateGameState(GameState.WaitingForAnimation);
    }
}
