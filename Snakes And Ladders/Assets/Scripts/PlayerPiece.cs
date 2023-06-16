using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    Tile currentTile;

    bool isAnimating = false;
    bool isAnimatingSnakeOrLadder = false;
    bool isCurrentPlayerAnimation = false;

    // Player info
    public int PlayerId;
    public string PlayerName;
    public bool IsCPU;
    Vector3 startingPosition;

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
        CheckForCPUClick();

        MovePiece();

        if (GameManager.instance.State == GameState.CheckForVictory)
            CheckForVictory();

        if (GameManager.instance.State == GameState.CheckForSnakesAndLadders)
            CheckForSnakesAndLadders();
    }

    public void SetupPlayerPiece(int id, string name, bool cpu, int colourIndex, Vector3 startPos)
    {
        PlayerId = id;
        this.transform.name = "Player-" + name;
        PlayerName = name;
        IsCPU = cpu;
        this.GetComponentInChildren<Renderer>().material = PlayerManager.instance.PlayerColours[colourIndex];
        startingPosition = startPos;
    }

    void CheckForVictory()
    {
        // if the player has landed on the last square, they are victorious
        if(currentTile.TileNumber == BoardManager.instance.GetLastTile().TileNumber)
        {
            // the game is over!
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
            // check if there is a piece on this tile
            if (currentTile.TileId != 0 && currentTile.PlayerPiece)
            {
                currentTile.PlayerPiece.AnimateToSpecialTile(BoardManager.instance.GetTile(0), false);
            }

            // Set this player piece as being on this tile
            currentTile.PlayerPiece = this;

            MoveToNextTurn();
        }
        else
        {
            Tile targetTile = currentTile.SnakeDestinationTile ?? currentTile.LadderDestinationTile;
            AnimateToSpecialTile(targetTile, true);
        }
    }

    // this is used to setup animations for snakes and ladders, and being sent either back home or to the start square
    public void AnimateToSpecialTile(Tile newTile, bool currentPlayer)
    {
        isAnimating = true;
        isAnimatingSnakeOrLadder = true;
        isCurrentPlayerAnimation = currentPlayer;
        targetPosition = newTile.transform.position;
        moveQueue = new Tile[moveQueue.Length];
        moveQueueIndex = moveQueue.Length;
        currentTile = newTile;
        SetNewTargetPosition(targetPosition);
        GameManager.instance.UpdateGameState(GameState.WaitingForAnimation);
    }

    void MoveToNextTurn()
    {
        //allow another roll if a 6 was rolled, otherwise move on to next turn
        if (moveQueue.Length == 6)
        {
            GameManager.instance.CurrentPlayerRollAgainCount++;

            if(GameManager.instance.CurrentPlayerRollAgainCount < GameManager.instance.MaximumRollAgain)
            {
                GameManager.instance.UpdateGameState(GameState.RollAgain);
            }
            else
            {
                GameManager.instance.UpdateGameState(GameState.NewTurn);
            }
        }            
        else
            GameManager.instance.UpdateGameState(GameState.NewTurn);
    }
    
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
            //currentTile.PlayerPiece = this;
            if(isCurrentPlayerAnimation)
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

    void CheckForCPUClick()
    {
        if (GameManager.instance.State == GameState.WaitingForClick && PlayerId == GameManager.instance.CurrentPlayerId && GameManager.instance.IsCurrentPlayerCPU)
        {
            SelectPiece();
        }

        // also auto move the piece if the maximum number of 6's has been reached
        if(GameManager.instance.State == GameState.WaitingForClick 
            && PlayerId == GameManager.instance.CurrentPlayerId 
            && GameManager.instance.DiceTotal == 6 
            && GameManager.instance.CurrentPlayerRollAgainCount == GameManager.instance.MaximumRollAgain - 1)
        {
            SelectPiece();
        }
    }

    private void OnMouseUp()
    {
        // are we waiting for a click?
        if (GameManager.instance.State != GameState.WaitingForClick)
            return;

        // Check if this piece belongs to us, if not then we can't click on it
        if (PlayerId != GameManager.instance.CurrentPlayerId)
            return;

        SelectPiece();
    }

    void SelectPiece()
    {
        // move this piece
        int spacesToMove = GameManager.instance.DiceTotal;

        isCurrentPlayerAnimation = true;

        moveQueue = new Tile[spacesToMove];

        // if we are on a tile then remove this piece from it
        if(currentTile != null)
            currentTile.PlayerPiece = null;

        if (spacesToMove == 6 && GameManager.instance.CurrentPlayerRollAgainCount == GameManager.instance.MaximumRollAgain - 1)
        {
            // we have now rolled the maximum number of consecutive 6's, send the player to the start
            AnimateToSpecialTile(BoardManager.instance.GetTile(0), true);
            GameManager.instance.SetInfoText("You have rolled too many 6's!  Back to the start!");
            return;
        }

        Tile lastTile = BoardManager.instance.GetLastTile();

        // check to see if the destination tile is legal
        if (currentTile != null && currentTile.TileNumber + spacesToMove > lastTile.TileNumber)
        {
            // we would overshoot the board if we ran the normal moveQueue code so calculate up to the final square
            // first get the tiles up to and including the final tile
            int tilesUntilFinalTile = lastTile.TileNumber - currentTile.TileNumber;

            for (int i = 0; i < tilesUntilFinalTile; i++)
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
