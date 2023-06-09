using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tile : MonoBehaviour
{
    public int TileId;
    public int TileNumber;
    public PlayerPiece PlayerPiece;
    public Tile SnakeDestinationTile;
    public Tile LadderDestinationTile;
    public Transform SnakeOrLadderHolder;
    public TMP_Text tileNumberText;
}
