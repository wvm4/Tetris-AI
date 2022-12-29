using System.Collections;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
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
    public int nextRotation; //0 = none, 1 = right, -1 = left
    public int nextMovement; //0 = none, 1 = right, -1 = left
    float currTime;
    RectInt bounds;


    public void Update()
    {
        if (data == null) //if the piece has not been initialized yet, do not execute update func
        {
            return;
        }
        currTime = Time.time;

        ClearPiece(this);

        if (nextRotation != 0)
        {
            lastMoveTimeClass = Time.time;
            RotatePiece(this);
            nextRotation = 0;
            nextMovement = 0;
        }

        if (nextMovement != 0)
        {
            lastMoveTimeClass = Time.time;
            MovePiece(this);
            nextMovement = 0;
        }

        if (PieceAutoFallTimer(this))
        {
            lastMoveTimeClass = Time.time;
            if (IsValidLocation(this, position + Vector2Int.down))
            {
                MovePieceDown(this);
            }

        }
        
        

        SetPiece(this);

    }

    public void LoadPiece(TetrominoData tetrominoData, Vector2Int _position)
    {
        //Initialize class
        data = tetrominoData;
        blocks = data.blocks;
        position = _position;
        bounds = player.bounds;
        rotationIndex = 0;
        lastMoveTimeClass = Time.time;
        SetPiece(this);
        
    }

    public void StopClass()
    {
        //stop class from executing update func by setting data to null
        data = null;
    }

    public void ResumeClass(TetrominoData tetrominoData)
    {
        //resume class, inverse of stopclass func
        data = tetrominoData;
    }

    public void SetPiece(Piece piece)
    {
        Vector2Int tilePositions = piece.position;

        //set all tiles on the tilemap containing the piece to correct color tile
        foreach (Vector2Int block in piece.blocks)
        {
            Vector3Int blockPosition = new Vector3Int(block.x + tilePositions.x, block.y + tilePositions.y, 0);
            tilemap.SetTile(blockPosition, piece.data.tile);
        }
    }

    public void ClearPiece(Piece piece)
    {
        Vector2Int tilePositions = piece.position;

        //set all tiles on tilemap containing the piece clear
        foreach (Vector2Int block in piece.blocks)
        {
            Vector3Int blockPosition = new Vector3Int(block.x + tilePositions.x, block.y + tilePositions.y, 0);
            tilemap.SetTile(blockPosition, null);
        }
    }


    
    public void RotateTiles(Piece piece, int direction)
    {
        float[] matrix = Data.RotationMatrix;
 
        // Rotate all of the piece.blocks using the rotation matrix
        for (int i = 0; i < piece.blocks.Length; i++)
        {
            Vector2 cell = piece.blocks[i];

            int x, y;

            switch (data.tetromino)
            {
                case Tetromino.I:
                case Tetromino.O:
                    // "I" and "O" are rotated from an offset center point
                    cell.x -= 0.5f;
                    cell.y -= 0.5f;
                    x = Mathf.CeilToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.CeilToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;

                default:
                    x = Mathf.RoundToInt((cell.x * matrix[0] * direction) + (cell.y * matrix[1] * direction));
                    y = Mathf.RoundToInt((cell.x * matrix[2] * direction) + (cell.y * matrix[3] * direction));
                    break;
            }

            piece.blocks[i] = new Vector2Int(x, y);
        }
    }

    public void RotatePiece(Piece piece)
    {
        RotateTiles(piece, piece.nextRotation);
        int kickIndex = piece.rotationIndex * 2;
        

        if (piece.nextRotation == -1)
        {
            kickIndex--;
        }

        if (kickIndex < 0)
        {
            kickIndex = 7;
        } else if (kickIndex == 8)
        {
            kickIndex = 0;
        }

        
        for (int i = 0; i < piece.data.wallKicks.GetLength(1); i++){
            if (IsValidLocation(piece, piece.position + piece.data.wallKicks[kickIndex, i]))
            {
                piece.position += piece.data.wallKicks[kickIndex, i];
                piece.rotationIndex += piece.nextRotation;

                if (rotationIndex < 0)
                {
                    rotationIndex = 3;
                }
                else if (rotationIndex == 4)
                {
                    rotationIndex = 0;
                }
                return;
            } 
        }
        RotateTiles(piece, -piece.nextRotation);

    }

    public void MovePiece(Piece piece)
    {
        Vector2Int direction = new Vector2Int(piece.nextMovement, 0);

        if (IsValidLocation(piece, piece.position + direction)) 
        {
            piece.position += direction;
        }
    }

    public bool IsValidLocation(Piece piece, Vector2Int checkPosition)
    {
        //checks tile validity for all blocks in piece
        foreach (Vector2Int block in piece.blocks)
        {
            Vector2Int tilePosition = block + checkPosition;

            if (!IsWithinBounds(tilePosition))
            {
                return false;  
            }

            if (!IsTileValid(tilePosition))
            {
                return false;
            }
        }
        return true;
    }

    public bool IsWithinBounds(Vector2Int tilePosition)
    {
        //check if all blocks are within bounds
        if (!bounds.Contains(tilePosition))
        {
            print(tilePosition + "out of bounds, " + "bounds: " + bounds);
            return false;
        }
        return true;
    }

    public bool IsTileValid(Vector2Int tilePosition)
    {
        //check if space is not already occupied by tile
        if (tilemap.HasTile((Vector3Int)tilePosition))
        {
            print("tile occupied");
            return false;
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


}
