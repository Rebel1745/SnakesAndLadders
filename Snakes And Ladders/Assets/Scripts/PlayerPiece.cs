using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPiece : MonoBehaviour
{
    [SerializeField] Tile startingTile;
    Tile currentTile;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnMouseUp()
    {
        print("Clicked");
        if (!DiceRoller.instance.IsDoneRolling)
            return;

        // move this piece
        int spacesToMove = DiceRoller.instance.DiceTotal;

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
        }

        this.transform.position = finalTile.transform.position;
        currentTile = finalTile;
    }
}
