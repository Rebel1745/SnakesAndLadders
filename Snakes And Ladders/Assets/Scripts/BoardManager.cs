using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    [SerializeField] Transform boardParent;
    [SerializeField] GameObject tilePrefab;

    private void Awake()
    {
        instance = this;
    }

    public void CreateBoard(int width, int height)
    {
        // destroy the placeholder tiles that are there
        for(int i = 0; i < boardParent.childCount; i++)
        {
            Destroy(boardParent.GetChild(i).gameObject);
        }

        GameObject newTile;
        Vector3 startPos = Vector3.zero, tilePos;
        int tileNo = 1;
        // create an array of tiles
        // first of two loops, one going in the positive direction, one negative
        for (int z = 0; z < height; z+=2)
        {
            for (int x = 0; x < width; x++)
            {
                tilePos = new Vector3((startPos.x + x), startPos.y, startPos.z + z);
                newTile = Instantiate(tilePrefab, tilePos, Quaternion.identity, boardParent);
                newTile.name = "Tile " + tileNo;

                tileNo++;
            }

            for (int x = width; x > 0; x--)
            {
                tilePos = new Vector3((startPos.x + x - 1), startPos.y, (startPos.z + z + 1));
                newTile = Instantiate(tilePrefab, tilePos, Quaternion.identity, boardParent);
                newTile.name = "Tile " + tileNo;

                tileNo++;
            }
        }
    }
}
