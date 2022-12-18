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
    public float maxLockTime;
    public float autoFallTime;
    public float lastMoveTimeClass;
    float currTime;
    RectInt bounds;


    public void Update()
    {
        if (data == null) //if the piece has not been initialized yet, do not execute update func
        {
            return;
        }
        currTime = Time.time;

        
        if (PieceAutoFallTimer(this))
        {
            lastMoveTimeClass = Time.time;
            ClearPiece(this);
            if (IsValidLocation(this, position + Vector2Int.down))
            {
                MovePieceDown(this);
            }
            SetPiece(this);

        }
        

    }

    public void LoadPiece(TetrominoData tetrominoData, Vector2Int _position)
    {
        //Initialize class
        data = tetrominoData;
        blocks = data.blocks;
        position = _position;
        bounds = player.bounds;
        lastMoveTimeClass = Time.time;
        SetPiece(this);
        
    }

    public void StopClass()
    {
        data = null;
    }

    public void ResumeClass(TetrominoData tetrominoData)
    {
        data = tetrominoData;
    }

    public void ClearPiece(Piece piece)
    {
        //set all tiles on tilemap containing the piece clear
        foreach (Vector2Int block in piece.blocks)
        {
            Vector3Int blockPosition = new Vector3Int(block.x + piece.position.x, block.y + piece.position.y, 0);
            tilemap.SetTile(blockPosition, null);
        }
    }

    public bool IsValidLocation(Piece piece, Vector2Int checkPosition)
    {
        //checks tile validity for all blocks in piece
        foreach (Vector2Int block in piece.blocks)
        {
            Vector2Int tilePosition = block + checkPosition;
            
            //check if all blocks are within bounds
            if (!bounds.Contains(tilePosition))
            {
                print(tilePosition + "out of bounds, " + "bounds: " + bounds);
                return false;
            }

            //check if space is not already occupied by tile
            if (tilemap.HasTile((Vector3Int)tilePosition))
            {
                print("tile occupied");
                return false;
            }
        }
        return true;
    }

    public bool PieceAutoFallTimer(Piece piece)
    {
        //check if piece should auto fall (true = should autofall)
        float lastMoveTime = piece.lastMoveTimeClass;

        if (currTime - lastMoveTime > autoFallTime){
            return true;
        }
        return false;
    }

    public bool PieceLockTimer(Piece piece)
    {
        //check if piece should lock movement (true = should lock piece)
        float lastMoveTime = piece.lastMoveTimeClass;

        if (currTime - lastMoveTime > maxLockTime)
        {
            return true;
        }
        return false;

    }

    public void MovePieceDown(Piece piece)
    {
        piece.position += Vector2Int.down;
    }

    public void SetPiece(Piece piece)
    {
       

        //set all tiles on the tilemap containing the piece to correct color tile
        foreach (Vector2Int block in piece.blocks)
        {
            Vector3Int blockPosition = new Vector3Int(block.x + position.x, block.y + position.y, 0);
            tilemap.SetTile(blockPosition, piece.data.tile);
        }
    }
}
