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
public struct TetrominoData
{
    public Tetromino tetromino;
    public Tile tile;
    public Vector2Int[] blocks { get; private set; }
    public Vector2Int[,] wallKicks;
    public Vector2Int startPositionOffset;

    public void Initialize()
    {
        this.blocks = Data.Blocks[this.tetromino];
        this.wallKicks = Data.WallKicks[this.tetromino];
        if (this.tetromino == Tetromino.I)
        {
            this.startPositionOffset = new Vector2Int(0, -1);
        }
        else
        {
            this.startPositionOffset = new Vector2Int(0, 0);
        }
    }

    //public TetrominoData DeepCopy()
    //{
    //    TetrominoData other = (TetrominoData) this.MemberwiseClone();
    //    other.blocks = new Vector2Int[this.blocks.Length](this.blocks);

    //    return other;
    //}
}