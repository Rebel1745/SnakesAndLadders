using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    [SerializeField] Tile startingTile;
    Tile currentTile;

    // movement variables
    Tile[] moveQueue;
    int moveQueueIndex;
    Vector3 targetPosition;
    Vector3 velocity = Vector3.zero;
    [SerializeField] float smoothTime = 0.25f;
    [SerializeField] float smoothDistance = 0.01f;
    [SerializeField] float maxHeight = 0.5f;
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
        if(Vector3.Distance(this.transform.position, targetPosition) < smoothDistance)
        {
            AdvanceMoveQueue();
        }

        targetHeight = heightCurve.Evaluate(heightTime / smoothTime) * maxHeight;
        // TODO: Why is this not working? it is giving me wrong numbers
        print(heightTime + " / " + smoothTime + " = " + heightTime / smoothTime);
        targetPositionWithHeight = new Vector3(targetPosition.x, targetHeight , targetPosition.z);

        this.transform.position = Vector3.SmoothDamp(this.transform.position, targetPositionWithHeight, ref velocity, smoothTime);

        heightTime += Time.deltaTime;
    }

    void AdvanceMoveQueue()
    {
        if (moveQueue != null && moveQueueIndex < moveQueue.Length)
        {
            SetNewTargetPosition(moveQueue[moveQueueIndex].transform.position);
            moveQueueIndex++;
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
        if (!DiceRoller.instance.IsDoneRolling)
            return;

        // move this piece
        int spacesToMove = DiceRoller.instance.DiceTotal;

        print("Clicked. Moving " + spacesToMove + " spaces");
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

        moveQueueIndex = 0;

        currentTile = finalTile;
    }
}
