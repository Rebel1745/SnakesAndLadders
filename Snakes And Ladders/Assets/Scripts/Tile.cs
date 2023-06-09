using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    public int TileId;
    public int TileNumber;
    public PlayerPiece PlayerPiece;
    public Tile SnakeDestinationTile;
    public Tile LadderDestinationTile;
    public Transform SnakeOrLadderHolder;
    public Transform TileNumberText;
}
