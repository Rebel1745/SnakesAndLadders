using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BoardManager : MonoBehaviour
{
    public static BoardManager instance;
    [SerializeField] Transform boardParent;
    [SerializeField] GameObject tilePrefab;
    [SerializeField] GameObject ladderPrefab;
    [SerializeField] GameObject snakePrefab;

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
                newTile.tileNumberText.text = tileNo.ToString();

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
                newTile.tileNumberText.text = tileNo.ToString();

                boardTiles[tileNo - 1] = newTile;

                tileNo++;
            }
        }

        DrawSnakesAndLadders();

        // finished building, move on to next step
        GameManager.instance.UpdateGameState(GameState.WaitingForRoll);
    }

    void DrawSnakesAndLadders()
    {
        // snakes
        CreateSnakeOrLadder(GetTile(35), GetTile(1), true);
        CreateSnakeOrLadder(GetTile(23), GetTile(5), true);
        CreateSnakeOrLadder(GetTile(49), GetTile(6), true);
        CreateSnakeOrLadder(GetTile(26), GetTile(9), true);
        CreateSnakeOrLadder(GetTile(59), GetTile(21), true);
        CreateSnakeOrLadder(GetTile(94), GetTile(21), true);
        CreateSnakeOrLadder(GetTile(87), GetTile(25), true);
        CreateSnakeOrLadder(GetTile(53), GetTile(28), true);
        CreateSnakeOrLadder(GetTile(68), GetTile(34), true);
        CreateSnakeOrLadder(GetTile(82), GetTile(44), true);
        CreateSnakeOrLadder(GetTile(89), GetTile(47), true);
        CreateSnakeOrLadder(GetTile(98), GetTile(60), true);
        CreateSnakeOrLadder(GetTile(91), GetTile(85), true);

        // ladders
        CreateSnakeOrLadder(GetTile(2), GetTile(19), false);
        CreateSnakeOrLadder(GetTile(11), GetTile(30), false);
        CreateSnakeOrLadder(GetTile(14), GetTile(33), false);
        CreateSnakeOrLadder(GetTile(17), GetTile(74), false);
        CreateSnakeOrLadder(GetTile(20), GetTile(41), false);
        CreateSnakeOrLadder(GetTile(46), GetTile(65), false);
        CreateSnakeOrLadder(GetTile(48), GetTile(69), false);
        CreateSnakeOrLadder(GetTile(52), GetTile(73), false);
        CreateSnakeOrLadder(GetTile(56), GetTile(77), false);
        CreateSnakeOrLadder(GetTile(58), GetTile(79), false);
        CreateSnakeOrLadder(GetTile(63), GetTile(75), false);
        CreateSnakeOrLadder(GetTile(71), GetTile(90), false);
        CreateSnakeOrLadder(GetTile(81), GetTile(96), false);
    }

    void CreateSnakeOrLadder(Tile startTile, Tile endTile, bool isSnake)
    {
        if(isSnake)
            startTile.SnakeDestinationTile = endTile;
        else
            startTile.LadderDestinationTile = endTile;

        int segments = Mathf.CeilToInt(Vector3.Distance(startTile.transform.position, endTile.transform.position)) * 2;
        GameObject segmentPrefab = isSnake ? snakePrefab : ladderPrefab;
        GameObject segment;
        Vector3 segmentPos;

        for (int i = 0; i < segments; i++)
        {
            segmentPos = new Vector3(startTile.transform.position.x, startTile.transform.position.y, startTile.transform.position.z + ( i * 0.5f));
            segment = Instantiate(segmentPrefab, segmentPos, Quaternion.identity, startTile.SnakeOrLadderHolder);
        }

        startTile.SnakeOrLadderHolder.LookAt(endTile.transform.position);
    }
}
