using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    [SerializeField] Tile startingTile;
    [SerializeField] int playerId;
    Tile currentTile;

    bool isAnimating = false;

    // movement variables
    Tile[] moveQueue;
    int moveQueueIndex;
    Vector3 targetPosition;
    Vector3 velocity = Vector3.zero;
    readonly float smoothTime = 0.25f;
    [SerializeField] int smoothTimeMultiplier = 1;
    readonly float smoothDistance = 0.01f;
    readonly float maxHeight = 0.5f;
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

        targetPositionWithHeight = new Vector3(targetPosition.x, targetPosition.y + targetHeight , targetPosition.z);

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
            //finished all animations, allow another roll if a 6 was rolled, otherwise move on to next turn
            isAnimating = false;
            if (moveQueue.Length == 6)
                GameManager.instance.UpdateGameState(GameState.RollAgain);
            else
                GameManager.instance.UpdateGameState(GameState.NewTurn);
            
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

        // we have clicked on a valid piece, set it moving
        GameManager.instance.UpdateGameState(GameState.WaitingForAnimation);
    }
}
