using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
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

        if (GameManager.instance.State == GameState.CheckForVictory)
            CheckForVictory();

        if (GameManager.instance.State == GameState.CheckForSnakesAndLadders)
            CheckForSnakesAndLadders();
    }

    void CheckForVictory()
    {
        // if the player has landed on the last square, they are victorious
        if(currentTile.TileNumber == BoardManager.instance.GetLastTile().TileNumber)
        {
            // the game is over!
            GameManager.instance.SetInfoText("Player " + GameManager.instance.CurrentPlayerName + " has won!");
            GameManager.instance.UpdateGameState(GameState.GameOverScreen);
        }
        else
        {
            // if not, check if the player is on a snake or a ladder
            GameManager.instance.UpdateGameState(GameState.CheckForSnakesAndLadders);
        }
    }

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

    // TODO: Make this public and call it from the GameManager when player pieces are programatically generated
    void MovePiece()
    {
        // have we finished animating?
        if (!isAnimating)
            return;

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
            //finished all animations, check for victory state
            isAnimating = false;
            GameManager.instance.UpdateGameState(GameState.CheckForVictory);            
        }
    }

    void SetNewTargetPosition(Vector3 pos)
    {
        // update the info text to count to the dice total
        if (moveQueueIndex == 0)
            GameManager.instance.SetInfoText("1");
        else
        {
            GameManager.instance.SetInfoText(GameManager.instance.GetInfoText() + " ... " + (moveQueueIndex + 1).ToString());
        }

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
        
        moveQueue = new Tile[spacesToMove];

        //Tile finalTile = currentTile;
        Tile lastTile = BoardManager.instance.GetLastTile();

        // check to see if the destination tile is legal
        if (currentTile != null && currentTile.TileNumber + spacesToMove > lastTile.TileNumber)
        {
            // we would overshoot the board if we ran the normal moveQueue code so calculate up to the final square
            // first get the tiles up to and including the final tile
            int tilesUntilFinalTile = lastTile.TileNumber - currentTile.TileNumber;

            for(int i = 0; i < tilesUntilFinalTile; i++)
            {
                moveQueue[i] = BoardManager.instance.GetTile(currentTile.TileId + i + 1);
            }

            // then in reverse order, get the remaining tiles starting at one before the last tile
            int remainingTiles = spacesToMove - tilesUntilFinalTile;

            for (int i = 0; i < remainingTiles; i++)
            {
                moveQueue[tilesUntilFinalTile + i] = BoardManager.instance.GetTile(lastTile.TileId - i - 1);
            }

            currentTile = moveQueue[spacesToMove - 1];
        }
        else
        {
            for (int i = 0; i < spacesToMove; i++)
            {
                // if there isn't a currentTile then we are not yet on the board, so start on the first square
                if (currentTile == null)
                {
                    moveQueue[i] = BoardManager.instance.GetTile(0);
                }
                else
                {
                    moveQueue[i] = BoardManager.instance.GetTile(currentTile.TileId + 1);
                }

                currentTile = moveQueue[i];
            }
        }
        

        moveQueueIndex = 0;
        isAnimating = true;
        isAnimatingSnakeOrLadder = false;

        // we have clicked on a valid piece, set it moving
        GameManager.instance.UpdateGameState(GameState.WaitingForAnimation);
    }
}
