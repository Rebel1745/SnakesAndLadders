using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    [SerializeField] Transform boardParent;
    [SerializeField] GameObject tilePrefab;

    public Tile[] boardTiles;

    private void Awake()
    {
        instance = this;
    }

    public Tile GetLastTile()
    {
        return boardTiles[boardTiles.Length - 1];
    }

    public Tile GetTile(int index)
    {
        return boardTiles[index];
    }

    public Tile[] GetTiles(int startIndex, int numberOfTiles)
    {
        Tile[] tilesArray = new Tile[numberOfTiles];
        int arrayIndex = 0;

        for (int i = startIndex; i < numberOfTiles; i++)
        {
            tilesArray[arrayIndex] = GetTile(i);
        }

        return tilesArray;
    }

    public void CreateBoard(int width, int height)
    {
        boardTiles = new Tile[width * height];

        // destroy the placeholder tiles that are there
        for(int i = 0; i < boardParent.childCount; i++)
        {
            Destroy(boardParent.GetChild(i).gameObject);
        }

        GameObject newTileGO;
        Tile newTile;
        Vector3 startPos = Vector3.zero, tilePos;
        int tileNo = 1;
        // create an array of tiles
        // first of two loops, one going in the positive direction, one negative
        for (int z = 0; z < height; z+=2)
        {
            for (int x = 0; x < width; x++)
            {
                tilePos = new Vector3((startPos.x + x), startPos.y, startPos.z + z);
                newTileGO = Instantiate(tilePrefab, tilePos, Quaternion.identity, boardParent);
                newTileGO.name = "Tile " + tileNo;
                newTile = newTileGO.GetComponent<Tile>();
                newTile.TileId = tileNo - 1;
                newTile.TileNumber = tileNo;

                boardTiles[tileNo - 1] = newTile;

                tileNo++;
            }

            for (int x = width; x > 0; x--)
            {
                tilePos = new Vector3((startPos.x + x - 1), startPos.y, (startPos.z + z + 1));
                newTileGO = Instantiate(tilePrefab, tilePos, Quaternion.identity, boardParent);
                newTileGO.name = "Tile " + tileNo;
                newTile = newTileGO.GetComponent<Tile>();
                newTile.TileId = tileNo - 1;
                newTile.TileNumber = tileNo;

                boardTiles[tileNo - 1] = newTile;

                tileNo++;
            }
        }

        // finished building, move on to next step
        GameManager.instance.UpdateGameState(GameState.WaitingForRoll);
    }
}
