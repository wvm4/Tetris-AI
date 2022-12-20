using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum Tetromino
{
    I,
    O,
    T,
    J,
    L,
    S,
    Z,
}

[System.Serializable]
public class TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] blocks;
    public Vector2Int[,] wallKicks;

    public void Initialize()
    {
        this.blocks = Data.Blocks[this.tetromino];
        this.wallKicks = Data.WallKicks[this.tetromino];
    }

}