using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Piece : MonoBehaviour
{
    public TetrominoData data;
    public Tilemap tilemap;
    public Vector2Int[] blocks; //list of blocks of piece relative to center
    public Vector2Int position;
    public int rotationIndex; //rotation as int: 0 = normal, 1 = 90 degrees right, 2 = 180 degrees. 3 = 90 degrees left
    public Player player;


    public void SetPiece(TetrominoData tetrominoData, Vector2Int _position)
    {
        data = tetrominoData;
        blocks = data.blocks;
        position = _position;
    }
    public void FixedUpdate()
    {
        //fixed update, speed determined in main.cs
        ClearFallingPiece(this);

        SetFallingPiece(this);
    }

    public void ClearFallingPiece(Piece piece)
    {
        //set all tiles on tilemap containing the piece clear
        foreach (Vector2Int block in piece.blocks)
        {
            Vector3Int blockPosition = new Vector3Int(block.x + position.x, block.y + position.y, 0);
            tilemap.SetTile(blockPosition, null);
        }
    }

    public void SetFallingPiece(Piece piece)
    {
        //move piece down 1 tile
        position += Vector2Int.down;

        //set all tiles on the tilemap containing the piece to correct color tile
        foreach (Vector2Int block in piece.blocks)
        {
            Vector3Int blockPosition = new Vector3Int(block.x + position.x, block.y + position.y, 0);
            tilemap.SetTile(blockPosition, piece.data.tile);
        }
    }
}
